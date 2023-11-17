FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Thaliak.Service.Poller/Thaliak.Service.Poller.csproj", "Thaliak.Service.Poller/"]
RUN dotnet restore "Thaliak.Service.Poller/Thaliak.Service.Poller.csproj"
COPY . .
WORKDIR "/src/Thaliak.Service.Poller"
RUN dotnet build "Thaliak.Service.Poller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Service.Poller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# certified Square Enix moment
COPY ./openssl.cnf /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "Thaliak.Service.Poller.dll"]
