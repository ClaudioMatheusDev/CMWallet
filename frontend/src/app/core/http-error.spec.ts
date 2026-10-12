import { HttpErrorResponse } from '@angular/common/http';
import { mensagemDeErro } from './http-error';

const PADRAO = 'Falhou.';

describe('mensagemDeErro', () => {
  it('usa a mensagem de regra de negócio da API', () => {
    const erro = new HttpErrorResponse({
      status: 404,
      error: { message: 'Conta com Id 3 não encontrado.' },
    });
    expect(mensagemDeErro(erro, PADRAO)).toBe('Conta com Id 3 não encontrado.');
  });

  it('usa o primeiro erro de validação do ProblemDetails', () => {
    const erro = new HttpErrorResponse({
      status: 400,
      error: {
        title: 'One or more validation errors occurred.',
        errors: { Valor: ['O valor da transação deve ser maior que zero.'] },
      },
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
