namespace Unidad2;

/// <summary>Pestana de la Tarea 2.2: catalogo de ejercicios iterativo vs. recursivo.</summary>
public class CatalogoPanel : UserControl
{
    private readonly ListBox _lista = new() { Dock = DockStyle.Left, Width = 230, Font = new Font("Segoe UI", 11) };
    private readonly TextBox _info = Utilidades.CajaTexto();
    private readonly TextBox _entrada = new() { Width = 320, Font = new Font("Consolas", 11) };
    private readonly Label _ayuda = new() { AutoSize = true, Padding = new Padding(0, 6, 0, 0) };
    private readonly Button _btnEjecutar = new() { Text = "Ejecutar", AutoSize = true };
    private readonly TextBox _resultado = Utilidades.CajaTexto();
    private readonly TextBox _traza = Utilidades.CajaTexto();

    public CatalogoPanel()
    {
        Dock = DockStyle.Fill;

        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6 };
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));   // info del ejercicio
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));    // ayuda de la entrada
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));    // entrada + boton
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));    // resultados
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));    // titulo de la traza
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // traza

        var fila = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 4, 0, 0) };
        fila.Controls.Add(new Label { Text = "Entrada:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        fila.Controls.Add(_entrada);
        fila.Controls.Add(_btnEjecutar);

        var tituloTraza = new Label
        {
            Text = "Prueba de escritorio (pila de llamadas):  '->' entra una llamada, '<-' regresa con su valor",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        tabla.Controls.Add(_info, 0, 0);
        tabla.Controls.Add(_ayuda, 0, 1);
        tabla.Controls.Add(fila, 0, 2);
        tabla.Controls.Add(_resultado, 0, 3);
        tabla.Controls.Add(tituloTraza, 0, 4);
        tabla.Controls.Add(_traza, 0, 5);

        Controls.Add(tabla);   // el control con Dock = Fill se agrega primero
        Controls.Add(_lista);

        foreach (var e in Catalogo.Todos) _lista.Items.Add(e);
        _lista.SelectedIndexChanged += (_, _) => MostrarEjercicio();
        _btnEjecutar.Click += (_, _) => Ejecutar();
        _entrada.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Ejecutar(); }
        };
        _lista.SelectedIndex = 0;
    }

    private EjercicioCatalogo? Actual => _lista.SelectedItem as EjercicioCatalogo;

    private void MostrarEjercicio()
    {
        var ej = Actual;
        if (ej == null) return;
        _info.Text =
            "EJERCICIO : " + ej.Nombre + Environment.NewLine +
            "Definicion: " + ej.Descripcion + Environment.NewLine +
            "CASO BASE : " + ej.CasoBase + Environment.NewLine +
            "CASO RECURSIVO: " + ej.CasoRecursivo;
        _ayuda.Text = "Formato de entrada: " + ej.AyudaEntrada;
        _entrada.Text = ej.EntradaEjemplo;
        _resultado.Clear();
        _traza.Clear();
    }

    private void Ejecutar()
    {
        var ej = Actual;
        if (ej == null) return;

        try
        {
            ResultadoCatalogo r = ej.Ejecutar(_entrada.Text);
            _resultado.Text =
                "Iterativo : " + r.Iterativo + Environment.NewLine +
                "Recursivo : " + r.Recursivo + Environment.NewLine +
                r.Resumen;
            _traza.Text = string.Join(Environment.NewLine, r.Traza);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Entrada no valida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}