
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Conta } from '../models/conta';

export interface ContaInput { nome: string; saldoInicial: number; tipoConta: number; }

@Injectable({
  providedIn: 'root',
})
export class ContaService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7049/api/contas';

  listarContas(): Observable<Conta[]> {
    return this.http.get<Conta[]>(this.apiUrl);
  }

  criarConta(dados: ContaInput): Observable<void> { return this.http.post<void>(this.apiUrl, dados); }
  atualizarConta(id: number, dados: ContaInput): Observable<void> { return this.http.put<void>(`${this.apiUrl}/${id}`, dados); }
  excluirConta(id: number): Observable<void> { return this.http.delete<void>(`${this.apiUrl}/${id}`); }
}
