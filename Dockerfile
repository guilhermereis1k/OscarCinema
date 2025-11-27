FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY /src .

RUN dotnet restore "OscarCinema.API/OscarCinema.API.csproj"

COPY . .

RUN dotnet build "OscarCinema.API/OscarCinema.API.csproj" -c Release --no-restore
RUN dotnet publish "OscarCinema.API/OscarCinema.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OscarCinema.API.dll"]