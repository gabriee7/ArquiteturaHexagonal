FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/ContratacaoService.Domain/*.csproj ./src/ContratacaoService.Domain/
COPY src/ContratacaoService.Application/*.csproj ./src/ContratacaoService.Application/
COPY src/ContratacaoService.Infrastructure/*.csproj ./src/ContratacaoService.Infrastructure/
COPY src/ContratacaoService.API/*.csproj ./src/ContratacaoService.API/

RUN dotnet restore src/ContratacaoService.API/ContratacaoService.API.csproj

COPY . .

WORKDIR /app/src/ContratacaoService.API
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ContratacaoService.API.dll"]