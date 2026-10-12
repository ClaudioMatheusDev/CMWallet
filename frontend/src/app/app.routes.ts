import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    title: 'Visão geral · CMWallet',
    loadComponent: () => import('./features/visao-geral/visao-geral').then((m) => m.VisaoGeral),
  },
  {
    path: 'transacoes',
    title: 'Transações · CMWallet',
    loadComponent: () =>
      import('./features/transacoes/transacoes-page').then((m) => m.TransacoesPage),
  },
  {
    path: 'metas',
    title: 'Metas · CMWallet',
    loadComponent: () => import('./features/metas/metas-page').then((m) => m.MetasPage),
  },
  {
    path: 'contas',
    title: 'Contas · CMWallet',
    loadComponent: () => import('./features/contas/contas-page').then((m) => m.ContasPage),
  },
  {
    path: 'categorias',
    title: 'Categorias · CMWallet',
    loadComponent: () =>
      import('./features/categorias/categorias-page').then((m) => m.CategoriasPage),
  },
  { path: '**', redirectTo: '' },
];
