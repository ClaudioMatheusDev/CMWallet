import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

const API = 'https://localhost:7049/api';
export type TipoMovimento = 1 | 2;
export interface Categoria { categoriaId: number; nome: string; tipo: TipoMovimento; }
export interface CategoriaInput { nome: string; tipo: TipoMovimento; }
export interface Transacao { transacaoId: number; descricao: string; valor: number; data: string; tipo: TipoMovimento; categoriaId: number; categoriaNome: string; contaId: number; contaNome: string; pago: boolean; }
export interface TransacaoInput { descricao: string; valor: number; data: string; tipo: TipoMovimento; categoriaId: number; contaId: number; pago: boolean; }
export interface PaginaTransacoes { itens: Transacao[]; pagina: number; tamanhoPagina: number; total: number; totalPaginas: number; }
export interface FiltroTransacoes { ContaId?: number; CategoriaId?: number; Tipo?: number; DataInicio?: string; DataFim?: string; Pagina: number; TamanhoPagina: number; }
export interface Meta { metaId: number; valorMeta: number; valorAtual: number; dataMeta: string; contaId: number; }
export interface MetaInput { valorMeta: number; valorAtual: number; dataMeta: string; contaId: number; }

@Injectable({ providedIn: 'root' })
export class FinanceiroService {
  private readonly http = inject(HttpClient);
  listarCategorias(): Observable<Categoria[]> { return this.http.get<Categoria[]>(`${API}/categorias`); }
  criarCategoria(input: CategoriaInput): Observable<unknown> { return this.http.post(`${API}/categorias`, input); }
  atualizarCategoria(id: number, input: CategoriaInput): Observable<void> { return this.http.put<void>(`${API}/categorias/${id}`, input); }
  excluirCategoria(id: number): Observable<void> { return this.http.delete<void>(`${API}/categorias/${id}`); }
  listarTransacoes(filtro: FiltroTransacoes): Observable<PaginaTransacoes> {
    let params = new HttpParams();
    Object.entries(filtro).forEach(([key, value]) => { if (value !== undefined && value !== '') params = params.set(key, value); });
    return this.http.get<PaginaTransacoes>(`${API}/transacoes`, { params });
  }
  criarTransacao(input: TransacaoInput): Observable<unknown> { return this.http.post(`${API}/transacoes`, input); }
  atualizarTransacao(id: number, input: TransacaoInput): Observable<void> { return this.http.put<void>(`${API}/transacoes/${id}`, input); }
  excluirTransacao(id: number): Observable<void> { return this.http.delete<void>(`${API}/transacoes/${id}`); }
  listarMetas(): Observable<Meta[]> { return this.http.get<Meta[]>(`${API}/metas`); }
  criarMeta(input: MetaInput): Observable<unknown> { return this.http.post(`${API}/metas`, input); }
  atualizarMeta(id: number, input: MetaInput): Observable<void> { return this.http.put<void>(`${API}/metas/${id}`, input); }
  excluirMeta(id: number): Observable<void> { return this.http.delete<void>(`${API}/metas/${id}`); }
}
