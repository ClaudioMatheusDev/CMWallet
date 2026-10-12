import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extrai uma mensagem legível de um erro HTTP da API.
 *
 * A API responde `{ message }` para erros de negócio e `ProblemDetails` com
 * `errors` para falhas de validação de modelo.
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
    const { message, errors } = corpo as { message?: unknown; errors?: unknown };
    if (typeof message === 'string' && message.trim()) {
      return message;
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
