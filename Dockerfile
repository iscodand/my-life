#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["src/MyLifeApp.WebApi/MyLifeApp.WebApi.csproj", "src/MyLifeApp.WebApi/"]
COPY ["src/MyLifeApp.Infrastructure.Ioc/MyLifeApp.Infrastructure.Ioc.csproj", "src/MyLifeApp.Infrastructure.Ioc/"]
COPY ["src/MyLifeApp.Infrastructure.Identity/MyLifeApp.Infrastructure.Identity.csproj", "src/MyLifeApp.Infrastructure.Identity/"]
COPY ["src/MyLifeApp.Application/MyLifeApp.Application.csproj", "src/MyLifeApp.Application/"]
COPY ["src/MyLifeApp.Domain/MyLifeApp.Domain.csproj", "src/MyLifeApp.Domain/"]
COPY ["src/MyLifeApp.Infrastructure.Data/MyLifeApp.Infrastructure.Data.csproj", "src/MyLifeApp.Infrastructure.Data/"]
COPY ["src/MyLifeApp.WebApi.Test/MyLifeApp.WebApi.Test.csproj", "src/MyLifeApp.WebApi.Test/"]

RUN dotnet restore "src/MyLifeApp.WebApi/MyLifeApp.WebApi.csproj"
COPY . .

RUN dotnet build "src/MyLifeApp.WebApi/MyLifeApp.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/MyLifeApp.WebApi/MyLifeApp.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyLifeApp.WebApi.dll"]