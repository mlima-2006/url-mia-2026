# MIA - Azure Blob Storage Console App

## Objetivo de la aplicación
Esta aplicación de consola en C# permite interactuar con un contenedor de Azure Blob Storage. El objetivo principal es gestionar archivos en la nube, brindando funcionalidades para subir, listar, descargar y eliminar blobs directamente desde la terminal del usuario.

## Tecnologías utilizadas
* C# / .NET 8.0 (o versión aplicable)
* Azure Blob Storage SDK (`Azure.Storage.Blobs`)
* Git / GitHub para control de versiones.

## Configuración de Azure
* Se aprovisionó un **Storage Account** en el portal de Azure.
* Se creó un contenedor privado llamado `archivos`.
* Se extrajo la **Connection String** desde el apartado de *Security + networking > Access keys*.

## Arquitectura de la solución
La solución está construida como un cliente de consola monolítico. Utiliza el paquete `Azure.Storage.Blobs` para establecer comunicación vía HTTPS y la API REST de Azure. 
1. **BlobServiceClient**: Maneja la conexión principal con la cuenta de almacenamiento.
2. **BlobContainerClient**: Maneja las operaciones que ocurren a nivel de la carpeta "archivos".
3. **BlobClient**: Representa y opera sobre cada archivo individual dentro del contenedor.

## Descripción de las cuatro operaciones
1. **Subir archivo**: Valida que el archivo local exista usando `File.Exists`. Extrae el nombre del archivo, obtiene una referencia `BlobClient` y lo sube de manera asíncrona usando `UploadAsync`.
2. **Listar archivos**: Itera de forma asíncrona usando `GetBlobsAsync()` sobre el contenedor, mostrando en consola el nombre (`blobItem.Name`) y tamaño (`blobItem.Properties.ContentLength`) de cada blob.
3. **Descargar archivo**: Valida primero que el blob exista en Azure (`ExistsAsync`). Luego solicita una ruta de destino al usuario, verifica que el directorio local exista y descarga los datos binarios a un archivo físico con `DownloadToAsync`.
4. **Eliminar archivo**: Valida la existencia del blob en la nube. Requiere una confirmación explícita (s/n) antes de emitir la orden de borrado mediante el método `DeleteAsync`.

## Manejo de errores
Todas las interacciones de red y operaciones de I/O de disco están envueltas en bloques `try-catch`. Esto previene que la aplicación haga *crash* si ocurre un problema de conexión, si se introducen rutas inválidas con permisos insuficientes, o si las credenciales de Azure caducan.

## Mecanismo utilizado para proteger la Connection String
Para cumplir con las normativas de seguridad y no filtrar secretos en GitHub, el código implementa la lectura de la cadena de conexión mediante **Variables de Entorno**. 
La aplicación utiliza:
`Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")`

Esto asegura que el código fuente sea seguro para versionar, mientras que la credencial se maneja de forma local en el entorno del sistema operativo.

## Instrucciones para ejecutar el proyecto

1. Clonar el repositorio.
2. Navegar a la carpeta del proyecto:
   ```bash
   cd MIA_AzureBlob
