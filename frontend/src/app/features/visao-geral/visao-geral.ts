import { CurrencyPipe, DatePipe, formatDate } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  LOCALE_ID,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { forkJoin, map } from 'rxjs';
import { mensagemDeErro } from '../../core/http-error';
import { Conta, Meta, Resumo, TipoMovimento, Transacao } from '../../core/models';
import { rotuloTipoConta, soma } from '../../core/rotulos';
import { ContasService } from '../../core/services/contas.service';
import { MetasService } from '../../core/services/metas.service';
import { ResumoService } from '../../core/services/resumo.service';
import { TransacoesService } from '../../core/services/transacoes.service';
import { EmptyState } from '../../shared/empty-state';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { resumirMeta } from '../metas/resumo-meta';

interface DadosPainel {
  contas: Conta[];
  metas: Meta[];
  resumo: Resumo | null;
  recentes: Transacao[];
}

const CATEGORIAS_EXIBIDAS = 6;

@Component({
  selector: 'app-visao-geral',
  imports: [CurrencyPipe, DatePipe, RouterLink, PageHeader, EmptyState, Icon],
  templateUrl: './visao-geral.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VisaoGeral {
  private readonly contasService = inject(ContasService);
  private readonly metasService = inject(MetasService);
  private readonly resumoService = inject(ResumoService);
  private readonly transacoesService = inject(TransacoesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly TipoMovimento = TipoMovimento;
  protected readonly rotuloTipoConta = rotuloTipoConta;

  private readonly hoje = new Date();
  protected readonly mesAtual = formatDate(this.hoje, "MMMM 'de' y", inject(LOCALE_ID));

  protected readonly estado = signal<'carregando' | 'erro' | 'pronto'>('carregando');
  protected readonly erro = signal('');
  private readonly dados = signal<DadosPainel>({ contas: [], metas: [], resumo: null, recentes: [] });

  protected readonly contas = computed(() => this.dados().contas);
  protected readonly recentes = computed(() => this.dados().recentes);
  protected readonly resumo = computed(() => this.dados().resumo);

  protected readonly saldoTotal = computed(() => soma(this.contas().map((c) => c.saldoAtual)));

  protected readonly gastosPorCategoria = computed(() =>
    (this.resumo()?.categorias ?? [])
      .filter((c) => c.tipo === TipoMovimento.Saida)
      .slice(0, CATEGORIAS_EXIBIDAS),
  );

  protected readonly metas = computed(() => {
    const nomes = new Map(this.contas().map((c) => [c.contaId, c.nome]));
    return this.dados()
      .metas.map((m) => resumirMeta(m, nomes.get(m.contaId) ?? `Conta #${m.contaId}`, this.hoje))
      .filter((m) => m.situacao !== 'concluida')
      .sort((a, b) => a.meta.dataMeta.localeCompare(b.meta.dataMeta))
      .slice(0, 4);
  });

  constructor() {
    this.carregar();
  }

  protected carregar(): void {
    this.estado.set('carregando');

    forkJoin({
      contas: this.contasService.listar(),
      metas: this.metasService.listar(),
      resumo: this.resumoService.obter(),
      recentes: this.transacoesService
        .listar({ pagina: 1, tamanhoPagina: 6 })
        .pipe(map((pagina) => pagina.itens)),
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (dados) => {
          this.dados.set(dados);
          this.estado.set('pronto');
        },
        error: (erro: unknown) => {
          this.erro.set(mensagemDeErro(erro, 'Não foi possível carregar o resumo.'));
          this.estado.set('erro');
        },
      });
  }

  protected lancamentos(quantidade: number): string {
    return quantidade === 1 ? '1 lançamento' : `${quantidade} lançamentos`;
  }
}
