import { diasAte, intervaloDoMes, paraDataISO } from './datas';

describe('datas', () => {
  it('formata no fuso local, sem converter para UTC', () => {
    expect(paraDataISO(new Date(2026, 9, 12, 23, 30))).toBe('2026-10-12');
  });

  it('calcula o primeiro e o último dia do mês', () => {
    expect(intervaloDoMes(new Date(2026, 1, 10))).toEqual({
      inicio: '2026-02-01',
      fim: '2026-02-28',
    });
  });

  it('conta dias até uma data vinda da API, ignorando o horário', () => {
    const hoje = new Date(2026, 9, 12, 22, 0);
    expect(diasAte('2026-10-15T00:00:00', hoje)).toBe(3);
    expect(diasAte('2026-10-12T00:00:00', hoje)).toBe(0);
    expect(diasAte('2026-10-10', hoje)).toBe(-2);
  });
});
