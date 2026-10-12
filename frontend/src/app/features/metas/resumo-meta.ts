import { diasAte } from '../../core/datas';
import { Meta } from '../../core/models';

export type SituacaoMeta = 'andamento' | 'concluida' | 'vencida';

export const ROTULO_SITUACAO: Record<SituacaoMeta, string> = {
  andamento: 'Em andamento',
  concluida: 'Concluída',
  vencida: 'Prazo vencido',
};

export interface ResumoMeta {
  meta: Meta;
  conta: string;
  /** Entre 0 e 100. */
  percentual: number;
  restante: number;
  situacao: SituacaoMeta;
  prazo: string;
}

export function resumirMeta(meta: Meta, conta: string, hoje = new Date()): ResumoMeta {
  const percentual =
    meta.valorMeta > 0 ? Math.min(100, Math.max(0, (meta.valorAtual / meta.valorMeta) * 100)) : 0;
  const dias = diasAte(meta.dataMeta, hoje);
  const situacao: SituacaoMeta =
    meta.valorAtual >= meta.valorMeta ? 'concluida' : dias < 0 ? 'vencida' : 'andamento';

  return {
    meta,
    conta,
    percentual,
    restante: Math.max(0, meta.valorMeta - meta.valorAtual),
    situacao,
    prazo: descreverPrazo(dias, situacao),
  };
}

function descreverPrazo(dias: number, situacao: SituacaoMeta): string {
  if (situacao === 'concluida') return 'Meta atingida';
  if (dias === 0) return 'Vence hoje';
  if (dias === 1) return 'Falta 1 dia';
  if (dias > 1) return `Faltam ${dias} dias`;
  return dias === -1 ? 'Venceu ontem' : `Venceu há ${-dias} dias`;
}
