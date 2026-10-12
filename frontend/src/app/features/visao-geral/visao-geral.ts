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
import { intervaloDoMes } from '../../core/datas';
import { mensagemDeErro } from '../../core/http-error';
import { Conta, Meta, TipoMovimento, Transacao } from '../../core/models';
import { rotuloTipoConta, soma } from '../../core/rotulos';
import { ContasService } from '../../core/services/contas.service';
import { MetasService } from '../../core/services/metas.service';
import { TransacoesService } from '../../core/services/transacoes.service';
import { EmptyState } from '../../shared/empty-state';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { resumirMeta } from '../metas/resumo-meta';

interface DadosPainel {
  contas: Conta[];
  metas: Meta[];
  transacoesDoMes: Transacao[];
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
  private readonly transacoesService = inject(TransacoesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly TipoMovimento = TipoMovimento;
  protected readonly rotuloTipoConta = rotuloTipoConta;

  private readonly hoje = new Date();
  protected readonly mesAtual = formatDate(this.hoje, "MMMM 'de' y", inject(LOCALE_ID));

  protected readonly estado = signal<'carregando' | 'erro' | 'pronto'>('carregando');
  protected readonly erro = signal('');
  private readonly dados = signal<DadosPainel>({
    contas: [],
    metas: [],
    transacoesDoMes: [],
    recentes: [],
  });

  protected readonly contas = computed(() => this.dados().contas);
  protected readonly recentes = computed(() => this.dados().recentes);

  private readonly entradas = computed(() =>
    this.dados().transacoesDoMes.filter((t) => t.tipo === TipoMovimento.Entrada),
  );
  private readonly saidas = computed(() =>
    this.dados().transacoesDoMes.filter((t) => t.tipo === TipoMovimento.Saida),
  );

  protected readonly qtdEntradas = computed(() => this.entradas().length);
  protected readonly qtdSaidas = computed(() => this.saidas().length);
  protected readonly totalEntradas = computed(() => soma(this.entradas().map((t) => t.valor)));
  protected readonly totalSaidas = computed(() => soma(this.saidas().map((t) => t.valor)));
  protected readonly resultado = computed(() => this.totalEntradas() - this.totalSaidas());

  private readonly pendentes = computed(() => this.saidas().filter((t) => !t.pago));
  protected readonly qtdPendentes = computed(() => this.pendentes().length);
  protected readonly totalPendente = computed(() => soma(this.pendentes().map((t) => t.valor)));

  protected readonly saldoInicialTotal = computed(() =>
    soma(this.contas().map((c) => c.saldoInicial)),
  );

  protected readonly gastosPorCategoria = computed(() => {
    const total = this.totalSaidas();
    const porCategoria = new Map<string, number>();
    for (const t of this.saidas()) {
      porCategoria.set(t.categoriaNome, (porCategoria.get(t.categoriaNome) ?? 0) + t.valor);
    }
    return [...porCategoria]
      .map(([nome, valor]) => ({ nome, valor, percentual: total > 0 ? (valor / total) * 100 : 0 }))
      .sort((a, b) => b.valor - a.valor)
      .slice(0, CATEGORIAS_EXIBIDAS);
  });

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
    const mes = intervaloDoMes(this.hoje);

    forkJoin({
      contas: this.contasService.listar(),
      metas: this.metasService.listar(),
      transacoesDoMes: this.transacoesService.listarTodas({
        dataInicio: mes.inicio,
        dataFim: mes.fim,
      }),
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
