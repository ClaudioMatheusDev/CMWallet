import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { EMPTY, Observable, expand, reduce } from 'rxjs';
import { API_URL } from '../api';
import {
  FiltroTransacoes,
  Pagina,
  TAMANHO_PAGINA_MAXIMO,
  Transacao,
  TransacaoInput,
} from '../models';

@Injectable({ providedIn: 'root' })
export class TransacoesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/transacoes`;

  listar(filtro: FiltroTransacoes): Observable<Pagina<Transacao>> {
    return this.http.get<Pagina<Transacao>>(this.url, { params: paraParametros(filtro) });
  }

  /**
   * Percorre todas as páginas que atendem ao filtro. Indicado para períodos curtos
   * (como o mês corrente), enquanto a API não oferece um endpoint de totais.
   */
  listarTodas(filtro: Omit<FiltroTransacoes, 'pagina' | 'tamanhoPagina'>): Observable<Transacao[]> {
    const buscar = (pagina: number) =>
      this.listar({ ...filtro, pagina, tamanhoPagina: TAMANHO_PAGINA_MAXIMO });

    return buscar(1).pipe(
      expand((p) => (p.pagina < p.totalPaginas ? buscar(p.pagina + 1) : EMPTY)),
      reduce((todas: Transacao[], p) => todas.concat(p.itens), []),
    );
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
  // A API compara com `<=`; estender até o fim do dia torna a data final inclusiva.
  if (filtro.dataFim) params = params.set('dataFim', `${filtro.dataFim}T23:59:59.9999999`);

  return params;
}
