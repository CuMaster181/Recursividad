using System.Diagnostics;

namespace Unidad2;

public class ResultadoLaberinto
{
    public bool Resuelto;
    public long Llamadas;
    public int ProfundidadMaxima;
    public long Retrocesos;       // veces que se "deshizo" un paso (backtracking)
    public int LongitudCamino;
    public double Milisegundos;
    public bool[,] Visitado = new bool[0, 0];
    public List<(int Fila, int Col)> Camino = new();
}

/// <summary>
/// TAREA 2.3 - Laberinto resuelto con recursividad y backtracking.
/// El laberinto se GENERA con busqueda en profundidad recursiva (DFS) y se
/// RESUELVE con otra busqueda recursiva que retrocede cuando llega a un callejon sin salida.
/// Direcciones: 0 = arriba, 1 = derecha, 2 = abajo, 3 = izquierda.
/// </summary>
public class Laberinto
{
    private static readonly int[] DF = { -1, 0, 1, 0 };
    private static readonly int[] DC = { 0, 1, 0, -1 };

    public int Filas { get; }
    public int Columnas { get; }
    public bool[,,] Muros { get; }     // Muros[fila, col, direccion] = true si hay pared

    private Laberinto(int filas, int columnas)
    {
        Filas = filas;
        Columnas = columnas;
        Muros = new bool[filas, columnas, 4];
        for (int f = 0; f < filas; f++)
            for (int c = 0; c < columnas; c++)
                for (int d = 0; d < 4; d++)
                    Muros[f, c, d] = true;
    }

    /// <summary>Genera un laberinto perfecto (siempre tiene solucion y un unico camino).</summary>
    public static Laberinto Crear(int filas, int columnas, int semilla)
    {
        // La generacion es recursiva y puede ser profunda: se usa un hilo con pila grande.
        return Utilidades.EjecutarConPila(() =>
        {
            var lab = new Laberinto(filas, columnas);
            var visitado = new bool[filas, columnas];
            lab.Generar(0, 0, new Random(semilla), visitado);
            return lab;
        }, 256);
    }

    private void Generar(int f, int c, Random rnd, bool[,] visitado)
    {
        visitado[f, c] = true;

        int[] orden = { 0, 1, 2, 3 };
        for (int i = 3; i > 0; i--)                   // mezclar las direcciones (Fisher-Yates)
        {
            int j = rnd.Next(i + 1);
            (orden[i], orden[j]) = (orden[j], orden[i]);
        }

        foreach (int d in orden)
        {
            int nf = f + DF[d], nc = c + DC[d];
            if (nf < 0 || nc < 0 || nf >= Filas || nc >= Columnas || visitado[nf, nc]) continue;

            Muros[f, c, d] = false;                   // se derriba la pared entre ambas celdas
            Muros[nf, nc, (d + 2) % 4] = false;
            Generar(nf, nc, rnd, visitado);           // llamada recursiva
        }
    }

    /// <summary>Resuelve el laberinto de la esquina superior izquierda a la inferior derecha.</summary>
    public ResultadoLaberinto Resolver()
    {
        return Utilidades.EjecutarConPila(() =>
        {
            var r = new ResultadoLaberinto { Visitado = new bool[Filas, Columnas] };
            var sw = Stopwatch.StartNew();
            r.Resuelto = Explorar(0, 0, 1, r);
            sw.Stop();
            r.Milisegundos = sw.Elapsed.TotalMilliseconds;
            r.LongitudCamino = r.Camino.Count;
            return r;
        }, 256);
    }

    private bool Explorar(int f, int c, int profundidad, ResultadoLaberinto r)
    {
        r.Llamadas++;
        if (profundidad > r.ProfundidadMaxima) r.ProfundidadMaxima = profundidad;

        r.Visitado[f, c] = true;
        r.Camino.Add((f, c));                          // se prueba esta celda como parte del camino

        if (f == Filas - 1 && c == Columnas - 1)       // CASO BASE: se llego a la meta
            return true;

        for (int d = 0; d < 4; d++)
        {
            if (Muros[f, c, d]) continue;
            int nf = f + DF[d], nc = c + DC[d];
            if (nf < 0 || nc < 0 || nf >= Filas || nc >= Columnas || r.Visitado[nf, nc]) continue;

            if (Explorar(nf, nc, profundidad + 1, r))  // CASO RECURSIVO: intentar por ese lado
                return true;
        }

        r.Camino.RemoveAt(r.Camino.Count - 1);         // BACKTRACKING: callejon sin salida, se deshace el paso
        r.Retrocesos++;
        return false;
    }
}