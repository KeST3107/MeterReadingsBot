FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
EXPOSE 5432

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG Configuration=Release
WORKDIR /src
COPY *.sln ./
COPY MeterReadingsBot/MeterReadingsBot.csproj MeterReadingsBot/
COPY MeterReadingsBotTest/MeterReadingsBotTest.csproj MeterReadingsBotTest/
RUN dotnet restore
COPY . .
WORKDIR /src/MeterReadingsBot
RUN dotnet build -c $Configuration -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MeterReadingsBot.dll"]