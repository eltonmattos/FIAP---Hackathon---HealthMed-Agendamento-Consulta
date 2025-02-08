FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .

RUN ls

RUN dotnet restore "./HealthMed.API.AgendamentoConsulta.csproj" --disable-parallel
RUN dotnet publish "./HealthMed.API.AgendamentoConsulta.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080

ENTRYPOINT ["dotnet", "HealthMed.API.AgendamentoConsulta.dll"]