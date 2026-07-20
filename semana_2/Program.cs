using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;



//Leer el archivo estudiantes.csv completo
string[] lineas = File.ReadAllLines("estudiantes.csv");

// Crear la lista vacía para almacenar a los estudiantes
List<Estudiante> listaEstudiantes = new List<Estudiante>();

// Omitir la primera línea
for (int i = 1; i < lineas.Length; i++)
{
    string linea = lineas[i];
    
    //  método Split(',')
    string[] datos = linea.Split(',');

    if (datos.Length == 3)
    {
        // 4. Crear objetos de tipo Estudiante 
        Estudiante nuevoEstudiante = new Estudiante
        {
            Id = int.Parse(datos[0]),
            Nombre = datos[1],
            Carrera = datos[2]
        };

        //List<Estudiante>
        listaEstudiantes.Add(nuevoEstudiante);
    }
}

//Mostrar todos los estudiantes en consola
Console.WriteLine("--- Estudiantes Procesados ---");
foreach (var est in listaEstudiantes)
{
    Console.WriteLine($"ID: {est.Id}, Nombre: {est.Nombre}, Carrera: {est.Carrera}");
}

//Convertir la lista a formato JSON
string jsonResultado = JsonSerializer.Serialize(listaEstudiantes, new JsonSerializerOptions { WriteIndented = true });

//estudiantes.json
File.WriteAllText("estudiantes.json", jsonResultado);

Console.WriteLine("\n¡Proceso terminado! Archivo 'estudiantes.json' generado con éxito.");
