import { TipoConta } from './models';

export const TIPOS_CONTA: readonly { valor: TipoConta; rotulo: string }[] = [
  { valor: TipoConta.ContaCorrente, rotulo: 'Conta corrente' },
  { valor: TipoConta.ContaPoupanca, rotulo: 'Poupança' },
  { valor: TipoConta.Dinheiro, rotulo: 'Dinheiro em espécie' },
];

export function rotuloTipoConta(tipo: TipoConta): string {
  return TIPOS_CONTA.find((t) => t.valor === tipo)?.rotulo ?? 'Conta';
}

export function soma(valores: readonly number[]): number {
  return valores.reduce((total, valor) => total + valor, 0);
}
