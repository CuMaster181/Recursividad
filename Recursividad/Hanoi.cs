namespace Unidad2;

public record Movimiento(int Disco, int Origen, int Destino);

public class EstadisticasHanoi
{
    public long Movimientos;
    public long Llamadas;
    public int ProfundidadMaxima;
}

/// <summary>
/// TAREA 2.3 - Torres de Hanoi (problema complejo resuelto con recursividad).
/// Torres: 0 = A, 1 = B, 2 = C.
/// </summary>
public static class Hanoi
{
    /// <summary>Devuelve la lista de movimientos para pasar n discos de A a C.</summary>
    public static List<Movimiento> Resolver(int n)
    {
        var movs = new List<Movimiento>();
        Mover(n, 0, 2, 1, movs);
        return movs;
    }

    private static void Mover(int n, int origen, int destino, int auxiliar, List<Movimiento> movs)
    {
        if (n == 1)                                       // CASO BASE: un solo disco
        {
            movs.Add(new Movimiento(1, origen, destino));
            return;
        }
        Mover(n - 1, origen, auxiliar, destino, movs);    // CASO RECURSIVO: n-1 discos a la torre auxiliar
        movs.Add(new Movimiento(n, origen, destino));     // el disco mayor va al destino
        Mover(n - 1, auxiliar, destino, origen, movs);    // n-1 discos del auxiliar al destino
    }

    /// <summary>
    /// Resuelve el problema SIN guardar los movimientos, solo contando
    /// movimientos, llamadas y la profundidad maxima de la pila de llamadas.
    /// </summary>
    public static EstadisticasHanoi Medir(int n)
    {
        var e = new EstadisticasHanoi();
        Contar(n, 1, e);
        return e;
    }

    private static void Contar(int n, int nivel, EstadisticasHanoi e)
    {
        e.Llamadas++;
        if (nivel > e.ProfundidadMaxima) e.ProfundidadMaxima = nivel;

        if (n == 1)                                       // CASO BASE
        {
            e.Movimientos++;
            return;
        }
        Contar(n - 1, nivel + 1, e);
        e.Movimientos++;
        Contar(n - 1, nivel + 1, e);
    }
}