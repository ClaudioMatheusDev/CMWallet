import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize } from 'rxjs';
import { mensagemDeErro } from '../../core/http-error';
import { Conta, ContaInput, TipoConta } from '../../core/models';
import { TIPOS_CONTA } from '../../core/rotulos';
import { ContasService } from '../../core/services/contas.service';
import { textoObrigatorio } from '../../core/validadores';
import { Icon } from '../../shared/icon';

@Component({
  selector: 'app-conta-form',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './conta-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContaForm implements OnInit {
  readonly conta = input<Conta | null>(null);
  readonly salvo = output<void>();
  readonly cancelado = output<void>();

  private readonly service = inject(ContasService);

  protected readonly tiposConta = TIPOS_CONTA;
  protected readonly salvando = signal(false);
  protected readonly enviado = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = inject(FormBuilder).nonNullable.group({
    nome: ['', [textoObrigatorio, Validators.maxLength(200)]],
    tipoConta: [TipoConta.ContaCorrente as TipoConta],
    saldoInicial: [0 as number | null, Validators.required],
  });

  ngOnInit(): void {
    const conta = this.conta();
    if (conta) {
      this.form.setValue({
        nome: conta.nome,
        tipoConta: conta.tipoConta,
        saldoInicial: conta.saldoInicial,
      });
    }
  }

  protected invalido(campo: string): boolean {
    const controle = this.form.get(campo);
    return !!controle && controle.invalid && (controle.touched || this.enviado());
  }

  protected salvar(): void {
    this.enviado.set(true);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();
    const dados: ContaInput = {
      nome: valores.nome.trim(),
      tipoConta: valores.tipoConta,
      saldoInicial: valores.saldoInicial ?? 0,
    };

    const existente = this.conta();
    const requisicao: Observable<unknown> = existente
      ? this.service.atualizar(existente.contaId, dados)
      : this.service.criar(dados);

    this.salvando.set(true);
    this.erro.set(null);
    requisicao.pipe(finalize(() => this.salvando.set(false))).subscribe({
      next: () => this.salvo.emit(),
      error: (erro: unknown) =>
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a conta.')),
    });
  }
}
