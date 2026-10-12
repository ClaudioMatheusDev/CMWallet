import { Injectable, signal } from '@angular/core';

export interface OpcoesConfirmacao {
  titulo: string;
  mensagem: string;
  /** Texto do botão de confirmação. */
  confirmar?: string;
  /** Destaca a ação como destrutiva (botão vermelho). */
  destrutiva?: boolean;
}

export interface PedidoConfirmacao extends Required<OpcoesConfirmacao> {
  resolver: (confirmado: boolean) => void;
}

/** Substitui `window.confirm` por um diálogo da aplicação. Renderizado uma vez no shell. */
@Injectable({ providedIn: 'root' })
export class ConfirmacaoService {
  private readonly _pedido = signal<PedidoConfirmacao | null>(null);
  readonly pedido = this._pedido.asReadonly();

  confirmar(opcoes: OpcoesConfirmacao): Promise<boolean> {
    this.responder(false);
    return new Promise((resolver) =>
      this._pedido.set({ confirmar: 'Confirmar', destrutiva: false, ...opcoes, resolver }),
    );
  }

  responder(confirmado: boolean): void {
    const pedido = this._pedido();
    if (!pedido) return;
    this._pedido.set(null);
    pedido.resolver(confirmado);
  }
}
