FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.Api/Thaliak.Api.csproj", "Thaliak.Api/"]
RUN dotnet restore "Thaliak.Api/Thaliak.Api.csproj"
COPY . .
WORKDIR "/src/Thaliak.Api"
RUN dotnet build "Thaliak.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.Api.dll"]
