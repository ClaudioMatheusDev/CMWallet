export enum TipoConta {
  ContaCorrente = 1,
  ContaPoupanca = 2,
  Dinheiro = 3,
}

export enum TipoMovimento {
  Entrada = 1,
  Saida = 2,
}

export interface Conta {
  contaId: number;
  nome: string;
  saldoInicial: number;
  tipoConta: TipoConta;
  dataCriacao: string;
  dataAtualizacao: string;
}

export interface ContaInput {
  nome: string;
  saldoInicial: number;
  tipoConta: TipoConta;
}

export interface Categoria {
  categoriaId: number;
  nome: string;
  tipo: TipoMovimento;
}

export interface CategoriaInput {
  nome: string;
  tipo: TipoMovimento;
}

export interface Transacao {
  transacaoId: number;
  descricao: string;
  valor: number;
  data: string;
  tipo: TipoMovimento;
  categoriaId: number;
  categoriaNome: string;
  contaId: number;
  contaNome: string;
  pago: boolean;
}

export interface TransacaoInput {
  descricao: string;
  valor: number;
  /** Data no formato `yyyy-MM-dd`. */
  data: string;
  tipo: TipoMovimento;
  categoriaId: number;
  contaId: number;
  pago: boolean;
}

/** Datas no formato `yyyy-MM-dd`; `dataFim` é inclusiva. */
export interface FiltroTransacoes {
  contaId?: number;
  categoriaId?: number;
  tipo?: TipoMovimento;
  dataInicio?: string;
  dataFim?: string;
  pagina: number;
  tamanhoPagina: number;
}

/** Limite definido pela API (`TransacaoFiltroDto.TamanhoPaginaMaximo`). */
export const TAMANHO_PAGINA_MAXIMO = 100;

export interface Pagina<T> {
  itens: T[];
  pagina: number;
  tamanhoPagina: number;
  total: number;
  totalPaginas: number;
}

export interface Meta {
  metaId: number;
  valorMeta: number;
  valorAtual: number;
  dataMeta: string;
  contaId: number;
}

export interface MetaInput {
  valorMeta: number;
  valorAtual: number;
  /** Data no formato `yyyy-MM-dd`. */
  dataMeta: string;
  contaId: number;
}
