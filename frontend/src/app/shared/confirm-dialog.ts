import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ConfirmacaoService } from '../core/feedback/confirmacao.service';
import { Dialog } from './dialog';

@Component({
  selector: 'app-confirm-dialog',
  imports: [Dialog],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-dialog
      [aberto]="pedido() !== null"
      [titulo]="pedido()?.titulo ?? ''"
      [largura]="420"
      (fechado)="confirmacao.responder(false)"
    >
      @if (pedido(); as p) {
        <div class="dialog__body">
          <p class="dialog__text">{{ p.mensagem }}</p>
        </div>
        <footer class="dialog__footer">
          <button
            type="button"
            class="btn btn--secondary"
            autofocus
            (click)="confirmacao.responder(false)"
          >
            Cancelar
          </button>
          <button
            type="button"
            class="btn"
            [class.btn--danger]="p.destrutiva"
            [class.btn--primary]="!p.destrutiva"
            (click)="confirmacao.responder(true)"
          >
            {{ p.confirmar }}
          </button>
        </footer>
      }
    </app-dialog>
  `,
})
export class ConfirmDialog {
  protected readonly confirmacao = inject(ConfirmacaoService);
  protected readonly pedido = this.confirmacao.pedido;
}
