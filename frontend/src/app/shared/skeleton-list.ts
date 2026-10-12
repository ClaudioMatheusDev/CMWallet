import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

/** Placeholder de carregamento para listas e tabelas. */
@Component({
  selector: 'app-skeleton-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'skeleton-list', 'aria-busy': 'true', 'aria-label': 'Carregando' },
  template: `
    @for (linha of linhas(); track linha) {
      <div class="skeleton-list__row">
        <span class="skeleton" [style.width.%]="18"></span>
        <span class="skeleton" [style.width.%]="30 + (linha % 3) * 8"></span>
        <span class="skeleton skeleton-list__end" [style.width.%]="12"></span>
      </div>
    }
  `,
})
export class SkeletonList {
  readonly quantidade = input(5);
  protected readonly linhas = computed(() => Array.from({ length: this.quantidade() }, (_, i) => i));
}
