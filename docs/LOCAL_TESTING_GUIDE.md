# LagerPro — Lokal Testguide

## Kva du treng
- Windows PC med .NET SDK 8
- Docker Desktop (gratis)
- Git

---

## Steg 1: Få koden

### Allereie har repo (oppdater):
```powershell
cd C:\Users\marti\source\repos\LagerPro
git pull
```

### Nytt repo:
```powershell
git clone https://github.com/mtalle/LagerPro.git
cd LagerPro
```

---

## Steg 2: Start SQL Server (Docker)

### 2.1 Start Docker Desktop
Apne Docker Desktop (må vera køyrande).

### 2.2 Kjør i PowerShell (Administrator):
```powershell
# Stop gamle containerar
docker stop lagerpro-sql -ErrorAction SilentlyContinue
docker rm lagerpro-sql -ErrorAction SilentlyContinue

# Start SQL Server på port 14333
docker run -d `
  --name lagerpro-sql `
  -e "ACCEPT_EULA=Y" `
  -e "MSSQL_SA_PASSWORD=LagerPro123!" `
  -p 14333:1433 `
  mcr.microsoft.com/mssql/server:2022-latest
```

### 2.3 Vent 20 sekund, test:
```powershell
docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "SELECT 1"
```
Viser `1` = SQL Server er klar.

---

## Steg 3: Konfigurer tilgang

### 3.1 Oppdater appsettings (port 14333):
```powershell
$json = '{"ConnectionStrings":{"DefaultConnection":"Server=localhost,14333;Database=LagerProDb;User Id=sa;Password=LagerPro123!;Encrypt=False;TrustServerCertificate=True;"}}'
$json | Out-File -FilePath "C:\Users\marti\source\repos\LagerPro\src\LagerPro.Api\appsettings.json"
$json | Out-File -FilePath "C:\Users\marti\source\repos\LagerPro\src\LagerPro.Api\appsettings.Development.json"
```

### 3.2 Lag database:
```powershell
docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost,14333 -U sa -P "LagerPro123!" -C -Q "CREATE DATABASE LagerProDb"
```

### 3.3 Kjør migrering:
```powershell
cd C:\Users\marti\source\repos\LagerPro
dotnet tool install --global dotnet-ef --version 8.0.0 -ErrorAction SilentlyContinue
dotnet ef database update --connection "Server=localhost,14333;Database=LagerProDb;User Id=sa;Password=LagerPro123!;Encrypt=False;TrustServerCertificate=True;" --project src\LagerPro.Infrastructure --startup-project src\LagerPro.Api
```

---

## Steg 4: Start appen

### 4.1 Backend (ny terminal):
```powershell
cd C:\Users\marti\source\repos\LagerPro
dotnet run --project src\LagerPro.Api --urls "http://localhost:5000"
```

### 4.2 Frontend (ny terminal):
```powershell
cd C:\Users\marti\source\repos\LagerPro
dotnet run --project src\LagerPro.Web --urls "http://localhost:5001"
```

---

## Steg 5: Test

| Side | URL |
|------|-----|
| Frontend | http://localhost:5001 |
| API | http://localhost:5000/api/articles |
| Swagger | http://localhost:5000/swagger |

---

## Feilsøking

### "Port already in use"
```powershell
docker stop lagerpro-sql
docker rm lagerpro-sql
# Start på ny port (andre port i begge kommandoar)
```

### "Login failed for user 'sa'"
- Sjekk at Docker container kjører: `docker ps`
- Prøv restart: `docker restart lagerpro-sql`

---

## Kjapp restart (når du skal bruka appen igjen)

```powershell
# 1. Docker Desktop →开着

# 2. Start SQL Server
docker start lagerpro-sql

# 3. Vent 10 sek
Start-Sleep -Seconds 10

# 4. Start Backend
cd C:\Users\marti\source\repos\LagerPro
dotnet run --project src\LagerPro.Api --urls "http://localhost:5000"

# 5. Start Frontend (ny terminal)
dotnet run --project src\LagerPro.Web --urls "http://localhost:5001"
```
