
export enum TipoConta {
  ContaCorrente = 1,
  ContaPoupanca = 2,
  Dinheiro = 3,
}

export interface Conta {
  contaId: number;
  nome: string;
  saldoInicial: number;
  dataCriacao: string;
  dataAtualizacao: string;
  tipoConta: TipoConta;
}
