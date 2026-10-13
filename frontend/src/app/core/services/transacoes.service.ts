import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api';
import { FiltroTransacoes, Pagina, Transacao, TransacaoInput } from '../models';

@Injectable({ providedIn: 'root' })
export class TransacoesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/transacoes`;

  listar(filtro: FiltroTransacoes): Observable<Pagina<Transacao>> {
    return this.http.get<Pagina<Transacao>>(this.url, { params: paraParametros(filtro) });
  }

  criar(dados: TransacaoInput): Observable<{ transacaoId: number }> {
    return this.http.post<{ transacaoId: number }>(this.url, dados);
  }

  atualizar(id: number, dados: TransacaoInput): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, dados);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}

function paraParametros(filtro: FiltroTransacoes): HttpParams {
  let params = new HttpParams()
    .set('pagina', filtro.pagina)
    .set('tamanhoPagina', filtro.tamanhoPagina);

  if (filtro.contaId != null) params = params.set('contaId', filtro.contaId);
  if (filtro.categoriaId != null) params = params.set('categoriaId', filtro.categoriaId);
  if (filtro.tipo != null) params = params.set('tipo', filtro.tipo);
  if (filtro.dataInicio) params = params.set('dataInicio', filtro.dataInicio);
  if (filtro.dataFim) params = params.set('dataFim', filtro.dataFim);

  return params;
}
