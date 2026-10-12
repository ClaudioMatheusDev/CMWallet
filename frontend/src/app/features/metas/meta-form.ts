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
import { paraDataISO, somarMeses } from '../../core/datas';
import { mensagemDeErro } from '../../core/http-error';
import { Conta, Meta, MetaInput } from '../../core/models';
import { MetasService } from '../../core/services/metas.service';
import { Icon } from '../../shared/icon';

@Component({
  selector: 'app-meta-form',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './meta-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetaForm implements OnInit {
  readonly meta = input<Meta | null>(null);
  readonly contas = input.required<Conta[]>();
  readonly salvo = output<void>();
  readonly cancelado = output<void>();

  private readonly service = inject(MetasService);

  protected readonly salvando = signal(false);
  protected readonly enviado = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = inject(FormBuilder).nonNullable.group({
    valorMeta: [null as number | null, [Validators.required, Validators.min(0.01)]],
    valorAtual: [0 as number | null, [Validators.required, Validators.min(0)]],
    dataMeta: [paraDataISO(somarMeses(new Date(), 6)), Validators.required],
    contaId: [null as number | null, Validators.required],
  });

  ngOnInit(): void {
    const meta = this.meta();
    if (meta) {
      this.form.setValue({
        valorMeta: meta.valorMeta,
        valorAtual: meta.valorAtual,
        dataMeta: meta.dataMeta.slice(0, 10),
        contaId: meta.contaId,
      });
    } else if (this.contas().length === 1) {
      this.form.controls.contaId.setValue(this.contas()[0].contaId);
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
    const dados: MetaInput = {
      valorMeta: valores.valorMeta ?? 0,
      valorAtual: valores.valorAtual ?? 0,
      dataMeta: valores.dataMeta,
      contaId: valores.contaId ?? 0,
    };

    const existente = this.meta();
    const requisicao: Observable<unknown> = existente
      ? this.service.atualizar(existente.metaId, dados)
      : this.service.criar(dados);

    this.salvando.set(true);
    this.erro.set(null);
    requisicao.pipe(finalize(() => this.salvando.set(false))).subscribe({
      next: () => this.salvo.emit(),
      error: (erro: unknown) =>
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a meta.')),
    });
  }
}
