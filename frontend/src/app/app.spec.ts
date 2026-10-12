import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('renderiza a marca e a navegação principal', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const elemento = fixture.nativeElement as HTMLElement;

    expect(elemento.querySelector('.sidebar .brand__name')?.textContent).toContain('CMWallet');

    const rotulos = Array.from(elemento.querySelectorAll('.nav__link')).map((link) =>
      link.textContent?.trim(),
    );
    expect(rotulos).toEqual(['Visão geral', 'Transações', 'Metas', 'Contas', 'Categorias']);
  });
});
