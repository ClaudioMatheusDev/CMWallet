import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Icon, NomeIcone } from './icon';

@Component({
  selector: 'app-empty-state',
  imports: [Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'empty-state' },
  template: `
    <span class="empty-state__icon"><app-icon [nome]="icone()" [tamanho]="20" /></span>
    <p class="empty-state__title">{{ titulo() }}</p>
    @if (descricao()) {
      <p class="empty-state__description">{{ descricao() }}</p>
    }
    <div class="empty-state__actions">
      <ng-content />
    </div>
  `,
})
export class EmptyState {
  readonly icone = input.required<NomeIcone>();
  readonly titulo = input.required<string>();
  readonly descricao = input<string>();
}
