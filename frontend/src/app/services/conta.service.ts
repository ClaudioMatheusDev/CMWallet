
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Conta } from '../models/conta';

@Injectable({
  providedIn: 'root',
})
export class ContaService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7049/api/contas';

  listarContas(): Observable<Conta[]> {
    return this.http.get<Conta[]>(this.apiUrl);
  }
}
