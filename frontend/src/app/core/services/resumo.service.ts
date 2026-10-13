import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api';
import { FiltroResumo, Resumo } from '../models';

@Injectable({ providedIn: 'root' })
export class ResumoService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/resumo`;

  obter(filtro: FiltroResumo = {}): Observable<Resumo> {
    let params = new HttpParams();
    if (filtro.dataInicio) params = params.set('dataInicio', filtro.dataInicio);
    if (filtro.dataFim) params = params.set('dataFim', filtro.dataFim);
    if (filtro.contaId != null) params = params.set('contaId', filtro.contaId);

    return this.http.get<Resumo>(this.url, { params });
  }
}
