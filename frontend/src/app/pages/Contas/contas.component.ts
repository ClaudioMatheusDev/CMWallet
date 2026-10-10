import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { ContaService } from '../../services/conta.service';
import { Conta, TipoConta } from '../../models/conta';

@Component({ selector: 'app-contas', imports: [CurrencyPipe, DatePipe, FormsModule], templateUrl: './contas.component.html' })
export class ContasComponent implements OnInit {
  private readonly contaService = inject(ContaService);
  private readonly changeDetector = inject(ChangeDetectorRef);
  readonly contas = signal<Conta[]>([]);
  readonly erro = signal('');
  readonly carregando = signal(true);
  readonly contaCount = computed(() => this.contas().length);
  readonly saldoTotal = computed(() => this.contas().reduce((total, conta) => total + conta.saldoInicial, 0));
  readonly maiorConta = computed(() => this.contas().reduce<Conta | null>((maior, conta) => maior === null || conta.saldoInicial > maior.saldoInicial ? conta : maior, null));
  readonly tipoContaLabels: Record<TipoConta, string> = { [TipoConta.ContaCorrente]: 'Conta corrente', [TipoConta.ContaPoupanca]: 'Conta poupança', [TipoConta.Dinheiro]: 'Dinheiro' };
  formAberto = false;
  contaEditandoId: number | null = null;
  salvando = false;
  erroForm = '';
  form = { nome: '', saldoInicial: 0, tipoConta: TipoConta.ContaCorrente };

  ngOnInit(): void { this.carregarContas(); }
  carregarContas(): void {
    this.carregando.set(true);
    this.contaService.listarContas().pipe(finalize(() => {
      this.carregando.set(false);
      this.changeDetector.markForCheck();
    })).subscribe({
      next: (dados) => { this.contas.set(dados); this.erro.set(''); },
      error: () => { this.erro.set('Não foi possível carregar as contas. Verifique a conexão com a API.'); this.changeDetector.markForCheck(); },
    });
  }
  abrirNovaConta(): void { this.form = { nome: '', saldoInicial: 0, tipoConta: TipoConta.ContaCorrente }; this.contaEditandoId = null; this.erroForm = ''; this.formAberto = true; }
  editarConta(conta: Conta): void { this.contaEditandoId = conta.contaId; this.form = { nome: conta.nome, saldoInicial: conta.saldoInicial, tipoConta: conta.tipoConta }; this.erroForm = ''; this.formAberto = true; }
  fecharForm(): void { this.formAberto = false; this.erroForm = ''; }
  salvarConta(): void {
    this.salvando = true;
    const request = this.contaEditandoId ? this.contaService.atualizarConta(this.contaEditandoId, this.form) : this.contaService.criarConta(this.form);
    request.pipe(finalize(() => {
      this.salvando = false;
      this.changeDetector.markForCheck();
    })).subscribe({
      next: () => { this.fecharForm(); this.changeDetector.markForCheck(); this.carregarContas(); },
      error: (error: unknown) => { this.erroForm = this.mensagemErro(error, 'Não foi possível salvar a conta.'); },
    });
  }
  excluirConta(conta: Conta): void {
    if (!confirm(`Excluir a conta "${conta.nome}"? A operação pode ser impedida se ela estiver vinculada a registros.`)) return;
    this.contaService.excluirConta(conta.contaId).subscribe({
      next: () => this.carregarContas(),
      error: (error: unknown) => {
        this.erro.set(this.mensagemErro(error, 'Não foi possível excluir esta conta.'));
        this.changeDetector.markForCheck();
      },
    });
  }
  private mensagemErro(error: unknown, padrao: string): string { const body = (error as { error?: { message?: string; title?: string } })?.error; return body?.message || body?.title || padrao; }
}
