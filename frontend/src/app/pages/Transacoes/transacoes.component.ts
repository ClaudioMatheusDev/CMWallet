import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { ContaService } from '../../services/conta.service';
import { Conta } from '../../models/conta';
import { Categoria, FinanceiroService, Transacao, TransacaoInput } from '../../services/financeiro.service';

@Component({ selector: 'app-transacoes', imports: [CurrencyPipe, DatePipe, FormsModule], templateUrl: './transacoes.component.html' })
export class TransacoesComponent implements OnInit {
  private readonly api = inject(FinanceiroService);
  private readonly contaApi = inject(ContaService);
  private readonly changeDetector = inject(ChangeDetectorRef);
  itens: Transacao[] = []; contas: Conta[] = []; categorias: Categoria[] = [];
  carregando = false; erro = ''; erroForm = ''; salvando = false; formAberto = false; editandoId: number | null = null;
  total = 0; pagina = 1; tamanhoPagina = 20; totalPaginas = 0;
  filtroConta = ''; filtroCategoria = ''; filtroTipo: number | '' = ''; dataInicio = ''; dataFim = '';
  form: TransacaoInput = { descricao: '', valor: 0, data: this.hoje(), tipo: 2, categoriaId: 0, contaId: 0, pago: false };
  get categoriasFiltradas() { return this.categorias.filter(c => c.tipo === this.form.tipo); }
  get totalEntradas() { return this.itens.filter(t => t.tipo === 1).reduce((n, t) => n + t.valor, 0); }
  get totalSaidas() { return this.itens.filter(t => t.tipo === 2).reduce((n, t) => n + t.valor, 0); }
  ngOnInit() {
    this.contaApi.listarContas().subscribe({
      next: x => { this.contas = x; this.changeDetector.markForCheck(); },
      error: () => { this.erro = 'Não foi possível carregar as contas.'; this.changeDetector.markForCheck(); }
    });
    this.api.listarCategorias().subscribe({
      next: x => { this.categorias = x; this.changeDetector.markForCheck(); },
      error: () => { this.erro = 'Não foi possível carregar as categorias.'; this.changeDetector.markForCheck(); }
    });
    this.carregar();
  }
  carregar() {
    this.carregando = true; this.erro = '';
    this.api.listarTransacoes({ Pagina: this.pagina, TamanhoPagina: this.tamanhoPagina, ContaId: this.filtroConta ? +this.filtroConta : undefined, CategoriaId: this.filtroCategoria ? +this.filtroCategoria : undefined, Tipo: this.filtroTipo || undefined, DataInicio: this.dataInicio || undefined, DataFim: this.dataFim ? `${this.dataFim}T23:59:59.999` : undefined })
      .pipe(finalize(() => {
        this.carregando = false;
        this.changeDetector.markForCheck();
      }))
      .subscribe({
        next: r => { this.itens = r.itens; this.total = r.total; this.totalPaginas = r.totalPaginas; },
        error: e => { this.erro = this.msg(e, 'Não foi possível carregar as transações.'); }
      });
  }
  filtrar() { this.pagina = 1; this.carregar(); }
  limparFiltros() { this.filtroConta = ''; this.filtroCategoria = ''; this.filtroTipo = ''; this.dataInicio = ''; this.dataFim = ''; this.filtrar(); }
  mudarPagina(delta: number) { this.pagina += delta; this.carregar(); }
  nova() {
    if (!this.contas.length || !this.categorias.length) { this.erro = 'Cadastre ao menos uma conta e uma categoria antes de criar uma transação.'; return; }
    this.editandoId = null; this.form = { descricao: '', valor: 0, data: this.hoje(), tipo: 2, categoriaId: 0, contaId: this.contas[0].contaId, pago: false }; this.ajustarCategorias(); this.erroForm = ''; this.formAberto = true;
  }
  editar(t: Transacao) { this.editandoId = t.transacaoId; this.form = { descricao: t.descricao, valor: t.valor, data: t.data.slice(0, 10), tipo: t.tipo, categoriaId: t.categoriaId, contaId: t.contaId, pago: t.pago }; this.erroForm = ''; this.formAberto = true; }
  ajustarCategorias() { if (!this.categoriasFiltradas.some(c => c.categoriaId === this.form.categoriaId)) this.form.categoriaId = this.categoriasFiltradas[0]?.categoriaId ?? 0; }
  salvar() {
    this.salvando = true; const op = this.editandoId ? this.api.atualizarTransacao(this.editandoId, this.form) : this.api.criarTransacao(this.form);
    op.subscribe({ next: () => { this.formAberto = false; this.salvando = false; this.changeDetector.markForCheck(); this.carregar(); }, error: e => { this.erroForm = this.msg(e, 'Não foi possível salvar a transação.'); this.salvando = false; this.changeDetector.markForCheck(); } });
  }
  excluir(t: Transacao) { if (confirm('Excluir a transação "' + t.descricao + '"?')) this.api.excluirTransacao(t.transacaoId).subscribe({ next: () => this.carregar(), error: e => { this.erro = this.msg(e, 'Não foi possível excluir a transação.'); this.changeDetector.markForCheck(); } }); }
  private hoje() { return new Date().toISOString().slice(0, 10); }
  private msg(e: any, fallback: string) { return e?.error?.message || e?.error?.title || fallback; }
}
