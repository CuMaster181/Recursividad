namespace Unidad2;

/// <summary>
/// Registra las llamadas recursivas (la "pila de llamadas") para construir la
/// prueba de escritorio. Cada nivel de recursion se muestra con una sangria mayor.
/// </summary>
public class Trazador
{
    private readonly List<string> _lineas = new();
    private readonly int _maxLineas;

    public long Llamadas { get; private set; }
    public int ProfundidadMaxima { get; private set; }
    public bool Truncado { get; private set; }
    public IReadOnlyList<string> Lineas => _lineas;

    public Trazador(int maxLineas = 400) { _maxLineas = maxLineas; }

    /// <summary>Se llama al ENTRAR a una llamada recursiva (se apila un marco).</summary>
    public void Entrar(int nivel, string texto)
    {
        Llamadas++;
        if (nivel + 1 > ProfundidadMaxima) ProfundidadMaxima = nivel + 1;
        Agregar(nivel, "-> " + texto);
    }

    /// <summary>Se llama al SALIR de una llamada (se desapila y se devuelve un valor).</summary>
    public void Salir(int nivel, string texto) => Agregar(nivel, "<- " + texto);

    /// <summary>Anotacion intermedia (por ejemplo, un movimiento realizado).</summary>
    public void Nota(int nivel, string texto) => Agregar(nivel, "   * " + texto);

    private void Agregar(int nivel, string texto)
    {
        if (_lineas.Count >= _maxLineas) { Truncado = true; return; }
        _lineas.Add(new string(' ', nivel * 2) + texto);
    }
}