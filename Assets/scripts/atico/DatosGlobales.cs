using UnityEngine;
using System.Collections.Generic;

public static class DatosGlobales
{
    // Listas separadas para nombres y colores
    public static List<string> nombresGuardados = new List<string>();
    public static List<string> coloresGuardados = new List<string>();

    // Método para guardar datos si no están duplicados
    public static void GuardarDatos(string nombre, string color)
    {
        // Comprueba si ya existe el par de nombre y color
        if (nombresGuardados.Contains(nombre) && coloresGuardados.Contains(color))
        {
            Debug.Log($"El dato ya existe: {nombre} - {color}");
            return; // Sal del método si ya existe
        }

        // Agrega el nombre y color a sus respectivas listas
        nombresGuardados.Add(nombre);
        coloresGuardados.Add(color);

        Debug.Log($"Datos guardados: {nombre} - {color}");
    }

    // Método para obtener todos los datos como texto
    public static string ObtenerTodosLosDatos()
    {
        string resultado = "Datos guardados:\n";
        for (int i = 0; i < nombresGuardados.Count; i++)
        {
            resultado += $"- {nombresGuardados[i]}, {coloresGuardados[i]}\n";
        }
        return resultado;
    }
}

