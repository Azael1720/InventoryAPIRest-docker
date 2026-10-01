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
	

## Guía de Publicación: Cómo subir este proyecto a GitHub desde Visual Studio

Si deseas replicar este proceso o necesitas volver a configurar el repositorio remoto directamente desde el entorno de desarrollo, sigue estos pasos visuales:

### Paso 1: Abrir el asistente de Git
1. En la barra de menús superior de **Visual Studio**, haz clic en la pestaña **Git**.
2. Selecciona la opción **Crear repositorio de Git...** (*Create Git Repository...*).

### Paso 2: Configurar la cuenta y el repositorio remoto
En la ventana emergente que aparece, realiza los siguientes ajustes:
1. **Configuración local:** 
   * Asegúrate de que la plantilla de `.gitignore` esté configurada para **DotNet** o **VisualStudio** (esto evitará de forma automática que se suban carpetas pesadas o temporales como `bin/` u `obj/`).
2. **Seleccionar Destino:** 
   * En el panel izquierdo, haz clic sobre el ícono de **GitHub**.
3. **Detalles de la cuenta:**
   * **Cuenta:** Vincula tu cuenta de GitHub iniciando sesión desde el navegador si es la primera vez.
   * **Nombre del repositorio:** Asigna el nombre deseado (ej: `mi-api-net10-docker`).
   * **Visibilidad:** Asegúrate de **desmarcar** la casilla *Hacer que este repositorio sea privado* (o selecciona **Público**) para garantizar el acceso público al código fuente.

### Paso 3: Publicar el código fuente
1. Haz clic en el botón inferior **Crear y enviar cambios** (*Create and Push*).
2. Visual Studio inicializará Git en tu proyecto local, incluirá automáticamente archivos clave como tu `Dockerfile` y `docker-compose.yml`, creará el primer *commit* y subirá todo el contenido a tu perfil de GitHub de forma automática.

## Cómo descargar (Clonar) este Repositorio

Si necesitas descargar este proyecto en otra computadora o si otro desarrollador quiere usarlo, existen dos formas muy sencillas de hacerlo:

### Opción A: Desde Visual Studio (Método Visual Recomendado)
1. Abre **Visual Studio** (en la pantalla de inicio, antes de abrir ningún proyecto).
2. En la columna de la derecha, selecciona la opción **Clonar un repositorio** (*Clone a repository*).
3. En el campo **URL del repositorio**, pega el enlace de este repositorio de GitHub (ej: `https://github.com`).
4. Elige la **Ruta local** (la carpeta de tu computadora donde quieres guardar el proyecto).
5. Haz clic en el botón **Clonar**. Visual Studio descargará todo el código y abrirá la solución automáticamente.

### Opción B: Usando la Terminal de Git (Línea de Comandos)
1. Abre la terminal o consola de comandos en tu computadora.
2. Navega hasta la carpeta donde deseas guardar el proyecto.
3. Ejecuta el comando `git clone` seguido de la URL del repositorio:
   ```bash
   git clone https://github.com
   ```
4. Entra a la carpeta descargada:
   ```bash
   cd mi-api-net10-docker
   ```

Una vez descargado por cualquiera de los dos métodos, recuerda que solo necesitas ejecutar `docker compose up --build` en la raíz para encender todo el entorno.