import { HttpErrorResponse } from '@angular/common/http';
import { mensagemDeErro } from './http-error';

const PADRAO = 'Falhou.';

describe('mensagemDeErro', () => {
  it('usa o detail do ProblemDetails', () => {
    const erro = new HttpErrorResponse({
      status: 409,
      error: { title: 'Operação não permitida', detail: 'A conta "Corrente" possui 2 transações.' },
    });
    expect(mensagemDeErro(erro, PADRAO)).toBe('A conta "Corrente" possui 2 transações.');
  });

  it('cai no primeiro erro de validação quando não há detail', () => {
    const erro = new HttpErrorResponse({
      status: 400,
      error: { errors: { Valor: ['O valor da transação deve ser maior que zero.'] } },
    });
    expect(mensagemDeErro(erro, PADRAO)).toBe('O valor da transação deve ser maior que zero.');
  });

  it('explica quando a API está inacessível', () => {
    expect(mensagemDeErro(new HttpErrorResponse({ status: 0 }), PADRAO)).toContain('conectar');
  });

  it('cai no texto padrão para respostas sem corpo útil', () => {
    expect(mensagemDeErro(new HttpErrorResponse({ status: 500 }), PADRAO)).toBe(PADRAO);
    expect(mensagemDeErro(new Error('x'), PADRAO)).toBe(PADRAO);
  });
});
