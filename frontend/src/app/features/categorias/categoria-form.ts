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
import { Categoria, CategoriaInput, TipoMovimento } from '../../core/models';
import { CategoriasService } from '../../core/services/categorias.service';
import { textoObrigatorio } from '../../core/validadores';
import { Icon } from '../../shared/icon';

@Component({
  selector: 'app-categoria-form',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './categoria-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoriaForm implements OnInit {
  readonly categoria = input<Categoria | null>(null);
  /** Tipo pré-selecionado ao criar uma categoria. */
  readonly tipoInicial = input<TipoMovimento>(TipoMovimento.Saida);
  readonly salvo = output<void>();
  readonly cancelado = output<void>();

  private readonly service = inject(CategoriasService);

  protected readonly TipoMovimento = TipoMovimento;
  protected readonly salvando = signal(false);
  protected readonly enviado = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = inject(FormBuilder).nonNullable.group({
    nome: ['', [textoObrigatorio, Validators.maxLength(200)]],
    tipo: [TipoMovimento.Saida as TipoMovimento],
  });

  ngOnInit(): void {
    const categoria = this.categoria();
    this.form.setValue(
      categoria
        ? { nome: categoria.nome, tipo: categoria.tipo }
        : { nome: '', tipo: this.tipoInicial() },
    );
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
    const dados: CategoriaInput = { nome: valores.nome.trim(), tipo: valores.tipo };

    const existente = this.categoria();
    const requisicao: Observable<unknown> = existente
      ? this.service.atualizar(existente.categoriaId, dados)
      : this.service.criar(dados);

    this.salvando.set(true);
    this.erro.set(null);
    requisicao.pipe(finalize(() => this.salvando.set(false))).subscribe({
      next: () => this.salvo.emit(),
      error: (erro: unknown) =>
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a categoria.')),
    });
  }
}
