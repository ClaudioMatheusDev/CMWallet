# CMWallet — Frontend

Aplicação Angular 22 (componentes standalone, signals, detecção de mudanças sem Zone.js).

## Executando

```bash
npm install
npm start            # http://localhost:4200
```

O `ng serve` encaminha `/api` para `http://localhost:5161` (veja `proxy.conf.json`).
Suba a API antes com `dotnet run --project CMWallet.API`.

| Comando         | O que faz                       |
| --------------- | ------------------------------- |
| `npm start`     | Servidor de desenvolvimento     |
| `npm run build` | Build de produção em `dist/`    |
| `npm test`      | Testes unitários (Vitest)       |

## Estrutura

```text
src/
├── app/
│   ├── core/                 # Sem UI: modelos, serviços HTTP, utilitários
│   │   ├── services/         # Um serviço por recurso da API
│   │   ├── feedback/         # Toasts e diálogo de confirmação
│   │   ├── datas.ts          # Datas no fuso local (yyyy-MM-dd)
│   │   └── http-error.ts     # Mensagens a partir de erros da API
│   ├── shared/               # Componentes visuais reutilizáveis
│   └── features/             # Uma pasta por tela (rotas com lazy loading)
└── styles/                   # Design system: tokens, base, layout, componentes
```

### Convenções

- **Estado** em `signal`/`computed`; nada de `ChangeDetectorRef` manual.
- **Formulários** reativos e tipados, com validação espelhando os DTOs da API.
- **Estilos** globais em `src/styles`, organizados por camada e baseados nos tokens de
  `tokens.css` (tema claro e escuro via `prefers-color-scheme`).
- **Datas** trafegam como `yyyy-MM-dd`. Use `core/datas.ts`; `toISOString()` converte para UTC
  e adianta o dia à noite no Brasil.
