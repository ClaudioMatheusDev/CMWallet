const MS_POR_DIA = 86_400_000;

const doisDigitos = (n: number) => String(n).padStart(2, '0');

/** Formata no fuso local (`toISOString` usaria UTC e adiantaria o dia à noite no Brasil). */
export function paraDataISO(data: Date): string {
  return `${data.getFullYear()}-${doisDigitos(data.getMonth() + 1)}-${doisDigitos(data.getDate())}`;
}

export function hojeISO(): string {
  return paraDataISO(new Date());
}

export function somarMeses(data: Date, meses: number): Date {
  const resultado = new Date(data);
  resultado.setMonth(resultado.getMonth() + meses);
  return resultado;
}

export function intervaloDoMes(referencia: Date): { inicio: string; fim: string } {
  const ano = referencia.getFullYear();
  const mes = referencia.getMonth();
  return {
    inicio: paraDataISO(new Date(ano, mes, 1)),
    fim: paraDataISO(new Date(ano, mes + 1, 0)),
  };
}

/** Interpreta apenas a parte de data (`yyyy-MM-dd`) de um valor vindo da API, no fuso local. */
export function lerData(valor: string): Date {
  const [ano, mes, dia] = valor.slice(0, 10).split('-').map(Number);
  return new Date(ano, mes - 1, dia);
}

/** Dias corridos de `hoje` até `data`; negativo quando a data já passou. */
export function diasAte(data: string, hoje = new Date()): number {
  const inicio = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate());
  return Math.round((lerData(data).getTime() - inicio.getTime()) / MS_POR_DIA);
}
