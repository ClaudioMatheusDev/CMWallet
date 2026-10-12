import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subject, catchError, debounceTime, forkJoin, of, switchMap, tap } from 'rxjs';
import { ConfirmacaoService } from '../../core/feedback/confirmacao.service';
import { ToastService } from '../../core/feedback/toast.service';
import { mensagemDeErro } from '../../core/http-error';
import {
  Categoria,
  Conta,
  FiltroTransacoes,
  Pagina,
  TipoMovimento,
  Transacao,
} from '../../core/models';
import { CategoriasService } from '../../core/services/categorias.service';
import { ContasService } from '../../core/services/contas.service';
import { TransacoesService } from '../../core/services/transacoes.service';
import { Dialog } from '../../shared/dialog';
import { EmptyState } from '../../shared/empty-state';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { SkeletonList } from '../../shared/skeleton-list';
import { TransacaoForm } from './transacao-form';

const TAMANHO_PAGINA = 20;

@Component({
  selector: 'app-transacoes-page',
  imports: [
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    RouterLink,
    PageHeader,
    EmptyState,
    SkeletonList,
    Icon,
    Dialog,
    TransacaoForm,
  ],
  templateUrl: './transacoes-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TransacoesPage {
  private readonly transacoesService = inject(TransacoesService);
  private readonly contasService = inject(ContasService);
  private readonly categoriasService = inject(CategoriasService);
  private readonly toast = inject(ToastService);
  private readonly confirmacao = inject(ConfirmacaoService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly TipoMovimento = TipoMovimento;

  protected readonly contas = signal<Conta[]>([]);
  protected readonly categorias = signal<Categoria[]>([]);
  private readonly cadastrosCarregados = signal(false);
  protected readonly faltamCadastros = computed(
    () => this.cadastrosCarregados() && (!this.contas().length || !this.categorias().length),
  );
  protected readonly categoriasDeEntrada = computed(() =>
    this.categorias().filter((c) => c.tipo === TipoMovimento.Entrada),
  );
  protected readonly categoriasDeSaida = computed(() =>
    this.categorias().filter((c) => c.tipo === TipoMovimento.Saida),
  );

  protected readonly filtros = inject(FormBuilder).nonNullable.group({
    contaId: [null as number | null],
    categoriaId: [null as number | null],
    tipo: [null as TipoMovimento | null],
    dataInicio: [''],
    dataFim: [''],
  });
  private readonly valoresFiltro = toSignal(this.filtros.valueChanges, {
    initialValue: this.filtros.getRawValue(),
  });
  protected readonly filtrosAtivos = computed(() => {
    const f = this.valoresFiltro();
    return (
      f.contaId != null || f.categoriaId != null || f.tipo != null || !!f.dataInicio || !!f.dataFim
    );
  });

  protected readonly pagina = signal(1);
  protected readonly resultado = signal<Pagina<Transacao> | null>(null);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly itens = computed(() => this.resultado()?.itens ?? []);
  protected readonly total = computed(() => this.resultado()?.total ?? 0);
  protected readonly totalPaginas = computed(() => Math.max(1, this.resultado()?.totalPaginas ?? 1));
  protected readonly intervalo = computed(() => {
    const r = this.resultado();
    if (!r || r.itens.length === 0) return null;
    const inicio = (r.pagina - 1) * r.tamanhoPagina + 1;
    return { inicio, fim: inicio + r.itens.length - 1 };
  });

  /** `undefined`: formulário fechado; `null`: nova transação. */
  protected readonly emEdicao = signal<Transacao | null | undefined>(undefined);
  protected readonly tituloFormulario = computed(() =>
    this.emEdicao() ? 'Editar transação' : 'Nova transação',
  );

  private readonly recarregar$ = new Subject<void>();

  constructor() {
    this.recarregar$
      .pipe(
        tap(() => {
          this.carregando.set(true);
          this.erro.set(null);
        }),
        switchMap(() =>
          this.transacoesService.listar(this.filtroAtual()).pipe(
            catchError((erro: unknown) => {
              this.erro.set(mensagemDeErro(erro, 'Não foi possível carregar as transações.'));
              return of(null);
            }),
          ),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((resultado) => {
        // Ao excluir o último item da última página, volta para a última página com dados.
        if (resultado && resultado.itens.length === 0 && resultado.total > 0 && this.pagina() > 1) {
          this.pagina.set(resultado.totalPaginas);
          this.recarregar$.next();
          return;
        }
        this.resultado.set(resultado);
        this.carregando.set(false);
      });

    this.filtros.valueChanges.pipe(debounceTime(250), takeUntilDestroyed()).subscribe(() => {
      this.pagina.set(1);
      this.recarregar$.next();
    });

    forkJoin({ contas: this.contasService.listar(), categorias: this.categoriasService.listar() })
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: ({ contas, categorias }) => {
          this.contas.set(contas);
          this.categorias.set(categorias);
          this.cadastrosCarregados.set(true);
          this.abrirFormularioSeSolicitado();
        },
        error: (erro: unknown) =>
          this.toast.erro(mensagemDeErro(erro, 'Não foi possível carregar contas e categorias.')),
      });

    this.recarregar$.next();
  }

  protected recarregar(): void {
    this.recarregar$.next();
  }

  protected irParaPagina(pagina: number): void {
    this.pagina.set(pagina);
    this.recarregar$.next();
  }

  protected limparFiltros(): void {
    this.filtros.reset();
  }

  protected novaTransacao(): void {
    if (!this.faltamCadastros()) {
      this.emEdicao.set(null);
    }
  }

  protected editar(transacao: Transacao): void {
    this.emEdicao.set(transacao);
  }

  protected fecharFormulario(): void {
    this.emEdicao.set(undefined);
  }

  protected aoSalvar(): void {
    const nova = this.emEdicao() === null;
    this.fecharFormulario();
    this.toast.sucesso(nova ? 'Transação registrada.' : 'Transação atualizada.');
    this.recarregar$.next();
  }

  protected async excluir(transacao: Transacao): Promise<void> {
    const confirmado = await this.confirmacao.confirmar({
      titulo: 'Excluir transação',
      mensagem: `A transação "${transacao.descricao}" será excluída permanentemente.`,
      confirmar: 'Excluir',
      destrutiva: true,
    });
    if (!confirmado) return;

    this.transacoesService.excluir(transacao.transacaoId).subscribe({
      next: () => {
        this.toast.sucesso('Transação excluída.');
        this.recarregar$.next();
      },
      error: (erro: unknown) =>
        this.toast.erro(mensagemDeErro(erro, 'Não foi possível excluir a transação.')),
    });
  }

  /** Atende ao atalho "Nova transação" da visão geral (`/transacoes?nova=1`). */
  private abrirFormularioSeSolicitado(): void {
    if (!this.route.snapshot.queryParamMap.has('nova')) return;
    this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
    this.novaTransacao();
  }

  private filtroAtual(): FiltroTransacoes {
    const f = this.filtros.getRawValue();
    return {
      contaId: f.contaId ?? undefined,
      categoriaId: f.categoriaId ?? undefined,
      tipo: f.tipo ?? undefined,
      dataInicio: f.dataInicio || undefined,
      dataFim: f.dataFim || undefined,
      pagina: this.pagina(),
      tamanhoPagina: TAMANHO_PAGINA,
    };
  }
}
