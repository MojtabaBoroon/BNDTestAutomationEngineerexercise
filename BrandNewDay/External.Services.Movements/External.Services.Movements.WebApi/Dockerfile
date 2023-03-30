#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["External.Services.Movements.WebApi/External.Services.Movements.WebApi.csproj", "External.Services.Movements.WebApi/"]
RUN dotnet restore "External.Services.Movements.WebApi/External.Services.Movements.WebApi.csproj"
COPY . .
WORKDIR "/src/External.Services.Movements.WebApi"
RUN dotnet build "External.Services.Movements.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "External.Services.Movements.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "External.Services.Movements.WebApi.dll"]