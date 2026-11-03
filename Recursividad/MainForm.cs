namespace Unidad2;

/// <summary>Ventana principal: una pestana por cada parte de la practica.</summary>
public partial class MainForm : Form
{
    public MainForm()
    {
        Text = "Unidad 2 - Recursividad (Tareas 2.2 y 2.3)";
        Width = 1100;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;

        var pestanas = new TabControl { Dock = DockStyle.Fill };
        AgregarPestana(pestanas, "2.2  Catalogo iterativo -> recursivo", new CatalogoPanel());
        AgregarPestana(pestanas, "2.3  Torres de Hanoi", new HanoiPanel());
        AgregarPestana(pestanas, "2.3  Laberinto (backtracking)", new LaberintoPanel());
        AgregarPestana(pestanas, "2.3  Profundidad de llamadas", new ProfundidadPanel());
        Controls.Add(pestanas);
    }

    private static void AgregarPestana(TabControl tabs, string titulo, Control contenido)
    {
        var pagina = new TabPage(titulo);
        contenido.Dock = DockStyle.Fill;
        pagina.Controls.Add(contenido);
        tabs.TabPages.Add(pagina);
    }
}