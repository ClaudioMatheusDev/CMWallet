import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize } from 'rxjs';
import { hojeISO } from '../../core/datas';
import { mensagemDeErro } from '../../core/http-error';
import { Categoria, Conta, TipoMovimento, Transacao, TransacaoInput } from '../../core/models';
import { TransacoesService } from '../../core/services/transacoes.service';
import { textoObrigatorio } from '../../core/validadores';
import { Icon } from '../../shared/icon';

@Component({
  selector: 'app-transacao-form',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './transacao-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TransacaoForm implements OnInit {
  readonly transacao = input<Transacao | null>(null);
  readonly contas = input.required<Conta[]>();
  readonly categorias = input.required<Categoria[]>();
  readonly salvo = output<void>();
  readonly cancelado = output<void>();

  private readonly service = inject(TransacoesService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly TipoMovimento = TipoMovimento;
  protected readonly salvando = signal(false);
  protected readonly enviado = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = inject(FormBuilder).nonNullable.group({
    tipo: [TipoMovimento.Saida as TipoMovimento],
    descricao: ['', [textoObrigatorio, Validators.maxLength(300)]],
    valor: [null as number | null, [Validators.required, Validators.min(0.01)]],
    data: [hojeISO(), Validators.required],
    categoriaId: [null as number | null, Validators.required],
    contaId: [null as number | null, Validators.required],
    pago: [true],
  });

  protected readonly tipo = toSignal(this.form.controls.tipo.valueChanges, {
    initialValue: this.form.controls.tipo.value,
  });
  protected readonly categoriasDoTipo = computed(() =>
    this.categorias().filter((c) => c.tipo === this.tipo()),
  );

  ngOnInit(): void {
    const transacao = this.transacao();
    if (transacao) {
      this.form.setValue({
        tipo: transacao.tipo,
        descricao: transacao.descricao,
        valor: transacao.valor,
        data: transacao.data.slice(0, 10),
        categoriaId: transacao.categoriaId,
        contaId: transacao.contaId,
        pago: transacao.pago,
      });
    } else if (this.contas().length === 1) {
      this.form.controls.contaId.setValue(this.contas()[0].contaId);
    }

    // Ao trocar entre entrada e saída, descarta a categoria que não pertence ao novo tipo.
    this.form.controls.tipo.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((tipo) => {
        const categoriaId = this.form.controls.categoriaId.value;
        const compativel = this.categorias().some(
          (c) => c.categoriaId === categoriaId && c.tipo === tipo,
        );
        if (!compativel) {
          this.form.controls.categoriaId.setValue(null);
        }
      });
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
    const dados: TransacaoInput = {
      descricao: valores.descricao.trim(),
      valor: valores.valor ?? 0,
      data: valores.data,
      tipo: valores.tipo,
      categoriaId: valores.categoriaId ?? 0,
      contaId: valores.contaId ?? 0,
      pago: valores.pago,
    };

    const existente = this.transacao();
    const requisicao: Observable<unknown> = existente
      ? this.service.atualizar(existente.transacaoId, dados)
      : this.service.criar(dados);

    this.salvando.set(true);
    this.erro.set(null);
    requisicao.pipe(finalize(() => this.salvando.set(false))).subscribe({
      next: () => this.salvo.emit(),
      error: (erro: unknown) =>
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a transação.')),
    });
  }
}
