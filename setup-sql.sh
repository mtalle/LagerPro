#!/bin/bash

# Stop og fjern gammal container
docker stop lagerpro-sql 2>/dev/null || true
docker rm lagerpro-sql 2>/dev/null || true

# Start ny SQL Server
docker run -d \
  --name lagerpro-sql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=LagerPro123!" \
  -e "MSSQL_PID=Express" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest

# Vent til SQL Server er klar (opptil 30 sek)
echo "Venter på SQL Server..."
for i in {1..30}; do
  if docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "SELECT 1" -b &>/dev/null; then
    echo "SQL Server er klar!"
    break
  fi
  echo "Prøver igjen... ($i/30)"
  sleep 1
done

# Lag database
docker exec lagerpro-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "LagerPro123!" -C -Q "CREATE DATABASE LagerProDb" -b

echo ""
echo "Ferdig! Kjør nå:"
echo "  dotnet run --project src/LagerPro.Api --urls \"http://localhost:5000\""
