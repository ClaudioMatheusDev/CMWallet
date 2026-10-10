import { Routes } from '@angular/router';
import { ContasComponent } from './pages/Contas/contas.component';
import { TransacoesComponent } from './pages/Transacoes/transacoes.component';
import { CategoriasComponent } from './pages/Categorias/categorias.component';
import { MetasComponent } from './pages/Metas/metas.component';

export const routes: Routes = [
  { path: '', component: ContasComponent, title: 'Visão geral | CMWallet' },
  { path: 'contas', component: ContasComponent, title: 'Contas | CMWallet' },
  { path: 'transacoes', component: TransacoesComponent, title: 'Transações | CMWallet' },
  { path: 'categorias', component: CategoriasComponent, title: 'Categorias | CMWallet' },
  { path: 'metas', component: MetasComponent, title: 'Metas financeiras | CMWallet' },
  { path: '**', redirectTo: '' },
];
