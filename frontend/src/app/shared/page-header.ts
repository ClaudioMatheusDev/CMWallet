import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'page-header' },
  template: `
    <div class="page-header__text">
      <h1 class="page-header__title">{{ titulo() }}</h1>
      @if (descricao()) {
        <p class="page-header__description">{{ descricao() }}</p>
      }
    </div>
    <div class="page-header__actions">
      <ng-content />
    </div>
  `,
})
export class PageHeader {
  readonly titulo = input.required<string>();
  readonly descricao = input<string>();
}
