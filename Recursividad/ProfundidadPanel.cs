namespace Unidad2;

/// <summary>Pestana de la Tarea 2.3: comportamiento segun la profundidad de la recursion.</summary>
public class ProfundidadPanel : UserControl
{
    private readonly ComboBox _comboPila = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };
    private readonly Button _btnMedir = new() { Text = "Medir profundidades", AutoSize = true };
    private readonly Label _estado = new() { Text = "Listo.", AutoSize = true, Padding = new Padding(0, 6, 0, 0) };
    private readonly DataGridView _tabla = Utilidades.Tabla("Profundidad (n)", "Recursivo (ms)", "Iterativo (ms)", "Estado");
    private readonly TextBox _limites = Utilidades.CajaTexto();

    public ProfundidadPanel()
    {
        Dock = DockStyle.Fill;

        _comboPila.Items.AddRange(new object[] { "1", "8", "64" });
        _comboPila.SelectedIndex = 0;

        var controles = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(6), WrapContents = true };
        controles.Controls.Add(new Label { Text = "Tamano de pila del hilo (MB):", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        controles.Controls.Add(_comboPila);
        controles.Controls.Add(_btnMedir);
        controles.Controls.Add(_estado);

        var explicacion = new Label
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(6, 0, 6, 0),
            Text = "Suma 1 + 2 + ... + n de forma recursiva (n llamadas apiladas) frente a un ciclo. " +
                   "Con una pila pequena, una profundidad grande agota la pila: se detecta el desbordamiento y se informa."
        };

        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        tabla.Controls.Add(controles, 0, 0);
        tabla.Controls.Add(explicacion, 0, 1);
        tabla.Controls.Add(_tabla, 0, 2);
        tabla.Controls.Add(_limites, 0, 3);
        Controls.Add(tabla);

        _btnMedir.Click += async (_, _) => await MedirAsync();
    }

    private async Task MedirAsync()
    {
        _btnMedir.Enabled = false;
        _tabla.Rows.Clear();
        _limites.Clear();
        _estado.Text = "Midiendo...";
        int mb = int.Parse((string)_comboPila.SelectedItem!);

        int[] profundidades = { 1_000, 5_000, 10_000, 50_000, 100_000, 500_000 };

        var resultado = await Task.Run(() =>
        {
            var filas = new List<FilaProfundidad>();
            foreach (int p in profundidades) filas.Add(Profundidad.Medir(p, mb));

            var lineas = new List<string> { "Profundidad maxima de recursion alcanzada segun el tamano de pila:" };
            foreach (int m in new[] { 1, 8, 64 })
                lineas.Add($"  Pila de {m,2} MB  ->  aprox. {Profundidad.ProfundidadMaxima(m):N0} llamadas apiladas");
            return (filas, lineas);
        });

        foreach (var f in resultado.filas)
            _tabla.Rows.Add(f.Profundidad.ToString("N0"), f.Recursivo, f.Iterativo, f.Estado);
        _limites.Text = string.Join(Environment.NewLine, resultado.lineas);

        _estado.Text = $"Medicion terminada con pila de {mb} MB.";
        _btnMedir.Enabled = true;
    }
}