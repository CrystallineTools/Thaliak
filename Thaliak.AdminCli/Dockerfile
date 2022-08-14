FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.AdminCli/Thaliak.AdminCli.csproj", "Thaliak.AdminCli/"]
RUN dotnet restore "Thaliak.AdminCli/Thaliak.AdminCli.csproj"
COPY . .
WORKDIR "/src/Thaliak.AdminCli"
RUN dotnet build "Thaliak.AdminCli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.AdminCli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.AdminCli.dll"]
