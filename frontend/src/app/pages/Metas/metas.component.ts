import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { ContaService } from '../../services/conta.service';
import { Conta } from '../../models/conta';
import { FinanceiroService, Meta, MetaInput } from '../../services/financeiro.service';

@Component({ selector: 'app-metas', imports: [CurrencyPipe, DatePipe, DecimalPipe, FormsModule], templateUrl: './metas.component.html' })
export class MetasComponent implements OnInit {
  private readonly api = inject(FinanceiroService); private readonly contaApi = inject(ContaService);
  private readonly changeDetector = inject(ChangeDetectorRef);
  metas: Meta[] = []; contas: Conta[] = []; carregando = false; erro = ''; erroForm = ''; aberto = false; salvando = false; editandoId: number | null = null;
  form: MetaInput = { valorMeta: 0, valorAtual: 0, dataMeta: this.dataFutura(), contaId: 0 };
  get totalGuardado() { return this.metas.reduce((n, m) => n + m.valorAtual, 0); }
  get totalAlvo() { return this.metas.reduce((n, m) => n + m.valorMeta, 0); }
  ngOnInit() {
    this.contaApi.listarContas().subscribe({
      next: x => { this.contas = x; this.changeDetector.markForCheck(); },
      error: () => { this.erro = 'Não foi possível carregar as contas.'; this.changeDetector.markForCheck(); },
    });
    this.carregar();
  }
  carregar() {
    this.carregando = true;
    this.erro = '';
    this.api.listarMetas().pipe(finalize(() => {
      this.carregando = false;
      this.changeDetector.markForCheck();
    })).subscribe({
      next: x => { this.metas = x; },
      error: e => { this.erro = e?.error?.message || 'Não foi possível carregar metas.'; },
    });
  }
  nova() { if (!this.contas.length) { this.erro = 'Cadastre uma conta antes de criar uma meta.'; return; } this.editandoId = null; this.form = { valorMeta: 0, valorAtual: 0, dataMeta: this.dataFutura(), contaId: this.contas[0].contaId }; this.erroForm = ''; this.aberto = true; }
  editar(m: Meta) { this.editandoId = m.metaId; this.form = { valorMeta: m.valorMeta, valorAtual: m.valorAtual, dataMeta: m.dataMeta.slice(0, 10), contaId: m.contaId }; this.erroForm = ''; this.aberto = true; }
  progresso(m: Meta) { return m.valorMeta > 0 ? Math.min(100, Math.max(0, m.valorAtual / m.valorMeta * 100)) : 0; }
  contaNome(id: number) { return this.contas.find(c => c.contaId === id)?.nome || 'Conta #' + id; }
  salvar() {
    this.salvando = true;
    const req = this.editandoId ? this.api.atualizarMeta(this.editandoId, this.form) : this.api.criarMeta(this.form);
    req.pipe(finalize(() => {
      this.salvando = false;
      this.changeDetector.markForCheck();
    })).subscribe({
      next: () => { this.aberto = false; this.changeDetector.markForCheck(); this.carregar(); },
      error: e => { this.erroForm = e?.error?.message || e?.error?.title || 'Não foi possível salvar a meta.'; },
    });
  }
  excluir(m: Meta) {
    if (confirm('Excluir esta meta financeira?')) {
      this.api.excluirMeta(m.metaId).subscribe({
        next: () => this.carregar(),
        error: e => {
          this.erro = e?.error?.message || 'Não foi possível excluir a meta.';
          this.changeDetector.markForCheck();
        },
      });
    }
  }
  private dataFutura() { const d = new Date(); d.setMonth(d.getMonth() + 6); return d.toISOString().slice(0, 10); }
}
