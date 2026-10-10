
import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';

import { ContaService } from './services/conta.service';
import { Conta, TipoConta } from './models/conta';

@Component({
  selector: 'app-contas',
  imports: [CurrencyPipe],
  templateUrl: './contas.component.html',
})
export class ContasComponent implements OnInit {
  private readonly contaService = inject(ContaService);

  contas: Conta[] = [];
  erro = '';
  carregando = true;

  readonly tipoContaLabels: Record<TipoConta, string> = {
    [TipoConta.ContaCorrente]: 'Conta corrente',
    [TipoConta.ContaPoupanca]: 'Conta poupança',
    [TipoConta.Dinheiro]: 'Dinheiro',
  };

  ngOnInit(): void {
    this.contaService.listarContas().subscribe({
      next: (dados) => {
        this.contas = dados;
      },
      error: (error: unknown) => {
        console.error('Erro ao buscar contas:', error);
        this.erro = 'Não foi possível carregar as contas.';
        this.carregando = false;
      },
      complete: () => {
        this.carregando = false;
      },
    });
  }
}
