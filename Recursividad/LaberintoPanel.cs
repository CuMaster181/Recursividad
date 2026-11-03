namespace Unidad2;

/// <summary>Pestana de la Tarea 2.3: laberinto con backtracking recursivo.</summary>
public class LaberintoPanel : UserControl
{
    private readonly NumericUpDown _numFilas = new() { Minimum = 5, Maximum = 100, Value = 15, Width = 60 };
    private readonly NumericUpDown _numCols = new() { Minimum = 5, Maximum = 100, Value = 25, Width = 60 };
    private readonly Button _btnGenerar = new() { Text = "Generar laberinto", AutoSize = true };
    private readonly Button _btnResolver = new() { Text = "Resolver (backtracking)", AutoSize = true };
    private readonly Button _btnMedir = new() { Text = "Medir tamanos 10, 20, 40, 80 y 100", AutoSize = true };
    private readonly Label _estado = new() { Text = "Genera un laberinto para empezar.", AutoSize = true, Padding = new Padding(0, 6, 0, 0) };
    private readonly LienzoPanel _lienzo = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _tabla = Utilidades.Tabla("Tamano", "Celdas", "Llamadas", "Profundidad maxima",
                                                           "Retrocesos", "Longitud del camino", "Tiempo (ms)");

    private Laberinto? _lab;
    private ResultadoLaberinto? _res;
    private int _semilla = 42;

    public LaberintoPanel()
    {
        Dock = DockStyle.Fill;

        var controles = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(6), WrapContents = true };
        controles.Controls.Add(new Label { Text = "Filas:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        controles.Controls.Add(_numFilas);
        controles.Controls.Add(new Label { Text = "Columnas:", AutoSize = true, Padding = new Padding(8, 6, 0, 0) });
        controles.Controls.Add(_numCols);
        controles.Controls.Add(_btnGenerar);
        controles.Controls.Add(_btnResolver);
        controles.Controls.Add(_btnMedir);
        controles.Controls.Add(_estado);

        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        tabla.Controls.Add(controles, 0, 0);
        tabla.Controls.Add(_lienzo, 0, 1);
        tabla.Controls.Add(_tabla, 0, 2);
        Controls.Add(tabla);

        _lienzo.Paint += (_, e) => Dibujar(e.Graphics);
        _btnGenerar.Click += (_, _) => Generar();
        _btnResolver.Click += (_, _) => Resolver();
        _btnMedir.Click += async (_, _) => await MedirAsync();

        Generar();
    }

    private void Generar()
    {
        _semilla = Environment.TickCount;   // un laberinto distinto cada vez
        _lab = Laberinto.Crear((int)_numFilas.Value, (int)_numCols.Value, _semilla);
        _res = null;
        _estado.Text = "Laberinto generado (inicio: verde, meta: rojo). Pulsa 'Resolver'.";
        _lienzo.Invalidate();
    }

    private void Resolver()
    {
        if (_lab == null) return;
        _res = _lab.Resolver();
        _estado.Text = $"Resuelto: {_res.Llamadas:N0} llamadas | profundidad maxima {_res.ProfundidadMaxima:N0} | " +
                       $"{_res.Retrocesos:N0} retrocesos | camino de {_res.LongitudCamino:N0} celdas | {_res.Milisegundos:F3} ms";
        _lienzo.Invalidate();
    }

    private void Dibujar(Graphics g)
    {
        if (_lab == null) return;

        int margen = 10;
        float tam = Math.Min((_lienzo.Width - 2 * margen) / (float)_lab.Columnas,
                             (_lienzo.Height - 2 * margen) / (float)_lab.Filas);
        if (tam < 2) return;

        float x0 = (_lienzo.Width - tam * _lab.Columnas) / 2f;
        float y0 = (_lienzo.Height - tam * _lab.Filas) / 2f;

        // Celdas exploradas (gris claro) y camino final (azul)
        if (_res != null)
        {
            using var gris = new SolidBrush(Color.Gainsboro);
            using var azul = new SolidBrush(Color.LightSkyBlue);
            for (int f = 0; f < _lab.Filas; f++)
                for (int c = 0; c < _lab.Columnas; c++)
                    if (_res.Visitado[f, c]) g.FillRectangle(gris, x0 + c * tam, y0 + f * tam, tam, tam);
            foreach (var (f, c) in _res.Camino)
                g.FillRectangle(azul, x0 + c * tam, y0 + f * tam, tam, tam);
        }

        g.FillRectangle(Brushes.LightGreen, x0, y0, tam, tam);
        g.FillRectangle(Brushes.IndianRed, x0 + (_lab.Columnas - 1) * tam, y0 + (_lab.Filas - 1) * tam, tam, tam);

        // Paredes
        using var lapiz = new Pen(Color.Black, tam > 8 ? 2 : 1);
        for (int f = 0; f < _lab.Filas; f++)
        {
            for (int c = 0; c < _lab.Columnas; c++)
            {
                float x = x0 + c * tam, y = y0 + f * tam;
                if (_lab.Muros[f, c, 0]) g.DrawLine(lapiz, x, y, x + tam, y);
                if (_lab.Muros[f, c, 1]) g.DrawLine(lapiz, x + tam, y, x + tam, y + tam);
                if (_lab.Muros[f, c, 2]) g.DrawLine(lapiz, x, y + tam, x + tam, y + tam);
                if (_lab.Muros[f, c, 3]) g.DrawLine(lapiz, x, y, x, y + tam);
            }
        }
    }

    private async Task MedirAsync()
    {
        _btnMedir.Enabled = _btnGenerar.Enabled = _btnResolver.Enabled = false;
        _tabla.Rows.Clear();
        _estado.Text = "Midiendo...";

        int[] tamanios = { 10, 20, 40, 80, 100 };
        var filas = await Task.Run(() =>
        {
            var lista = new List<object[]>();
            foreach (int n in tamanios)
            {
                var lab = Laberinto.Crear(n, n, 42);   // misma semilla: resultados repetibles
                var r = lab.Resolver();
                lista.Add(new object[]
                {
                    $"{n} x {n}", (n * n).ToString("N0"), r.Llamadas.ToString("N0"),
                    r.ProfundidadMaxima.ToString("N0"), r.Retrocesos.ToString("N0"),
                    r.LongitudCamino.ToString("N0"), r.Milisegundos.ToString("F3")
                });
            }
            return lista;
        });

        foreach (var f in filas) _tabla.Rows.Add(f);
        _estado.Text = "Medicion terminada (semilla fija 42).";
        _btnMedir.Enabled = _btnGenerar.Enabled = _btnResolver.Enabled = true;
    }
}