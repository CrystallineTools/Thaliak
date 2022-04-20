FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.PatchInspector/Thaliak.PatchInspector.csproj", "Thaliak.PatchInspector/"]
RUN dotnet restore "Thaliak.PatchInspector/Thaliak.PatchInspector.csproj"
COPY . .
WORKDIR "/src/Thaliak.PatchInspector"
RUN dotnet build "Thaliak.PatchInspector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.PatchInspector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.PatchInspector.dll"]
