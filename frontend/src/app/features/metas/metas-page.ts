import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ConfirmacaoService } from '../../core/feedback/confirmacao.service';
import { ToastService } from '../../core/feedback/toast.service';
import { mensagemDeErro } from '../../core/http-error';
import { Conta, Meta } from '../../core/models';
import { soma } from '../../core/rotulos';
import { ContasService } from '../../core/services/contas.service';
import { MetasService } from '../../core/services/metas.service';
import { Dialog } from '../../shared/dialog';
import { EmptyState } from '../../shared/empty-state';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { MetaForm } from './meta-form';
import { ROTULO_SITUACAO, ResumoMeta, resumirMeta } from './resumo-meta';

@Component({
  selector: 'app-metas-page',
  imports: [
    CurrencyPipe,
    DatePipe,
    DecimalPipe,
    RouterLink,
    PageHeader,
    EmptyState,
    Icon,
    Dialog,
    MetaForm,
  ],
  templateUrl: './metas-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetasPage {
  private readonly metasService = inject(MetasService);
  private readonly contasService = inject(ContasService);
  private readonly toast = inject(ToastService);
  private readonly confirmacao = inject(ConfirmacaoService);

  protected readonly rotuloSituacao = ROTULO_SITUACAO;

  protected readonly metas = signal<Meta[]>([]);
  protected readonly contas = signal<Conta[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly resumos = computed<ResumoMeta[]>(() => {
    const nomes = new Map(this.contas().map((c) => [c.contaId, c.nome]));
    return this.metas().map((m) => resumirMeta(m, nomes.get(m.contaId) ?? `Conta #${m.contaId}`));
  });
  protected readonly totalGuardado = computed(() => soma(this.metas().map((m) => m.valorAtual)));
  protected readonly totalObjetivo = computed(() => soma(this.metas().map((m) => m.valorMeta)));
  protected readonly progressoGeral = computed(() => {
    const objetivo = this.totalObjetivo();
    return objetivo > 0 ? Math.min(100, (this.totalGuardado() / objetivo) * 100) : 0;
  });
  protected readonly concluidas = computed(
    () => this.resumos().filter((r) => r.situacao === 'concluida').length,
  );

  /** `undefined`: formulário fechado; `null`: nova meta. */
  protected readonly emEdicao = signal<Meta | null | undefined>(undefined);
  protected readonly tituloFormulario = computed(() =>
    this.emEdicao() ? 'Editar meta' : 'Nova meta',
  );

  constructor() {
    this.carregar();
  }

  protected carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);
    forkJoin({ metas: this.metasService.listar(), contas: this.contasService.listar() }).subscribe({
      next: ({ metas, contas }) => {
        this.metas.set(metas);
        this.contas.set(contas);
        this.carregando.set(false);
      },
      error: (erro: unknown) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível carregar as metas.'));
        this.carregando.set(false);
      },
    });
  }

  protected novaMeta(): void {
    this.emEdicao.set(null);
  }

  protected editar(meta: Meta): void {
    this.emEdicao.set(meta);
  }

  protected fecharFormulario(): void {
    this.emEdicao.set(undefined);
  }

  protected aoSalvar(): void {
    const nova = this.emEdicao() === null;
    this.fecharFormulario();
    this.toast.sucesso(nova ? 'Meta criada.' : 'Meta atualizada.');
    this.carregar();
  }

  protected async excluir(meta: Meta): Promise<void> {
    const confirmado = await this.confirmacao.confirmar({
      titulo: 'Excluir meta',
      mensagem: 'A meta será excluída permanentemente.',
      confirmar: 'Excluir',
      destrutiva: true,
    });
    if (!confirmado) return;

    this.metasService.excluir(meta.metaId).subscribe({
      next: () => {
        this.toast.sucesso('Meta excluída.');
        this.carregar();
      },
      error: (erro: unknown) =>
        this.toast.erro(mensagemDeErro(erro, 'Não foi possível excluir a meta.')),
    });
  }
}
