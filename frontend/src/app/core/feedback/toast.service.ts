import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  tipo: 'sucesso' | 'erro';
  mensagem: string;
}

const MAXIMO_VISIVEL = 3;

@Injectable({ providedIn: 'root' })
export class ToastService {
  private sequencia = 0;
  private readonly _toasts = signal<Toast[]>([]);
  readonly toasts = this._toasts.asReadonly();

  sucesso(mensagem: string): void {
    this.mostrar('sucesso', mensagem, 3500);
  }

  erro(mensagem: string): void {
    this.mostrar('erro', mensagem, 6000);
  }

  remover(id: number): void {
    this._toasts.update((lista) => lista.filter((t) => t.id !== id));
  }

  private mostrar(tipo: Toast['tipo'], mensagem: string, duracaoMs: number): void {
    const id = ++this.sequencia;
    this._toasts.update((lista) => [...lista.slice(-(MAXIMO_VISIVEL - 1)), { id, tipo, mensagem }]);
    setTimeout(() => this.remover(id), duracaoMs);
  }
}
