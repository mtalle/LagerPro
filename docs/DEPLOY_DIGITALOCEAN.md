# LagerPro — DigitalOcean Deployguide

## Kva du treng
- DigitalOcean konto ( registrera deg gratis)
- Domene (valfritt, kan bruka IP-adresse)
- Kredittkort for $8/mnd

---

## Steg 1: Opprett VPS

### 1.1 Logg inn på DigitalOcean
- Gå til https://www.digitalocean.com
- Lag konto / logg inn

### 1.2 Opprett ny droplet
- Klikk **"Create" → "Droplets"**
- Velg:
  - **Region:** Frankrike eller Amsterdam (nærmast Norge)
  - **Image:** Ubuntu 22.04 LTS
  - **Size:** $8/mnd (2GB RAM, 1 vCPU, 50GB SSD)
  - **Authentication:** Password eller SSH key (vel Password for enkelheit)

### 1.3 Vent 1 minutt
- Du får e-post med IP-adresse og passord
- Kopier IP-adressen (t.d. `123.456.78.90`)

---

## Steg 2: Koble til serveren

### 2.1 Open PowerShell eller Command Prompt
```powershell
ssh root@123.456.78.90
```
(Erstatt med di IP-adresse)

### 2.2 Skriv inn passordet frå e-posten
- Fyrste gong blir du beden om å setja nytt passord

---

## Steg 3: Installer Docker

### 3.1 Kjør dette på serveren:
```bash
apt update && apt upgrade -y
apt install -y docker.io docker-compose
systemctl start docker
systemctl enable docker
```

### 3.2 Test:
```bash
docker --version
docker-compose --version
```

---

## Steg 4: Sett opp appen

### 4.1 Last ned koden:
```bash
cd /root
git clone https://github.com/mtalle/LagerPro.git
cd LagerPro
```

### 4.2 Sett opp miljøvariablar:
```bash
export ASPNETCORE_URLS="http://+:5000"
export ConnectionStrings__DefaultConnection="Server=localhost;Database=LagerProDb;User Id=sa;Password=LagerPro123!;TrustServerCertificate=True;"
```

### 4.3 Start med Docker Compose:
```bash
# Lag docker-compose.yml
cat > /root/LagerPro/docker-compose.yml << 'EOF'
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: lagerpro-sql
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=LagerPro123!
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    restart: unless-stopped

  api:
    build:
      context: .
      dockerfile: Dockerfile.Api
    container_name: lagerpro-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=LagerProDb;User Id=sa;Password=LagerPro123!;TrustServerCertificate=True;
      - ASPNETCORE_URLS=http://+:5000
    ports:
      - "5000:5000"
    depends_on:
      - sqlserver
    restart: unless-stopped

  web:
    build:
      context: .
      dockerfile: Dockerfile.Web
    container_name: lagerpro-web
    ports:
      - "5001:5001"
    depends_on:
      - api
    restart: unless-stopped

volumes:
  sqlserver_data:
EOF
```

### 4.4 Lag Dockerfiles:
```bash
# Dockerfile for API
cat > /root/LagerPro/Dockerfile.Api << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/LagerPro.Api/LagerPro.Api.csproj
RUN dotnet publish src/LagerPro.Api/LagerPro.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "LagerPro.Api.dll"]
EOF

# Dockerfile for Web
cat > /root/LagerPro/Dockerfile.Web << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/LagerPro.Web/LagerPro.Web.csproj
RUN dotnet publish src/LagerPro.Web/LagerPro.Web.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5001
ENTRYPOINT ["dotnet", "LagerPro.Web.dll"]
EOF
```

---

## Steg 5: Start alt

```bash
cd /root/LagerPro
docker-compose up -d
```

Vent 2-3 minutt for første oppstart (må laste ned bilete + kjøra migrations).

Sjekk loggar:
```bash
docker-compose logs -f
```

---

## Steg 6: Test

Opne nettlesar:
- **API:** `http://123.456.78.90:5000`
- **Web:** `http://123.456.78.90:5001`

---

## Steg 7: Få tilgang utan portnummer

### Med IP (berre for testing):
- Frontend: `http://123.456.78.90:5001`
- API: `http://123.456.78.90:5000`

### Med domene (betre):
1. Kjøp domene (t.d. `lagerpro.no` frå norid.no)
2. Lag DNS A-record: `lagerpro.no` → `123.456.78.90`
3. Sett opp Nginx som reverse proxy:

```bash
apt install -y nginx

cat > /etc/nginx/sites-available/lagerpro << 'EOF'
server {
    listen 80;
    server_name lagerpro.no;

    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
    }

    location /api/ {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }
}
EOF

ln -s /etc/nginx/sites-available/lagerpro /etc/nginx/sites-enabled/
nginx -t
systemctl reload nginx
```

---

## Kva det kostar

| Teneste | Pris |
|---------|------|
| DigitalOcean VPS (2GB RAM) | $8/mnd |
| Domene | ~100 kr/år |
| **Total** | **ca 80 kr/mnd** |

---

## Shutdown / Restart

### Stopp:
```bash
cd /root/LagerPro
docker-compose down
```

### Start:
```bash
cd /root/LagerPro
docker-compose up -d
```

### Restart:
```bash
cd /root/LagerPro
docker-compose restart
```

---

## Oppdater koden

```bash
cd /root/LagerPro
git pull
docker-compose up -d --build
```

---

## Backups

### Backup av database:
```bash
docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "BACKUP DATABASE LagerProDb TO DISK='/var/opt/mssql/backup/LagerProDb.bak'"
docker cp lagerpro-sql:/var/opt/mssql/backup/LagerProDb.bak ./LagerProDb_backup.bak
```

### Automatisk backup (cron):
```bash
# Legg til i crontab
crontab -e
# Legg til linje:
0 2 * * * docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "BACKUP DATABASE LagerProDb TO DISK='/var/opt/mssql/backup/LagerProDb.bak'" && docker cp lagerpro-sql:/var/opt/mssql/backup/LagerProDb.bak /root/LagerPro_backups/
```

---

## Konklusjon

Med $8/mnd får du:
- 5-10 samtidige brukarar
- 24/7 tilgjenge
- Ingen manuell drift
- Enkel skalering opp ved behov
