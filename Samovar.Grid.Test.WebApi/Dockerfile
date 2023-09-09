#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Samovar.Grid.Test.WebApi/Samovar.Grid.Test.WebApi.csproj", "Samovar.Grid.Test.WebApi/"]
RUN dotnet restore "Samovar.Grid.Test.WebApi/Samovar.Grid.Test.WebApi.csproj"
COPY . .
WORKDIR "/src/Samovar.Grid.Test.WebApi"
RUN dotnet build "Samovar.Grid.Test.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Samovar.Grid.Test.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Samovar.Grid.Test.WebApi.dll"]