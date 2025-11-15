# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "OscarCinema.API/OscarCinema.API.csproj"
RUN dotnet publish "OscarCinema.API/OscarCinema.API.csproj" -c Release -o /app/publish

# RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Porta padrão dentro do container
ENV ASPNETCORE_URLS=http://0.0.0.0:5028

COPY --from=build /app/publish .

EXPOSE 5028

ENTRYPOINT ["dotnet", "OscarCinema.API.dll"]