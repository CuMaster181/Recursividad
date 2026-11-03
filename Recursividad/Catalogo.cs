namespace Unidad2;

/// <summary>Resultado de ejecutar un ejercicio del catalogo.</summary>
public record ResultadoCatalogo(string Iterativo, string Recursivo, string Resumen, List<string> Traza);

/// <summary>Descripcion de un ejercicio del catalogo (para mostrarlo en la interfaz).</summary>
public class EjercicioCatalogo
{
    public string Nombre { get; }
    public string Descripcion { get; }
    public string CasoBase { get; }
    public string CasoRecursivo { get; }
    public string EntradaEjemplo { get; }
    public string AyudaEntrada { get; }
    public Func<string, ResultadoCatalogo> Ejecutar { get; }

    public EjercicioCatalogo(string nombre, string descripcion, string casoBase, string casoRecursivo,
                             string entradaEjemplo, string ayudaEntrada, Func<string, ResultadoCatalogo> ejecutar)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        CasoBase = casoBase;
        CasoRecursivo = casoRecursivo;
        EntradaEjemplo = entradaEjemplo;
        AyudaEntrada = ayudaEntrada;
        Ejecutar = ejecutar;
    }

    public override string ToString() => Nombre;
}

/// <summary>
/// TAREA 2.2 - Catalogo de ejercicios: conversion de iterativo a recursivo.
///
/// Cada ejercicio tiene una version ITERATIVA (con ciclos) y una RECURSIVA.
/// En cada version recursiva se identifica:
///   - CASO BASE: condicion de salida, la llamada que ya no se descompone.
///   - CASO RECURSIVO: la llamada a si misma con un problema mas pequeno.
/// Los parametros 'Trazador t' y 'int nivel' solo sirven para registrar la
/// prueba de escritorio en pantalla; no forman parte del algoritmo.
/// </summary>
public static class Catalogo
{
    // <ej:Factorial>
    public static long FactorialIterativo(int n)
    {
        long resultado = 1;
        for (int i = 2; i <= n; i++)
            resultado *= i;
        return resultado;
    }

    public static long FactorialRecursivo(int n, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Factorial({n})");
        if (n <= 1)                                              // CASO BASE
        {
            t.Salir(nivel, $"Factorial({n}) = 1   [CASO BASE]");
            return 1;
        }
        long r = n * FactorialRecursivo(n - 1, t, nivel + 1);    // CASO RECURSIVO
        t.Salir(nivel, $"Factorial({n}) = {n} * Factorial({n - 1}) = {r}");
        return r;
    }
    // </ej:Factorial>

    // <ej:Fibonacci>
    public static long FibonacciIterativo(int n)
    {
        if (n <= 1) return n;
        long anterior = 0, actual = 1;
        for (int i = 2; i <= n; i++)
        {
            long siguiente = anterior + actual;
            anterior = actual;
            actual = siguiente;
        }
        return actual;
    }

    public static long FibonacciRecursivo(int n, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Fib({n})");
        if (n <= 1)                                              // CASO BASE
        {
            t.Salir(nivel, $"Fib({n}) = {n}   [CASO BASE]");
            return n;
        }
        long r = FibonacciRecursivo(n - 1, t, nivel + 1)         // CASO RECURSIVO
               + FibonacciRecursivo(n - 2, t, nivel + 1);
        t.Salir(nivel, $"Fib({n}) = Fib({n - 1}) + Fib({n - 2}) = {r}");
        return r;
    }
    // </ej:Fibonacci>

    // <ej:Hanoi>
    public static List<string> HanoiIterativo(int n)
    {
        var movs = new List<string>();
        var torres = new List<int>[] { new(), new(), new() };    // 0 = A, 1 = B, 2 = C
        for (int d = n; d >= 1; d--) torres[0].Add(d);
        char[] nombre = { 'A', 'B', 'C' };

        // Orden ciclico de pares de torres segun la paridad de n
        int[][] pares = (n % 2 == 1)
            ? new[] { new[] { 0, 2 }, new[] { 0, 1 }, new[] { 1, 2 } }
            : new[] { new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 2 } };

