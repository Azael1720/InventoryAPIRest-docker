FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InventoryAPIRest.Web/InventoryAPIRest.Web.csproj InventoryAPIRest.Web/

RUN dotnet restore InventoryAPIRest.Web/InventoryAPIRest.Web.csproj

COPY InventoryAPIRest.Web/ InventoryAPIRest.Web/
RUN dotnet publish InventoryAPIRest.Web/InventoryAPIRest.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 7035
ENTRYPOINT ["dotnet", "InventoryAPIRest.Web.dll"]