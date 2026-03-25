# LagerPro — Datamodell

## Oversikt

```
┌─────────────────────────────────────────────────────────────────────┐
│                        STAMDATA                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              │
│  │  Leverandør   │  │   Artikkel   │  │    Kunde     │              │
│  │              │  │              │  │              │              │
│  │ Id           │  │ Id           │  │ Id           │              │
│  │ Navn         │  │ ArtikkelNr   │  │ Navn         │              │
│  │ KontaktInfo  │  │ Navn         │  │ KontaktInfo  │              │
│  │ Adresse      │  │ Beskrivelse  │  │ Adresse      │              │
│  │              │  │ Enhet (kg/stk)│  │ OrgNr       │              │
│  │              │  │ Type ────────┼──┤              │              │
│  │              │  │ (Råvare/     │  │              │              │
│  │              │  │  Ferdigvare) │  │              │              │
│  │              │  │ Strekkode    │  │              │              │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘              │
│         │                 │                 │                       │
└─────────┼─────────────────┼─────────────────┼───────────────────────┘
          │                 │                 │
          ▼                 ▼                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      MOTTAK → LAGER                                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────┐      ┌──────────────────┐                    │
│  │    Mottak         │      │   MottakLinje     │                    │
│  │                  │      │                  │                    │
│  │ Id               │──1:N─│ Id               │                    │
│  │ MottaksDato      │      │ MottakId (FK)    │                    │
│  │ LeverandørId(FK) │      │ ArtikelId (FK)   │                    │
│  │ Referanse        │      │ Antall/Mengde    │                    │
│  │ Kommentar        │      │ LotNr            │                    │
│  │                  │      │ BestFør          │                    │
│  │                  │      │ Avvik            │                    │
│  └──────────────────┘      │ Strekkode        │                    │
│                            └────────┬─────────┘                    │
│                                     │                               │
│                                     ▼                               │
│                            ┌──────────────────┐                    │
│                            │   LagerBeholdning │                    │
│                            │                  │                    │
│                            │ Id               │                    │
│                            │ ArtikelId (FK)   │                    │
│                            │ LotNr            │                    │
│                            │ Mengde           │                    │
│                            │ Lokasjon         │                    │
│                            │ SistOppdatert    │                    │
│                            └────────┬─────────┘                    │
│                                     │                               │
└─────────────────────────────────────┼───────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      PRODUKSJON                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────┐      ┌──────────────────┐                    │
│  │    Resept         │      │  ReseptLinje      │                    │
│  │                  │      │                  │                    │
│  │ Id               │──1:N─│ Id               │                    │
│  │ Navn             │      │ ReseptId (FK)    │                    │
│  │ FerdigvareId(FK) │      │ RåvareId (FK)    │                    │
│  │ Beskrivelse      │      │ Mengde           │                    │
│  │ AntallPortjoner  │      │ Enhet            │                    │
│  └──────────────────┘      └──────────────────┘                    │
│                                                                     │
│  ┌──────────────────┐      ┌──────────────────┐                    │
│  │ ProduksjonsOrdre  │      │ ProdOrdreForbruk  │                    │
│  │                  │      │                  │                    │
│  │ Id               │──1:N─│ Id               │                    │
│  │ ReseptId (FK)    │      │ ProdOrdreId (FK) │                    │
│  │ Dato             │      │ ArtikelId (FK)   │                    │
│  │ AntallProdusert  │      │ LotNr (råvare)   │                    │
│  │ FerdigvareLotNr  │      │ MengdeBrukt      │                    │
│  │ Status           │      └──────────────────┘                    │
│  │ (Planlagt/       │                                               │
│  │  UnderArbeid/    │      Kobling: FerdigvareLotNr                │
│  │  Ferdigmeldt)    │      knyttet til RåvareLotNr                 │
│  │ Kommentar        │      = FULL SPORBARHET                       │
│  └──────────────────┘                                               │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      LEVERING                                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────┐      ┌──────────────────┐                    │
│  │    Levering       │      │  LeveringLinje    │                    │
│  │                  │      │                  │                    │
│  │ Id               │──1:N─│ Id               │                    │
│  │ KundeId (FK)     │      │ LeveringId (FK)  │                    │
│  │ Dato             │      │ ArtikelId (FK)   │                    │
│  │ Referanse        │      │ LotNr            │                    │
│  │ Kommentar        │      │ Mengde           │                    │
│  │                  │      │                  │                    │
│  └──────────────────┘      └──────────────────┘                    │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘


## SPORBARHETSKJEDEN

  Leverandør → Mottak (LotNr) → Lager → Produksjon → Ferdigvare (nytt LotNr) → Levering → Kunde
                  │                          │                                      │
                  │          ProdOrdreForbruk │                                      │
                  └──── RåvareLotNr ──────────┘                                      │
                                             │                                      │
                                             └──── FerdigvareLotNr ─────────────────┘

  = Kan spore: "Kunde X fikk ferdigvare Y (lot 2024-001) som inneholdt råvare Z (lot R-445) fra leverandør W"
```

## Relasjoner

```
Leverandør  1 ──── N  Mottak
Mottak      1 ──── N  MottakLinje
Artikkel    1 ──── N  MottakLinje
Artikkel    1 ──── N  LagerBeholdning
Artikkel    1 ──── N  ReseptLinje (som råvare)
Artikkel    1 ──── 1  Resept (som ferdigvare)
Resept      1 ──── N  ReseptLinje
Resept      1 ──── N  ProduksjonsOrdre
ProdOrdre   1 ──── N  ProdOrdreForbruk
Kunde       1 ──── N  Levering
Levering    1 ──── N  LeveringLinje
Artikkel    1 ──── N  LeveringLinje
```

## Klasser / Entiteter

| Klasse | Familie | Beskrivelse |
|--------|---------|-------------|
| **Leverandør** | Stamdata | Hvem vi kjøper fra |
| **Artikkel** | Stamdata | Råvare eller ferdigvare (type-flag) |
| **Kunde** | Stamdata | Hvem vi leverer til |
| **Mottak** | Vareflyt | Innkommende forsendelse |
| **MottakLinje** | Vareflyt | Enkelt varemottak med LOT |
| **LagerBeholdning** | Lager | Nåværende beholdning per artikkel+LOT |
| **Resept** | Produksjon | Oppskrift: råvarer → ferdigvare |
| **ReseptLinje** | Produksjon | Ingrediens i resept |
| **ProduksjonsOrdre** | Produksjon | Én produksjonskjøring |
| **ProdOrdreForbruk** | Produksjon | Faktisk råvareforbruk med LOT-kobling |
| **Levering** | Levering | Utgående forsendelse til kunde |
| **LeveringLinje** | Levering | Enkelt vare levert med LOT |