        long total = (1L << n) - 1;
        for (long i = 1; i <= total; i++)
        {
            int a = pares[(i - 1) % 3][0], b = pares[(i - 1) % 3][1];
            int origen, destino;
            if (torres[a].Count == 0) { origen = b; destino = a; }
            else if (torres[b].Count == 0) { origen = a; destino = b; }
            else if (torres[a][^1] < torres[b][^1]) { origen = a; destino = b; }
            else { origen = b; destino = a; }

            int disco = torres[origen][^1];
            torres[origen].RemoveAt(torres[origen].Count - 1);
            torres[destino].Add(disco);
            movs.Add($"Disco {disco}: {nombre[origen]} -> {nombre[destino]}");
        }
        return movs;
    }

    public static List<string> HanoiRecursivo(int n, Trazador t)
    {
        var movs = new List<string>();
        MoverRec(n, 'A', 'C', 'B', movs, t, 0);
        return movs;
    }

    private static void MoverRec(int n, char origen, char destino, char auxiliar,
                                 List<string> movs, Trazador t, int nivel)
    {
        t.Entrar(nivel, $"Hanoi({n}, {origen} -> {destino}, aux {auxiliar})");
        if (n == 1)                                              // CASO BASE: un solo disco
        {
            movs.Add($"Disco 1: {origen} -> {destino}");
            t.Nota(nivel, $"Mover disco 1: {origen} -> {destino}   [CASO BASE]");
            t.Salir(nivel, $"Hanoi(1) terminado");
            return;
        }
        MoverRec(n - 1, origen, auxiliar, destino, movs, t, nivel + 1);   // CASO RECURSIVO (1)
        movs.Add($"Disco {n}: {origen} -> {destino}");
        t.Nota(nivel, $"Mover disco {n}: {origen} -> {destino}");
        MoverRec(n - 1, auxiliar, destino, origen, movs, t, nivel + 1);   // CASO RECURSIVO (2)
        t.Salir(nivel, $"Hanoi({n}) terminado");
    }
    // </ej:Hanoi>

    // <ej:SumaArreglo>
    public static int SumaIterativa(int[] a)
    {
        int suma = 0;
        for (int i = 0; i < a.Length; i++)
            suma += a[i];
        return suma;
    }

    public static int SumaRecursiva(int[] a, int i, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Suma(i={i})");
        if (i == a.Length)                                       // CASO BASE: ya no hay elementos
        {
            t.Salir(nivel, $"Suma(i={i}) = 0   [CASO BASE]");
            return 0;
        }
        int resto = SumaRecursiva(a, i + 1, t, nivel + 1);       // CASO RECURSIVO
        int r = a[i] + resto;
        t.Salir(nivel, $"Suma(i={i}) = a[{i}] + Suma(i={i + 1}) = {a[i]} + {resto} = {r}");
        return r;
    }
    // </ej:SumaArreglo>

    // <ej:BusquedaLineal>
    public static int BusquedaIterativa(int[] a, int x)
    {
        for (int i = 0; i < a.Length; i++)
            if (a[i] == x) return i;
        return -1;
    }

    public static int BusquedaRecursiva(int[] a, int x, int i, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Buscar({x}, i={i})");
        if (i >= a.Length)                                       // CASO BASE 1: se acabo el arreglo
        {
            t.Salir(nivel, $"Buscar({x}, i={i}) = -1   [CASO BASE: no encontrado]");
            return -1;
        }
        if (a[i] == x)                                           // CASO BASE 2: encontrado
        {
            t.Salir(nivel, $"Buscar({x}, i={i}) = {i}   [CASO BASE: a[{i}] = {x}]");
            return i;
        }
        int r = BusquedaRecursiva(a, x, i + 1, t, nivel + 1);    // CASO RECURSIVO
        t.Salir(nivel, $"Buscar({x}, i={i}) = {r}");
        return r;
    }
    // </ej:BusquedaLineal>

    // <ej:Potencia>
    public static long PotenciaIterativa(int b, int e)
    {
        long r = 1;
        for (int i = 0; i < e; i++)
            r = checked(r * b);
        return r;
    }

    public static long PotenciaRecursiva(int b, int e, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Pot({b}, {e})");
        if (e == 0)                                              // CASO BASE: x^0 = 1
        {
            t.Salir(nivel, $"Pot({b}, 0) = 1   [CASO BASE]");
            return 1;
        }
        long r = checked(b * PotenciaRecursiva(b, e - 1, t, nivel + 1));  // CASO RECURSIVO
        t.Salir(nivel, $"Pot({b}, {e}) = {b} * Pot({b}, {e - 1}) = {r}");
        return r;
    }
    // </ej:Potencia>

    // <ej:MCD>
    public static int McdIterativo(int a, int b)
    {
        while (b != 0)
        {
            int resto = a % b;
            a = b;
            b = resto;
        }
        return a;
    }

    public static int McdRecursivo(int a, int b, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"MCD({a}, {b})");
        if (b == 0)                                              // CASO BASE
        {
            t.Salir(nivel, $"MCD({a}, 0) = {a}   [CASO BASE]");
            return a;
        }
        int r = McdRecursivo(b, a % b, t, nivel + 1);            // CASO RECURSIVO
        t.Salir(nivel, $"MCD({a}, {b}) = MCD({b}, {a % b}) = {r}");
        return r;
    }
    // </ej:MCD>

    // <ej:InvertirCadena>
    public static string InvertirIterativo(string s)
    {
        string resultado = "";
        for (int i = s.Length - 1; i >= 0; i--)
            resultado += s[i];
        return resultado;
    }

    public static string InvertirRecursivo(string s, Trazador t, int nivel = 0)
    {
        t.Entrar(nivel, $"Invertir(\"{s}\")");
        if (s.Length <= 1)                                       // CASO BASE
        {
            t.Salir(nivel, $"Invertir(\"{s}\") = \"{s}\"   [CASO BASE]");
            return s;
        }
        string r = InvertirRecursivo(s.Substring(1), t, nivel + 1) + s[0];   // CASO RECURSIVO
        t.Salir(nivel, $"Invertir(\"{s}\") = \"{r}\"");
        return r;
    }
    // </ej:InvertirCadena>

    // ------------------------------------------------------------------
    // Utilidades para leer las entradas de la interfaz
    // ------------------------------------------------------------------

    private static int Entero(string texto, int min, int max, string nombre)
    {
        if (!int.TryParse(texto.Trim(), out int v))
            throw new FormatException($"'{texto}' no es un numero entero valido para {nombre}.");
        if (v < min || v > max)
            throw new ArgumentException($"{nombre} debe estar entre {min} y {max}.");
        return v;
    }

    private static int[] Enteros(string texto)
    {
        var partes = texto.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0) throw new FormatException("Escribe al menos un numero.");
        if (partes.Length > 30) throw new ArgumentException("Maximo 30 elementos.");
        var r = new int[partes.Length];
        for (int i = 0; i < partes.Length; i++)
        {
            if (!int.TryParse(partes[i], out r[i]))
                throw new FormatException($"'{partes[i]}' no es un numero entero valido.");
        }
        return r;
    }

    private static string Resumir(List<string> movs)
    {
        int mostrar = Math.Min(movs.Count, 12);
        string txt = string.Join(" | ", movs.Take(mostrar));
        return movs.Count > mostrar ? txt + $" | ... ({movs.Count} movimientos en total)" : txt;
    }

    private static ResultadoCatalogo Armar(string iterativo, string recursivo, Trazador t,
                                            bool? iguales = null, string extra = "")
    {
        var traza = new List<string>(t.Lineas);
        if (t.Truncado) traza.Add("... (traza truncada: demasiadas llamadas para mostrarlas todas)");
        string coincide = (iguales ?? iterativo == recursivo) ? "SI" : "NO";
        string resumen = $"Llamadas recursivas: {t.Llamadas:N0} | Profundidad maxima de la pila: {t.ProfundidadMaxima} | " +
                         $"Resultados iguales: {coincide}{extra}";
        return new ResultadoCatalogo(iterativo, recursivo, resumen, traza);
    }

    // ------------------------------------------------------------------
    // Registro de ejercicios mostrados en la interfaz
    // ------------------------------------------------------------------

    public static readonly List<EjercicioCatalogo> Todos = new()
    {
        new EjercicioCatalogo("1. Factorial",
            "n! = n * (n-1)!   con 0! = 1",
            "n <= 1  ->  devuelve 1",
            "n * Factorial(n - 1)",
            "5", "Un entero n entre 0 y 20",
            entrada =>
            {
                int n = Entero(entrada, 0, 20, "n");
                var t = new Trazador();
                long rec = FactorialRecursivo(n, t);
                long it = FactorialIterativo(n);
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("2. Fibonacci",
            "Fib(n) = Fib(n-1) + Fib(n-2)   con Fib(0) = 0 y Fib(1) = 1",
            "n <= 1  ->  devuelve n",
            "Fib(n - 1) + Fib(n - 2)",
            "6", "Un entero n entre 0 y 30 (la version recursiva crece exponencialmente)",
            entrada =>
            {
                int n = Entero(entrada, 0, 30, "n");
                var t = new Trazador();
                long rec = FibonacciRecursivo(n, t);
                long it = FibonacciIterativo(n);
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("3. Torres de Hanoi",
            "Mover n discos de la torre A a la C usando B, sin poner un disco grande sobre uno pequeno.",
            "n == 1  ->  mover el unico disco directamente",
            "Hanoi(n-1, A->B), mover disco n, Hanoi(n-1, B->C)",
            "3", "Numero de discos n entre 1 y 10",
            entrada =>
            {
                int n = Entero(entrada, 1, 10, "n");
                var t = new Trazador();
                var rec = HanoiRecursivo(n, t);
                var it = HanoiIterativo(n);
                bool iguales = rec.SequenceEqual(it);
                return Armar(Resumir(it), Resumir(rec), t, iguales, $" | Movimientos: {rec.Count} (2^{n} - 1)");
            }),

        new EjercicioCatalogo("4. Suma de arreglo",
            "Suma de todos los elementos de un arreglo.",
            "i == longitud del arreglo  ->  devuelve 0",
            "a[i] + Suma(a, i + 1)",
            "4, 8, 15, 16, 23", "Numeros enteros separados por coma (max. 30)",
            entrada =>
            {
                int[] a = Enteros(entrada);
                var t = new Trazador();
                int rec = SumaRecursiva(a, 0, t);
                int it = SumaIterativa(a);
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("5. Busqueda lineal",
            "Buscar un valor en un arreglo y devolver su posicion (o -1).",
            "i >= longitud  ->  -1   |   a[i] == x  ->  i",
            "Buscar(a, x, i + 1)",
            "3, 8, 1, 9, 5 ; 9", "Arreglo y valor separados por punto y coma:  3, 8, 1 ; 8",
            entrada =>
            {
                var partes = entrada.Split(';');
                if (partes.Length != 2) throw new FormatException("Usa el formato:  3, 8, 1, 9 ; 9");
                int[] a = Enteros(partes[0]);
                int x = Entero(partes[1], int.MinValue, int.MaxValue, "el valor buscado");
                var t = new Trazador();
                int rec = BusquedaRecursiva(a, x, 0, t);
                int it = BusquedaIterativa(a, x);
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("6. Potencia",
            "b^e calculado con multiplicaciones sucesivas.",
            "e == 0  ->  devuelve 1",
            "b * Pot(b, e - 1)",
            "2, 10", "Base y exponente separados por coma (base -20..20, exponente 0..40)",
            entrada =>
            {
                int[] v = Enteros(entrada);
                if (v.Length != 2) throw new FormatException("Escribe dos numeros: base, exponente");
                if (v[0] < -20 || v[0] > 20) throw new ArgumentException("La base debe estar entre -20 y 20.");
                if (v[1] < 0 || v[1] > 40) throw new ArgumentException("El exponente debe estar entre 0 y 40.");
                var t = new Trazador();
                long rec, it;
                try
                {
                    rec = PotenciaRecursiva(v[0], v[1], t);
                    it = PotenciaIterativa(v[0], v[1]);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException("El resultado no cabe en un long (desbordamiento numerico).");
                }
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("7. MCD (Euclides)",
            "Maximo comun divisor con el algoritmo de Euclides.",
            "b == 0  ->  devuelve a",
            "MCD(b, a % b)",
            "48, 18", "Dos enteros positivos separados por coma",
            entrada =>
            {
                int[] v = Enteros(entrada);
                if (v.Length != 2) throw new FormatException("Escribe dos numeros: a, b");
                if (v[0] < 0 || v[1] < 0) throw new ArgumentException("Usa numeros no negativos.");
                var t = new Trazador();
                int rec = McdRecursivo(v[0], v[1], t);
                int it = McdIterativo(v[0], v[1]);
                return Armar(it.ToString(), rec.ToString(), t);
            }),

        new EjercicioCatalogo("8. Invertir cadena",
            "Devuelve el texto escrito al reves.",
            "longitud <= 1  ->  devuelve la misma cadena",
            "Invertir(s.Substring(1)) + s[0]",
            "recursion", "Un texto de 1 a 40 caracteres",
            entrada =>
            {
                string s = entrada.Trim();
                if (s.Length < 1 || s.Length > 40) throw new ArgumentException("El texto debe tener entre 1 y 40 caracteres.");
                var t = new Trazador();
                string rec = InvertirRecursivo(s, t);
                string it = InvertirIterativo(s);
                return Armar(it, rec, t);
            }),
    };
}