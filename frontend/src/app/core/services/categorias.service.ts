import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api';
import { Categoria, CategoriaInput } from '../models';

@Injectable({ providedIn: 'root' })
export class CategoriasService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(API_URL)}/categorias`;

  listar(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.url);
  }

  criar(dados: CategoriaInput): Observable<{ categoriaId: number }> {
    return this.http.post<{ categoriaId: number }>(this.url, dados);
  }

  atualizar(id: number, dados: CategoriaInput): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, dados);
  }

  excluir(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
