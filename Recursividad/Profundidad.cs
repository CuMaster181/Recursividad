using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unidad2;

public record FilaProfundidad(int Profundidad, string Recursivo, string Iterativo, string Estado);

/// <summary>
/// TAREA 2.3 - Medicion del comportamiento con distintas profundidades de llamada.
///
/// Cada llamada recursiva apila un "marco" en la pila de llamadas (call stack).
/// La pila tiene un tamano limitado (1 MB por defecto en un hilo de .NET). Si la
/// recursion es demasiado profunda ocurre un desbordamiento de pila (StackOverflow),
/// que en .NET termina el programa y NO se puede capturar.
/// Para poder medirlo sin que se cierre la aplicacion, cada llamada ejecuta
/// RuntimeHelpers.EnsureSufficientExecutionStack(), que lanza una excepcion
/// capturable (InsufficientExecutionStackException) ANTES de que la pila se agote.
/// </summary>
public static class Profundidad
{
    /// <summary>Suma 1 + 2 + ... + n de forma recursiva (la profundidad es n).</summary>
    public static long SumaRecursiva(int n)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();   // proteccion contra el desbordamiento
        if (n == 0) return 0;                              // CASO BASE
        return n + SumaRecursiva(n - 1);                   // CASO RECURSIVO
    }

    /// <summary>La misma suma con un ciclo: usa un solo marco de pila.</summary>
    public static long SumaIterativa(int n)
    {
        long suma = 0;
        for (int i = 1; i <= n; i++) suma += i;
        return suma;
    }

    /// <summary>Recursion sin caso base: sirve para averiguar hasta donde llega la pila.</summary>
    private static int Descender(int nivel, int[] alcanzado)
    {
        alcanzado[0] = nivel;
        RuntimeHelpers.EnsureSufficientExecutionStack();
        return 1 + Descender(nivel + 1, alcanzado);
    }

    /// <summary>Profundidad maxima que soporta una pila del tamano indicado.</summary>
    public static int ProfundidadMaxima(int megabytes)
    {
        return Utilidades.EjecutarConPila(() =>
        {
            var alcanzado = new int[1];
            try { Descender(0, alcanzado); }
            catch (InsufficientExecutionStackException) { }
            return alcanzado[0];
        }, megabytes);
    }

    /// <summary>Mide la suma recursiva e iterativa a una profundidad dada, con una pila de 'megabytes' MB.</summary>
    public static FilaProfundidad Medir(int profundidad, int megabytes)
    {
        const int repeticiones = 5;

        double msIterativo;
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < repeticiones; i++) SumaIterativa(profundidad);
            sw.Stop();
            msIterativo = sw.Elapsed.TotalMilliseconds / repeticiones;
        }

        try
        {
            double msRecursivo = Utilidades.EjecutarConPila(() =>
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < repeticiones; i++) SumaRecursiva(profundidad);
                sw.Stop();
                return sw.Elapsed.TotalMilliseconds / repeticiones;
            }, megabytes);

            return new FilaProfundidad(profundidad, msRecursivo.ToString("F4"), msIterativo.ToString("F4"), "OK");
        }
        catch (InsufficientExecutionStackException)
        {
            return new FilaProfundidad(profundidad, "---", msIterativo.ToString("F4"),
                                       "DESBORDAMIENTO de pila evitado");
        }
    }
}