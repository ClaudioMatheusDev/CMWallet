import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api';
import { Conta, ContaInput } from '../models';

@Injectable({ providedIn: 'root' })
export class ContasService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/contas`;

  listar(): Observable<Conta[]> {
    return this.http.get<Conta[]>(this.url);
  }

  criar(dados: ContaInput): Observable<{ contaId: number }> {
    return this.http.post<{ contaId: number }>(this.url, dados);
  }

  atualizar(id: number, dados: ContaInput): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, dados);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
