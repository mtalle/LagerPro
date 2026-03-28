# LagerPro — SQL-plan

## Første tabeller

- Artikler
- Leverandorer
- Kunder
- Mottak
- MottakLinjer
- LagerBeholdninger
- LagerTransaksjoner
- Resepter
- ReseptLinjer
- ProduksjonsOrdre
- ProdOrdreForbruk
- Leveringer
- LeveringLinjer

## Viktige constraints

- `Artikkel.ArtikkelNr` skal være unik
- `LagerBeholdning (ArtikkelId, LotNr)` skal være unik
- `ProduksjonsOrdre.OrdreNr` skal være unik
- Fremmednøkler settes med `DeleteBehavior.Restrict` der sporbarhet er viktig

## Decimal-felt

Bruker foreløpig:

- `decimal(18,2)` for pris
- `decimal(18,3)` for mengder
- `decimal(5,2)` for temperatur

## Neste steg

1. Lage første EF Core migrering
2. Koble `ArticlesController` til repository/db i stedet for in-memory liste
3. Lage seed-data for demo / lokal utvikling
4. Bygge sporbarhetsspørringer på LOT-nivå
