import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  afterRenderEffect,
  input,
  output,
  viewChild,
} from '@angular/core';
import { Icon } from './icon';

let proximoId = 0;

/**
 * Modal baseado no `<dialog>` nativo: foco preso no diálogo, `Esc` para fechar e
 * respeito ao atributo `autofocus` do conteúdo projetado.
 */
@Component({
  selector: 'app-dialog',
  imports: [Icon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <dialog
      #dialogo
      class="dialog"
      [style.width]="'min(' + largura() + 'px, calc(100vw - 32px))'"
      [attr.aria-labelledby]="tituloId"
      (close)="fechado.emit()"
      (mousedown)="pressionadoNoFundo = $event.target === dialogo"
      (click)="fecharSeClicouNoFundo($event, dialogo)"
    >
      <div class="dialog__panel">
        <header class="dialog__header">
          <h2 class="dialog__title" [id]="tituloId">{{ titulo() }}</h2>
          <button type="button" class="icon-btn" aria-label="Fechar" (click)="dialogo.close()">
            <app-icon nome="fechar" [tamanho]="18" />
          </button>
        </header>
        <ng-content />
      </div>
    </dialog>
  `,
})
export class Dialog {
  readonly aberto = input(false);
  readonly titulo = input.required<string>();
  readonly largura = input(480);
  readonly fechado = output<void>();

  protected readonly tituloId = `dialogo-titulo-${++proximoId}`;
  protected pressionadoNoFundo = false;

  private readonly elemento = viewChild.required<ElementRef<HTMLDialogElement>>('dialogo');

  constructor() {
    afterRenderEffect(() => {
      const dialogo = this.elemento().nativeElement;
      if (this.aberto() && !dialogo.open) {
        dialogo.showModal();
      } else if (!this.aberto() && dialogo.open) {
        dialogo.close();
      }
    });
  }

  /** Fecha só quando o clique começa e termina no fundo, para não fechar ao arrastar uma seleção de texto. */
  protected fecharSeClicouNoFundo(evento: MouseEvent, dialogo: HTMLDialogElement): void {
    if (this.pressionadoNoFundo && evento.target === dialogo) {
      dialogo.close();
    }
    this.pressionadoNoFundo = false;
  }
}
