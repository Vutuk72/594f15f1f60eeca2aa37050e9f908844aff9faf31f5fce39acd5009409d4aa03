# 1. fáze: sestavení aplikace v obrazu s celým .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Nejdřív jen .csproj a restore – Docker si tuto vrstvu zapamatuje,
# takže při změně kódu (ne balíčků) se balíčky nestahují znovu.
COPY Api.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# 2. fáze: malý obraz jen s runtime, bez SDK a zdrojáků
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
