FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.Poller/Thaliak.Poller.csproj", "Thaliak.Poller/"]
RUN dotnet restore "Thaliak.Poller/Thaliak.Poller.csproj"
COPY . .
WORKDIR "/src/Thaliak.Poller"
RUN dotnet build "Thaliak.Poller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Poller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.Poller.dll"]
