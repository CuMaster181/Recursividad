using System.Runtime.ExceptionServices;

namespace Unidad2;

public static class Utilidades
{
    /// <summary>
    /// Ejecuta una funcion en un hilo nuevo con el tamano de pila indicado (en MB).
    /// La recursividad profunda consume pila (call stack); con este metodo se puede
    /// observar como cambia el comportamiento segun el tamano de pila disponible.
    /// </summary>
    public static T EjecutarConPila<T>(Func<T> funcion, int megabytes)
    {
        T resultado = default!;
        Exception? error = null;

        var hilo = new Thread(() =>
        {
            try { resultado = funcion(); }
            catch (Exception ex) { error = ex; }
        }, megabytes * 1024 * 1024);

        hilo.Start();
        hilo.Join();

        if (error != null)
            ExceptionDispatchInfo.Capture(error).Throw();
        return resultado;
    }

    /// <summary>Crea un TextBox de solo lectura con fuente monoespaciada.</summary>
    public static TextBox CajaTexto(bool soloLectura = true)
    {
        return new TextBox
        {
            Multiline = true,
            ReadOnly = soloLectura,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Font = new Font("Consolas", 10),
            BackColor = Color.White,
            Dock = DockStyle.Fill
        };
    }

    /// <summary>Crea una tabla (DataGridView) de solo lectura.</summary>
    public static DataGridView Tabla(params string[] columnas)
    {
        var t = new DataGridView
        {
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            RowHeadersVisible = false,
            Dock = DockStyle.Fill
        };
        foreach (string c in columnas) t.Columns.Add(c, c);
        return t;
    }
}

/// <summary>Panel sin parpadeo para dibujar con GDI+.</summary>
public class LienzoPanel : Panel
{
    public LienzoPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }
}