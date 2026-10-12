import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { ConfirmDialog } from './shared/confirm-dialog';
import { Icon, NomeIcone } from './shared/icon';
import { ToastOutlet } from './shared/toast-outlet';

interface ItemNavegacao {
  rota: string;
  rotulo: string;
  icone: NomeIcone;
}

interface GrupoNavegacao {
  titulo?: string;
  itens: ItemNavegacao[];
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Icon, ToastOutlet, ConfirmDialog],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '(document:keydown.escape)': 'menuAberto.set(false)' },
})
export class App {
  protected readonly menuAberto = signal(false);

  protected readonly navegacao: readonly GrupoNavegacao[] = [
    {
      itens: [
        { rota: '/', rotulo: 'Visão geral', icone: 'visao-geral' },
        { rota: '/transacoes', rotulo: 'Transações', icone: 'transacoes' },
        { rota: '/metas', rotulo: 'Metas', icone: 'metas' },
      ],
    },
    {
      titulo: 'Cadastros',
      itens: [
        { rota: '/contas', rotulo: 'Contas', icone: 'contas' },
        { rota: '/categorias', rotulo: 'Categorias', icone: 'categorias' },
      ],
    },
  ];

  constructor() {
    inject(Router)
      .events.pipe(
        filter((evento) => evento instanceof NavigationEnd),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.menuAberto.set(false));
  }

  protected alternarMenu(): void {
    this.menuAberto.update((aberto) => !aberto);
  }
}
