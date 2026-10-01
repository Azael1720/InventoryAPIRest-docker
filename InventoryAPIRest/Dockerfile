FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore InventoryAPIRest.csproj

RUN dotnet publish InventoryAPIRest.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:7013
EXPOSE 7013

ENTRYPOINT ["dotnet", "InventoryAPIRest.dll"]