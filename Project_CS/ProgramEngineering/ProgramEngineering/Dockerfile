﻿FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProgramEngineering/ProgramEngineering.csproj", "ProgramEngineering/"]
RUN dotnet restore "ProgramEngineering/ProgramEngineering.csproj"
COPY . .
WORKDIR "/src/ProgramEngineering"
RUN dotnet build "ProgramEngineering.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProgramEngineering.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProgramEngineering.dll"]
