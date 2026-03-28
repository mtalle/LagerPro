# LagerPro SQL Server Setup Script
# Kjør dette i PowerShell som Administrator

# Stop og fjern gammal container
docker stop lagerpro-sql -ErrorAction SilentlyContinue
docker rm lagerpro-sql -ErrorAction SilentlyContinue

# Start ny SQL Server
docker run -d `
  --name lagerpro-sql `
  -e "ACCEPT_EULA=Y" `
  -e "MSSQL_SA_PASSWORD=LagerPro123!" `
  -e "MSSQL_PID=Express" `
  -p 1433:1433 `
  mcr.microsoft.com/mssql/server:2022-latest

# Vent til SQL Server er klar (opptil 30 sek)
Write-Host "Venter paa SQL Server..."
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    $result = docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "SELECT 1" -b 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "SQL Server er klar!"
        $ready = $true
        break
    }
    Write-Host "Prover igjen... ($i/30)"
    Start-Sleep -Seconds 1
}

if (-not $ready) {
    Write-Host "FEIL: SQL Server klarte ikke a starte"
    exit 1
}

# Lag database
docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "CREATE DATABASE LagerProDb" -b

Write-Host ""
Write-Host "Ferdig! Kjor naa:"
Write-Host '  dotnet run --project src\LagerPro.Api --urls "http://localhost:5000"'
