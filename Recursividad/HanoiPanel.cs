using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Unidad2;

/// <summary>Pestana de la Tarea 2.3: simulacion grafica de Torres de Hanoi y medicion.</summary>
public class HanoiPanel : UserControl
{
    private static readonly Color[] Colores =
    {
        Color.Crimson, Color.OrangeRed, Color.Orange, Color.Gold, Color.YellowGreen,
        Color.MediumSeaGreen, Color.Teal, Color.DodgerBlue, Color.SlateBlue, Color.MediumOrchid
    };

    private readonly NumericUpDown _numDiscos = new() { Minimum = 1, Maximum = 10, Value = 4, Width = 55 };
    private readonly TrackBar _velocidad = new() { Minimum = 1, Maximum = 10, Value = 5, Width = 140, AutoSize = false, Height = 28 };
    private readonly Button _btnResolver = new() { Text = "Resolver (animar)", AutoSize = true };
    private readonly Button _btnReiniciar = new() { Text = "Reiniciar", AutoSize = true };
    private readonly Button _btnMedir = new() { Text = "Medir 5, 10, 15, 20 y 25 discos", AutoSize = true };
    private readonly Label _estado = new() { Text = "Listo.", AutoSize = true, Padding = new Padding(0, 6, 0, 0) };
    private readonly LienzoPanel _lienzo = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _tabla = Utilidades.Tabla("Discos", "Movimientos (2^n - 1)", "Llamadas recursivas",
                                                           "Profundidad maxima de la pila", "Tiempo (ms)");
    private readonly System.Windows.Forms.Timer _timer = new();

    private List<int>[] _torres = { new(), new(), new() };
    private List<Movimiento> _movs = new();
    private int _indice;
    private int _n = 4;

    public HanoiPanel()
    {
        Dock = DockStyle.Fill;

        var controles = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(6), WrapContents = true };
        controles.Controls.Add(new Label { Text = "Discos:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        controles.Controls.Add(_numDiscos);
        controles.Controls.Add(new Label { Text = "Velocidad:", AutoSize = true, Padding = new Padding(10, 6, 0, 0) });
        controles.Controls.Add(_velocidad);
        controles.Controls.Add(_btnResolver);
        controles.Controls.Add(_btnReiniciar);
        controles.Controls.Add(_btnMedir);
        controles.Controls.Add(_estado);

        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        tabla.Controls.Add(controles, 0, 0);
        tabla.Controls.Add(_lienzo, 0, 1);
        tabla.Controls.Add(_tabla, 0, 2);
        Controls.Add(tabla);

        _lienzo.Paint += (_, e) => Dibujar(e.Graphics);
        _timer.Tick += (_, _) => Paso();
        _btnResolver.Click += (_, _) => Resolver();
        _btnReiniciar.Click += (_, _) => Reiniciar();
        _numDiscos.ValueChanged += (_, _) => Reiniciar();
        _btnMedir.Click += async (_, _) => await MedirAsync();

        Reiniciar();
    }

    private void Reiniciar()
    {
        _timer.Stop();
        _n = (int)_numDiscos.Value;
        _torres = new[] { new List<int>(), new List<int>(), new List<int>() };
        for (int d = _n; d >= 1; d--) _torres[0].Add(d);
        _movs = new List<Movimiento>();
        _indice = 0;
        _btnResolver.Enabled = true;
        _estado.Text = "Listo. Pulsa 'Resolver' para ver la solucion recursiva.";
        _lienzo.Invalidate();
    }

    private void Resolver()
    {
        Reiniciar();
        _movs = Hanoi.Resolver(_n);
        _indice = 0;
        _btnResolver.Enabled = false;
        // velocidad 1 = 1000 ms por movimiento ... velocidad 10 = 2 ms
        _timer.Interval = Math.Max(1, (int)(1000 / Math.Pow(2, _velocidad.Value - 1)));
        _timer.Start();
    }

    private void Paso()
    {
        if (_indice >= _movs.Count)
        {
            _timer.Stop();
            _estado.Text = $"Terminado: {_movs.Count} movimientos (2^{_n} - 1 = {(1 << _n) - 1}).";
            _btnResolver.Enabled = true;
            return;
        }
        Movimiento m = _movs[_indice++];
        var origen = _torres[m.Origen];
        origen.RemoveAt(origen.Count - 1);
        _torres[m.Destino].Add(m.Disco);
        _estado.Text = $"Movimiento {_indice} de {_movs.Count}: disco {m.Disco}  {"ABC"[m.Origen]} -> {"ABC"[m.Destino]}";
        _lienzo.Invalidate();
    }

    private void Dibujar(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int w = _lienzo.Width, h = _lienzo.Height;
        if (w < 100 || h < 100) return;

        int baseY = h - 28;
        int anchoTorre = w / 3;
        int maxDiscos = Math.Max(1, _n);
        int altoDisco = Math.Max(6, Math.Min(26, (baseY - 30) / (maxDiscos + 1)));
        int maxAncho = anchoTorre - 40;
        int altoPoste = (maxDiscos + 1) * altoDisco;

        using var fuente = new Font("Segoe UI", 12, FontStyle.Bold);
        for (int t = 0; t < 3; t++)
        {
            int cx = anchoTorre * t + anchoTorre / 2;
            g.FillRectangle(Brushes.SaddleBrown, cx - 4, baseY - altoPoste, 8, altoPoste);
            g.FillRectangle(Brushes.DimGray, cx - maxAncho / 2 - 6, baseY, maxAncho + 12, 8);
            g.DrawString("ABC"[t].ToString(), fuente, Brushes.Black, cx - 8, baseY + 8);

            for (int i = 0; i < _torres[t].Count; i++)
            {
                int disco = _torres[t][i];
                int ancho = 24 + (maxAncho - 24) * disco / maxDiscos;
                var rect = new Rectangle(cx - ancho / 2, baseY - (i + 1) * altoDisco, ancho, altoDisco - 2);
                using var pincel = new SolidBrush(Colores[(disco - 1) % Colores.Length]);
                g.FillRectangle(pincel, rect);
                g.DrawRectangle(Pens.Black, rect);
            }
        }
    }

    private async Task MedirAsync()
    {
        _btnMedir.Enabled = false;
        _btnResolver.Enabled = false;
        _tabla.Rows.Clear();
        _estado.Text = "Midiendo... (25 discos son 33 millones de movimientos)";

        int[] tamanios = { 5, 10, 15, 20, 25 };
        var filas = await Task.Run(() =>
        {
            var lista = new List<object[]>();
            foreach (int n in tamanios)
            {
                var sw = Stopwatch.StartNew();
                EstadisticasHanoi e = Hanoi.Medir(n);
                sw.Stop();
                lista.Add(new object[]
                {
                    n, e.Movimientos.ToString("N0"), e.Llamadas.ToString("N0"),
                    e.ProfundidadMaxima, sw.Elapsed.TotalMilliseconds.ToString("F3")
                });
            }
            return lista;
        });

        foreach (var f in filas) _tabla.Rows.Add(f);
        _estado.Text = "Medicion terminada. La pila solo crece n niveles, pero el tiempo crece como 2^n.";
        _btnMedir.Enabled = true;
        _btnResolver.Enabled = true;
    }
}