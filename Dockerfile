#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /
COPY ["GarbageCan/GarbageCan.csproj", "GarbageCan/"]
RUN dotnet restore "GarbageCan/GarbageCan.csproj"
COPY . .
WORKDIR "GarbageCan"
RUN dotnet build "GarbageCan.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GarbageCan.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GarbageCan.dll"]
