#!/bin/zsh
export CONN_STRING_POSTGRES="host=postgresimage;port=5432;database=wiki;username=wikiadmin;password=admin"
export PORT=3001

if [ $1 = "migration" ] 
then
  dotnet ef migrations add $2
else
  dotnet run
fi