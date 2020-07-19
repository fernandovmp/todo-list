#!/bin/bash
/opt/mssql/bin/sqlservr &
pid=$!

echo "Waiting to SQL Server initialization..."
sleep 80

echo "Initializing database"
/opt/mssql-tools/bin/sqlcmd -U sa -P  $SA_PASSWORD -l 30 -i /scripts/init.sql

wait $pid
