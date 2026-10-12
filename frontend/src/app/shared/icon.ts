import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

/** Traços em grade 24×24, no estilo de ícones lineares (stroke 1.75). */
const ICONES = {
  'visao-geral': ['M4 4h6v8H4z', 'M14 4h6v5h-6z', 'M14 13h6v7h-6z', 'M4 16h6v4H4z'],
  transacoes: ['M7 4 3 8l4 4', 'M3 8h18', 'm17 20 4-4-4-4', 'M21 16H3'],
  contas: [
    'M19 7V4a1 1 0 0 0-1-1H5a2 2 0 0 0 0 4h15a1 1 0 0 1 1 1v4h-3a2 2 0 0 0 0 4h3a1 1 0 0 0 1-1v-2a1 1 0 0 0-1-1',
    'M3 5v14a2 2 0 0 0 2 2h15a1 1 0 0 0 1-1v-4',
  ],
  categorias: [
    'M12.586 2.586A2 2 0 0 0 11.172 2H4a2 2 0 0 0-2 2v7.172a2 2 0 0 0 .586 1.414l8.704 8.704a2.426 2.426 0 0 0 3.42 0l6.58-6.58a2.426 2.426 0 0 0 0-3.42z',
    'M7.5 7.5h.01',
  ],
  metas: [
    'M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0z',
    'M18 12a6 6 0 1 1-12 0 6 6 0 0 1 12 0z',
    'M14 12a2 2 0 1 1-4 0 2 2 0 0 1 4 0z',
  ],
  plus: ['M12 5v14', 'M5 12h14'],
  editar: [
    'M21.174 6.812a1 1 0 0 0-3.986-3.987L3.842 16.174a2 2 0 0 0-.5.83l-1.321 4.352a.5.5 0 0 0 .623.622l4.353-1.32a2 2 0 0 0 .83-.497z',
    'm15 5 4 4',
  ],
  excluir: [
    'M3 6h18',
    'M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6',
    'M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2',
    'M10 11v6',
    'M14 11v6',
  ],
  fechar: ['M18 6 6 18', 'm6 6 12 12'],
  menu: ['M4 6h16', 'M4 12h16', 'M4 18h16'],
  anterior: ['m15 18-6-6 6-6'],
  proximo: ['m9 18 6-6-6-6'],
  'seta-direita': ['M5 12h14', 'm12 5 7 7-7 7'],
  alerta: ['M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0z', 'M12 8v4', 'M12 16h.01'],
  check: ['M20 6 9 17l-5-5'],
  entrada: ['M17 7 7 17', 'M17 17H7V7'],
  saida: ['M7 17 17 7', 'M7 7h10v10'],
} as const satisfies Record<string, readonly string[]>;

export type NomeIcone = keyof typeof ICONES;

@Component({
  selector: 'app-icon',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'icon' },
  template: `
    <svg
      viewBox="0 0 24 24"
      [attr.width]="tamanho()"
      [attr.height]="tamanho()"
      fill="none"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
      stroke-linejoin="round"
      aria-hidden="true"
      focusable="false"
    >
      @for (d of caminhos(); track $index) {
        <svg:path [attr.d]="d" />
      }
    </svg>
  `,
})
export class Icon {
  readonly nome = input.required<NomeIcone>();
  readonly tamanho = input(16);

  protected readonly caminhos = computed<readonly string[]>(() => ICONES[this.nome()]);
}
