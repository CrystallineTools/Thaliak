FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.Analyser/Thaliak.Analyser.csproj", "Thaliak.Analyser/"]
RUN dotnet restore "Thaliak.Analyser/Thaliak.Analyser.csproj"
COPY . .
WORKDIR "/src/Thaliak.Analyser"
RUN dotnet build "Thaliak.Analyser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Analyser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.Analyser.dll"]
