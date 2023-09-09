#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Samovar.Grid.Test.Server/Samovar.Grid.Test.Server.csproj", "Samovar.Grid.Test.Server/"]
COPY ["Samovar.DataGrid/Samovar.DataGrid.csproj", "Samovar.DataGrid/"]
RUN dotnet restore "Samovar.Grid.Test.Server/Samovar.Grid.Test.Server.csproj"
COPY . .
WORKDIR "/src/Samovar.Grid.Test.Server"
RUN dotnet build "Samovar.Grid.Test.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Samovar.Grid.Test.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Samovar.Grid.Test.Server.dll"]