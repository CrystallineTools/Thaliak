FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Thaliak.Service.Api/Thaliak.Service.Api.csproj", "Thaliak.Service.Api/"]
RUN dotnet restore "Thaliak.Service.Api/Thaliak.Service.Api.csproj"
COPY . .
WORKDIR "/src/Thaliak.Service.Api"
RUN dotnet build "Thaliak.Service.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Thaliak.Service.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thaliak.Service.Api.dll"]
