# 🚀 API Rest con .NET 10 y Docker

Este repositorio contiene una **API Rest desarrollada en .NET 10** completamente dockerizada, la cual se conecta de manera automatizada a una base de datos **SQL Server**.

El proyecto está diseñado bajo una arquitectura de contenedores que automatiza tanto la inicialización del motor de base de datos como la ejecución de las migraciones de Entity Framework Core desde el primer arranque.

---

## 🛠️ Requisitos Previos

Antes de desplegar la aplicación, asegúrate de tener instalado en tu equipo:
*   [Docker Desktop](https://docker.com) (con el motor WSL2 activo si usas Windows).
*   Un cliente API como [Postman](https://getpostman.com) o tu navegador web para interactuar con los endpoints.

---

## En visual studio 

Se deben restaurar los packetes nugget o instalarlos:

1.- Instalados los paquetes de swagger en solución
	- Swashbuckle.AspNetCore
	- Swashbuckle.AspNetCore.SwaggerUI

2.- Instalados los paquetes de Microsoft en solución
	- Microsoft.EntityFrameworkCore.Tools
	- Microsoft.EntityFrameworkCore.SqlServer 
	- Moq.EntityFrameworkCore (Instalar solo en proyecto xUnit InventoryApiRest.Test)
	- Microsoft.AspNetCore.Authentication.JwtBearer
	- dotnet-ef
	 - comando: dotnet tool install --global dotnet-ef --version 10.*

3.- Descargar e instalar .NET 10.0 https://dotnet.microsoft.com/es-es/download/dotnet/10.0

## Despliegue con un Solo Comando

Abrir Docker desktop como primer paso
Para descargar las imágenes, compilar la API, enlazar la base de datos y ejecutar las migraciones estructurales automáticamente, abre una terminal en la carpeta raíz del proyecto y ejecuta:

```bash
docker compose up --build
```

### ¿Qué sucede en segundo plano?
1. **Orquestación inteligente:** El contenedor de la API esperará pacientemente a que el contenedor de SQL Server esté en estado saludable (`healthy`) antes de intentar cualquier acción.
2. **Migración automática:** Al arrancar por primera vez, el framework detectará tus archivos de migración (`InitialCreate`) e inicializará las tablas estructurales en la base de datos sin intervención manual.

---

## Puertos e Interacción

Una vez que veas en la terminal que los contenedores están corriendo de forma exitosa, podrás acceder a los servicios a través de las siguientes direcciones locales:

*   **API Principal:** [http://localhost:7013](http://localhost:7013)
*   **Base de Datos (SQL Server):** Ejecutándose internamente en el puerto `1433`.

---

## Estructura de Contenedores

La arquitectura se compone de dos servicios principales descritos en el archivo `docker-compose.yml`:
*   **`api`**: Servicio basado en la imagen oficial del runtime de **.NET 10.0**, configurado bajo el entorno de ejecución `Docker`.
*   **`db`**: Instancia oficial de **Microsoft SQL Server 2022** encargada del almacenamiento persistente de los datos de la aplicación.