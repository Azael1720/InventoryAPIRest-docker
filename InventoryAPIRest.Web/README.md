# InventoryAPIRest.Web

Interfaz web (ASP.NET Core Razor Pages) que consume la API de inventario `InventoryAPIRest`.
Permite administrar categorías, productos y movimientos de inventario (entradas, salidas y ventas).

La web se autentica contra la API con OAuth2 (flujo *client credentials*) usando Keycloak.
El token lo pide el servidor, lo guarda en caché hasta que expira y lo envía en cada llamada;
el navegador nunca ve el token ni el secreto del cliente.

## Requisitos

- .NET 10 SDK
- Visual Studio 2026 o la CLI de .NET
- La API `InventoryAPIRest` en ejecución (por defecto `https://localhost:7013`)
- Keycloak en ejecución (por defecto `http://localhost:8080`) con el realm `InventoryRealm`
- Certificado de desarrollo de HTTPS de confianza: `dotnet dev-certs https --trust`


## 1. Crear el proyecto

### Con Visual Studio

1. *Archivo → Agregar → Nuevo proyecto* dentro de la solución.
2. Plantilla **ASP.NET Core Web App (Razor Pages)**.
3. Nombre `InventoryAPIRest.Web`, marco **.NET 10**.
4. Autenticación: ninguna. HTTPS: activado. Docker: desactivado (se agrega después).
5. Elimina `Pages/Privacy.cshtml` y `Pages/Privacy.cshtml.cs`.
6. Para ejecutar API y web a la vez: *clic derecho en la solución → Configurar proyectos de inicio →
   Varios proyectos* y marca ambos como *Iniciar*.

### Con la CLI

Desde la carpeta de la solución:

```powershell
dotnet new razor -n InventoryAPIRest.Web
dotnet sln add InventoryAPIRest.Web/InventoryAPIRest.Web.csproj
```

## 2. Crear el cliente de Keycloak

La web usa su propio cliente (`inventory-web`), distinto del que usa la API.

1. Abre la consola de administración: `http://localhost:8080/admin` e inicia sesión.
2. En el selector de realms (arriba a la izquierda) elige **InventoryRealm**.
3. Menú **Clients → Create client**.
4. **General settings**
   - Client type: `OpenID Connect`
   - Client ID: `inventory-web`
   - Pulsa **Next**.
5. **Capability config**
   - Client authentication: **On**
   - Authorization: Off
   - Authentication flow: deja marcado **solo** `Service accounts roles`
     (desmarca *Standard flow* y *Direct access grants*)
   - Pulsa **Next**.
6. **Login settings**: déjalo vacío (no se necesitan redirecciones) y pulsa **Save**.
7. Pestaña **Credentials**: copia el **Client secret**.
8. Pestaña **Client scopes → `inventory-web-dedicated` → Configure a new mapper → Audience**
   - Name: `inventory-api-audience`
   - Included Custom Audience: `inventory-api`
   - Add to access token: **On**
   - **Save**.

Sin el paso 8 la API rechaza el token por audiencia (`IDX10214`).

### Verificar el token

```powershell
$body = @{
    grant_type    = "client_credentials"
    client_id     = "inventory-web"
    client_secret = "<TU_CLIENT_SECRET>"
}

$token = (Invoke-RestMethod -Method Post `
    -Uri "http://localhost:8080/realms/InventoryRealm/protocol/openid-connect/token" `
    -Body $body).access_token

$payload = $token.Split('.')[1].Replace('-', '+').Replace('_', '/')
$payload = $payload.PadRight($payload.Length + (4 - $payload.Length % 4) % 4, '=')
[Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload)) |
    ConvertFrom-Json | Select-Object iss, aud, azp
```

Debe mostrar:

- `iss`: `http://localhost:8080/realms/InventoryRealm`
- `aud`: incluye `inventory-api`
- `azp`: `inventory-web`

## 3. Configuración

`appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7013"
  },
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/InventoryRealm",
    "ClientId": "inventory-web",
    "ClientSecret": ""
  }
}
```

`ApiSettings:BaseUrl` es solo el host de la API, **sin** `/api` ni `/swagger`.

El secreto **nunca** se guarda en el repositorio. En desarrollo:

```powershell
cd InventoryAPIRest.Web
dotnet user-secrets init
dotnet user-secrets set "Keycloak:ClientSecret" "<TU_CLIENT_SECRET>"
```

| Valor | Variable de entorno (Docker, producción) |
|---|---|
| `ApiSettings:BaseUrl` | `ApiSettings__BaseUrl` |
| `Keycloak:Authority` | `Keycloak__Authority` |
| `Keycloak:ClientId` | `Keycloak__ClientId` |
| `Keycloak:ClientSecret` | `Keycloak__ClientSecret` |

## 4. Ejecutar

1. Inicia Keycloak.
2. Inicia la API (`https://localhost:7013`).
3. Inicia la web (`dotnet run --project InventoryAPIRest.Web` o F5 en Visual Studio).
4. Abre la URL que indique la consola.

## Funcionalidad

| Recurso | GET | POST | PUT | DELETE |
|---|---|---|---|---|
| Categorías | Lista y carga para editar | Crear | Editar | Eliminar |
| Productos | Lista y carga para editar | Crear | Editar | Dar de baja (baja lógica) |
| Movimientos | Historial y filtro por producto | Entrada, salida y venta | — | — |

Los movimientos no se editan ni se eliminan: son un historial que la API no modifica.

Estados de la interfaz: barra de carga al navegar, botón con indicador al enviar formularios,
alertas de error con el detalle que devuelve la API, mensaje de éxito tras guardar,
estado vacío con enlace para crear el primer registro y validaciones en el cliente.

## Docker

`InventoryAPIRest.Web/Dockerfile` (contexto de construcción: carpeta de la solución):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY InventoryAPIRest.Web/InventoryAPIRest.Web.csproj InventoryAPIRest.Web/
RUN dotnet restore InventoryAPIRest.Web/InventoryAPIRest.Web.csproj
COPY InventoryAPIRest.Web/ InventoryAPIRest.Web/
RUN dotnet publish InventoryAPIRest.Web/InventoryAPIRest.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryAPIRest.Web.dll"]
```

Servicio en `docker-compose.yml` (adapta los nombres `api` y `keycloak`):

```yaml
  web:
    build:
      context: .
      dockerfile: InventoryAPIRest.Web/Dockerfile
    ports:
      - "5100:8080"
    environment:
      ApiSettings__BaseUrl: http://api:8080
      Keycloak__Authority: http://keycloak:8080/realms/InventoryRealm
      Keycloak__ClientId: inventory-web
      Keycloak__ClientSecret: ${WEB_CLIENT_SECRET}
    depends_on:
      - api
```

`WEB_CLIENT_SECRET` va en el archivo `.env`

