import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ConfirmacaoService } from '../../core/feedback/confirmacao.service';
import { ToastService } from '../../core/feedback/toast.service';
import { mensagemDeErro } from '../../core/http-error';
import { Categoria, TipoMovimento } from '../../core/models';
import { CategoriasService } from '../../core/services/categorias.service';
import { Dialog } from '../../shared/dialog';
import { Icon } from '../../shared/icon';
import { PageHeader } from '../../shared/page-header';
import { SkeletonList } from '../../shared/skeleton-list';
import { CategoriaForm } from './categoria-form';

interface EdicaoCategoria {
  categoria: Categoria | null;
  tipo: TipoMovimento;
}

@Component({
  selector: 'app-categorias-page',
  imports: [PageHeader, SkeletonList, Icon, Dialog, CategoriaForm],
  templateUrl: './categorias-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoriasPage {
  private readonly service = inject(CategoriasService);
  private readonly toast = inject(ToastService);
  private readonly confirmacao = inject(ConfirmacaoService);

  protected readonly TipoMovimento = TipoMovimento;

  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly grupos = computed(() => [
    {
      tipo: TipoMovimento.Entrada,
      titulo: 'Entradas',
      descricao: 'Salário, rendimentos, reembolsos.',
      itens: this.categorias().filter((c) => c.tipo === TipoMovimento.Entrada),
    },
    {
      tipo: TipoMovimento.Saida,
      titulo: 'Saídas',
      descricao: 'Moradia, alimentação, transporte.',
      itens: this.categorias().filter((c) => c.tipo === TipoMovimento.Saida),
    },
  ]);

  protected readonly emEdicao = signal<EdicaoCategoria | null>(null);
  protected readonly tituloFormulario = computed(() =>
    this.emEdicao()?.categoria ? 'Editar categoria' : 'Nova categoria',
  );

  constructor() {
    this.carregar();
  }

  protected carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);
    this.service.listar().subscribe({
      next: (categorias) => {
        this.categorias.set(categorias);
        this.carregando.set(false);
      },
      error: (erro: unknown) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível carregar as categorias.'));
        this.carregando.set(false);
      },
    });
  }

  protected novaCategoria(tipo: TipoMovimento = TipoMovimento.Saida): void {
    this.emEdicao.set({ categoria: null, tipo });
  }

  protected editar(categoria: Categoria): void {
    this.emEdicao.set({ categoria, tipo: categoria.tipo });
  }

  protected fecharFormulario(): void {
    this.emEdicao.set(null);
  }

  protected aoSalvar(): void {
    const nova = !this.emEdicao()?.categoria;
    this.fecharFormulario();
    this.toast.sucesso(nova ? 'Categoria criada.' : 'Categoria atualizada.');
    this.carregar();
  }

  protected async excluir(categoria: Categoria): Promise<void> {
    const confirmado = await this.confirmacao.confirmar({
      titulo: 'Excluir categoria',
      mensagem: `A categoria "${categoria.nome}" será excluída. Categorias usadas em transações não podem ser excluídas.`,
      confirmar: 'Excluir',
      destrutiva: true,
    });
    if (!confirmado) return;

    this.service.excluir(categoria.categoriaId).subscribe({
      next: () => {
        this.toast.sucesso('Categoria excluída.');
        this.carregar();
      },
      error: (erro: unknown) =>
        this.toast.erro(
          mensagemDeErro(erro, 'Não foi possível excluir a categoria.'),
        ),
    });
  }
}
