# LagerPro — Foreslått mappestruktur

```text
lagerpro/
├── README.md
├── datamodell.md
├── diagram.mmd
├── klassediagram.md
├── docs/
│   ├── architecture/
│   │   └── folder-structure.md
│   ├── domain/
│   └── api/
├── src/
│   ├── LagerPro.Api/
│   ├── LagerPro.Application/
│   ├── LagerPro.Contracts/
│   ├── LagerPro.Domain/
│   │   ├── Common/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Events/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── ValueObjects/
│   └── LagerPro.Infrastructure/
│       ├── Persistence/
│       ├── Repositories/
│       └── Services/
└── tests/
    ├── LagerPro.Application.Tests/
    └── LagerPro.Domain.Tests/
```

## Tanke bak strukturen

- `Domain` holder forretningsregler og kjerneobjekter.
- `Application` holder use cases og flyt mellom API og domene.
- `Infrastructure` holder database og integrasjoner.
- `Api` holder HTTP-laget.
- `Contracts` holder DTO-er og request/response-modeller.
- `tests` speiler hovedprosjektene.

## Domeneområder

Mappene og klassene bør organiseres rundt disse områdene:

- Stamdata
- Mottak
- Lager
- Produksjon
- Levering
- Sporbarhet
