FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DeliveryAgency.Api.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=delivery_agency_db;Username=postgres;Password=postgres"
EXPOSE 8080
ENTRYPOINT ["dotnet", "DeliveryAgency.Api.dll"]
