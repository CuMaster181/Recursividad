namespace Unidad2;

internal static class Program
{
    /// <summary>Punto de entrada de la aplicacion Windows Forms.</summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}