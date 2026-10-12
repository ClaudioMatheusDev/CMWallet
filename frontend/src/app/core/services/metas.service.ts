import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api';
import { Meta, MetaInput } from '../models';

@Injectable({ providedIn: 'root' })
export class MetasService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/metas`;

  listar(): Observable<Meta[]> {
    return this.http.get<Meta[]>(this.url);
  }

  criar(dados: MetaInput): Observable<{ metaId: number }> {
    return this.http.post<{ metaId: number }>(this.url, dados);
  }

  atualizar(id: number, dados: MetaInput): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, dados);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
