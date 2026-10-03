
using Microsoft.Extensions.Configuration;
using MIA_AzureBlob.Services;

DotNetEnv.Env.Load(); 


// ------------------ Cargar configuración ------------------
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")!;
string containerName = config["AzureBlobStorage:ContainerName"]!;
if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("TU_CUENTA"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Debes configurar la ConnectionString en appsettings.json");
    Console.ResetColor();
    return;
}

var servicio = new BlobStorageService(connectionString, containerName);

// ------------------ Menú principal ------------------
bool salir = false;
while (!salir)
{
    MostrarMenu();

    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine()?.Trim() ?? "";

    try
    {
        switch (opcion)
        {
            case "1":
                await SubirArchivo(servicio);
                break;
            case "2":
                await ListarArchivos(servicio);
                break;
            case "3":
                await DescargarArchivo(servicio);
                break;
            case "4":
                await EliminarArchivo(servicio);
                break;
            case "5":
                salir = true;
                Console.WriteLine("\n¡Hasta luego!");
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nOpción inválida. Intente nuevamente.");
                Console.ResetColor();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERROR] {ex.Message}");
        Console.ResetColor();
    }

    if (!salir)
    {
        Console.WriteLine("\nPresione ENTER para continuar...");
        Console.ReadLine();
    }
}

// ------------------ Funciones auxiliares ------------------

static void MostrarMenu()
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("     MIA - AZURE BLOB STORAGE");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Subir archivo");
    Console.WriteLine("2. Listar archivos");
    Console.WriteLine("3. Descargar archivo");
    Console.WriteLine("4. Eliminar archivo");
    Console.WriteLine("5. Salir");
    Console.WriteLine("=================================");
}

// --------- 3.2 Subir archivo ---------
static async Task SubirArchivo(BlobStorageService servicio)
{
    Console.WriteLine("\n--- SUBIR ARCHIVO ---");
    Console.Write("Ingrese la ruta del archivo local: ");
    string ruta = Console.ReadLine()?.Trim('"', ' ') ?? "";

    if (string.IsNullOrWhiteSpace(ruta))
    {
        Console.WriteLine("Ruta no válida.");
        return;
    }

    string nombre = await servicio.SubirArchivoAsync(ruta);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n✔ Archivo '{nombre}' subido exitosamente a Azure Blob Storage.");
    Console.ResetColor();
}

// --------- 3.3 Listar archivos ---------
static async Task ListarArchivos(BlobStorageService servicio)
{
    Console.WriteLine("\n--- LISTAR ARCHIVOS ---");

    var archivos = await servicio.ListarArchivosAsync();

    if (archivos.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nNo hay archivos en el contenedor.");
        Console.ResetColor();
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"Nombre",-35} {"Tamaño",15}");
    Console.WriteLine(new string('-', 52));

    foreach (var (nombre, tamanio) in archivos)
    {
        Console.WriteLine($"{nombre,-35} {$"{tamanio} bytes",15}");
    }

    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"Total: {archivos.Count} archivo(s).");
}

// --------- 3.4 Descargar archivo ---------
static async Task DescargarArchivo(BlobStorageService servicio)
{
    Console.WriteLine("\n--- DESCARGAR ARCHIVO ---");
    Console.Write("Ingrese el nombre del blob: ");
    string nombreBlob = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Nombre no válido.");
        return;
    }

    if (!await servicio.ExisteBlobAsync(nombreBlob))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"El blob '{nombreBlob}' no existe en el contenedor.");
        Console.ResetColor();
        return;
    }

    Console.Write("Ingrese la carpeta de destino: ");
    string carpeta = Console.ReadLine()?.Trim('"', ' ') ?? "";

    if (string.IsNullOrWhiteSpace(carpeta))
        carpeta = Directory.GetCurrentDirectory();

    string rutaGuardado = await servicio.DescargarArchivoAsync(nombreBlob, carpeta);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n✔ Archivo descargado exitosamente.");
    Console.WriteLine($"   Ubicación: {Path.GetFullPath(rutaGuardado)}");
    Console.ResetColor();
}

// --------- 3.5 Eliminar archivo ---------
static async Task EliminarArchivo(BlobStorageService servicio)
{
    Console.WriteLine("\n--- ELIMINAR ARCHIVO ---");
    Console.Write("Ingrese el nombre del blob: ");
    string nombreBlob = Console.ReadLine()?.Trim() ?? "";

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Nombre no válido.");
        return;
    }

    if (!await servicio.ExisteBlobAsync(nombreBlob))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"El blob '{nombreBlob}' no existe en el contenedor.");
        Console.ResetColor();
        return;
    }

    Console.Write($"\n¿Está seguro de eliminar '{nombreBlob}'? (s/n): ");
    string respuesta = Console.ReadLine()?.Trim().ToLower() ?? "";

    if (respuesta != "s" && respuesta != "si" && respuesta != "sí")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nOperación cancelada.");
        Console.ResetColor();
        return;
    }

    await servicio.EliminarArchivoAsync(nombreBlob);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n✔ Blob '{nombreBlob}' eliminado exitosamente.");
    Console.ResetColor();
}