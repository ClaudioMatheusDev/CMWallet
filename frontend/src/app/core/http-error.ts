import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extrai uma mensagem legível de um erro HTTP da API.
 *
 * A API responde todos os erros como `ProblemDetails`: a mensagem para o usuário
 * vem em `detail`, e falhas de validação trazem também `errors` por campo.
 */
export function mensagemDeErro(erro: unknown, padrao: string): string {
  if (!(erro instanceof HttpErrorResponse)) {
    return padrao;
  }
  if (erro.status === 0) {
    return 'Não foi possível conectar à API. Verifique se o backend está em execução.';
  }

  const corpo: unknown = erro.error;
  if (corpo && typeof corpo === 'object') {
    const { detail, errors } = corpo as { detail?: unknown; errors?: unknown };
    if (typeof detail === 'string' && detail.trim()) {
      return detail;
    }
    if (errors && typeof errors === 'object') {
      const primeira = Object.values(errors)
        .flat()
        .find((item): item is string => typeof item === 'string' && item.trim() !== '');
      if (primeira) {
        return primeira;
      }
    }
  }
  return padrao;
}
