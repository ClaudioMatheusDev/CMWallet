import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { Categoria, FinanceiroService, TipoMovimento } from '../../services/financeiro.service';

@Component({ selector: 'app-categorias', imports: [FormsModule], templateUrl: './categorias.component.html' })
export class CategoriasComponent implements OnInit {
  private readonly api = inject(FinanceiroService);
  private readonly changeDetector = inject(ChangeDetectorRef);
  categorias: Categoria[] = []; carregando = false; erro = ''; erroForm = ''; aberto = false; salvando = false; editandoId: number | null = null; tipoFiltro: number | '' = '';
  form: { nome: string; tipo: TipoMovimento } = { nome: '', tipo: 2 };
  get visiveis() { return this.tipoFiltro ? this.categorias.filter(c => c.tipo === +this.tipoFiltro) : this.categorias; }
  ngOnInit() { this.carregar(); }
  carregar() {
    this.carregando = true;
    this.erro = '';
    this.api.listarCategorias().pipe(finalize(() => {
      this.carregando = false;
      this.changeDetector.markForCheck();
    })).subscribe({
      next: x => { this.categorias = x; },
      error: e => { this.erro = e?.error?.message || 'Não foi possível carregar categorias.'; },
    });
  }
  nova() { this.editandoId = null; this.form = { nome: '', tipo: 2 }; this.erroForm = ''; this.aberto = true; }
  editar(c: Categoria) { this.editandoId = c.categoriaId; this.form = { nome: c.nome, tipo: c.tipo }; this.erroForm = ''; this.aberto = true; }
  salvar() {
    this.salvando = true;
    const req = this.editandoId ? this.api.atualizarCategoria(this.editandoId, this.form) : this.api.criarCategoria(this.form);
    req.pipe(finalize(() => {
      this.salvando = false;
      this.changeDetector.markForCheck();
    })).subscribe({
      next: () => { this.aberto = false; this.changeDetector.markForCheck(); this.carregar(); },
      error: e => { this.erroForm = e?.error?.message || e?.error?.title || 'Não foi possível salvar a categoria.'; },
    });
  }
  excluir(c: Categoria) {
    if (confirm('Excluir a categoria "' + c.nome + '"?')) {
      this.api.excluirCategoria(c.categoriaId).subscribe({
        next: () => this.carregar(),
        error: e => {
          this.erro = e?.error?.message || 'Não foi possível excluir. A categoria pode estar vinculada a transações.';
          this.changeDetector.markForCheck();
        },
      });
    }
  }
}
