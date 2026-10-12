import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ConfirmacaoService } from '../../core/feedback/confirmacao.service';
import { ToastService } from '../../core/feedback/toast.service';
import { mensagemDeErro } from '../../core/http-error';
import { Conta } from '../../core/models';
import { rotuloTipoConta, soma } from '../../core/rotulos';
import { ContasService } from '../../core/services/contas.service';
import { Dialog } from '../../shared/dialog';
import { EmptyState } from '../../shared/empty-state';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { SkeletonList } from '../../shared/skeleton-list';
import { ContaForm } from './conta-form';

@Component({
  selector: 'app-contas-page',
  imports: [CurrencyPipe, DatePipe, PageHeader, EmptyState, SkeletonList, Icon, Dialog, ContaForm],
  templateUrl: './contas-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContasPage {
  private readonly service = inject(ContasService);
  private readonly toast = inject(ToastService);
  private readonly confirmacao = inject(ConfirmacaoService);

  protected readonly rotuloTipoConta = rotuloTipoConta;

  protected readonly contas = signal<Conta[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly saldoInicialTotal = computed(() =>
    soma(this.contas().map((c) => c.saldoInicial)),
  );

  /** `undefined`: formulário fechado; `null`: nova conta. */
  protected readonly emEdicao = signal<Conta | null | undefined>(undefined);
  protected readonly tituloFormulario = computed(() =>
    this.emEdicao() ? 'Editar conta' : 'Nova conta',
  );

  constructor() {
    this.carregar();
  }

  protected carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);
    this.service.listar().subscribe({
      next: (contas) => {
        this.contas.set(contas);
        this.carregando.set(false);
      },
      error: (erro: unknown) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível carregar as contas.'));
        this.carregando.set(false);
      },
    });
  }

  /** Contas anteriores à migração `AdicionarDatasConta` têm data padrão (ano 1). */
  protected criadaEm(conta: Conta): string | null {
    return conta.dataCriacao.startsWith('0001') ? null : conta.dataCriacao;
  }

  protected novaConta(): void {
    this.emEdicao.set(null);
  }

  protected editar(conta: Conta): void {
    this.emEdicao.set(conta);
  }

  protected fecharFormulario(): void {
    this.emEdicao.set(undefined);
  }

  protected aoSalvar(): void {
    const nova = this.emEdicao() === null;
    this.fecharFormulario();
    this.toast.sucesso(nova ? 'Conta cadastrada.' : 'Conta atualizada.');
    this.carregar();
  }

  protected async excluir(conta: Conta): Promise<void> {
    const confirmado = await this.confirmacao.confirmar({
      titulo: 'Excluir conta',
      mensagem: `A conta "${conta.nome}" será excluída junto com todas as transações e metas vinculadas a ela. Esta ação não pode ser desfeita.`,
      confirmar: 'Excluir conta',
      destrutiva: true,
    });
    if (!confirmado) return;

    this.service.excluir(conta.contaId).subscribe({
      next: () => {
        this.toast.sucesso('Conta excluída.');
        this.carregar();
      },
      error: (erro: unknown) =>
        this.toast.erro(mensagemDeErro(erro, 'Não foi possível excluir a conta.')),
    });
  }
}
