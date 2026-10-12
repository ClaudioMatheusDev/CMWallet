import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService } from '../core/feedback/toast.service';
import { Icon } from './icon';

@Component({
  selector: 'app-toast-outlet',
  imports: [Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'toasts', 'aria-live': 'polite' },
  template: `
    @for (t of toast.toasts(); track t.id) {
      <div
        class="toast"
        [class.toast--erro]="t.tipo === 'erro'"
        [attr.role]="t.tipo === 'erro' ? 'alert' : 'status'"
      >
        <app-icon
          class="toast__icon"
          [nome]="t.tipo === 'erro' ? 'alerta' : 'check'"
          [tamanho]="18"
        />
        <p class="toast__text">{{ t.mensagem }}</p>
        <button
          type="button"
          class="icon-btn icon-btn--sm"
          aria-label="Fechar notificação"
          (click)="toast.remover(t.id)"
        >
          <app-icon nome="fechar" [tamanho]="14" />
        </button>
      </div>
    }
  `,
})
export class ToastOutlet {
  protected readonly toast = inject(ToastService);
}
