FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/PropostaService.Domain/*.csproj ./src/PropostaService.Domain/
COPY src/PropostaService.Application/*.csproj ./src/PropostaService.Application/
COPY src/PropostaService.Infrastructure/*.csproj ./src/PropostaService.Infrastructure/
COPY src/PropostaService.API/*.csproj ./src/PropostaService.API/

RUN dotnet restore src/PropostaService.API/PropostaService.API.csproj

COPY . .

WORKDIR /app/src/PropostaService.API
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080

USER root
RUN mkdir -p /app/Logs && chown -R $USER /app/Logs
USER $USER

ENTRYPOINT ["dotnet", "PropostaService.API.dll"]