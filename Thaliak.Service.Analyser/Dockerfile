FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.Service.Analyser/Thaliak.Service.Analyser.csproj", "Thaliak.Service.Analyser/"]
RUN dotnet restore "Thaliak.Service.Analyser/Thaliak.Service.Analyser.csproj"
COPY . .
WORKDIR "/src/Thaliak.Service.Analyser"
RUN dotnet build "Thaliak.Service.Analyser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Service.Analyser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.Service.Analyser.dll"]
