using System;
using MySql.Data.MySqlClient;

namespace Northwind.Catalogo
{
    internal static class BusquedaProductos
    {
        // Orden por precio. Para la comparación se cambia a "ASC"
        // y, para la entrega, se restaura a "ASC".
        private const string OrdenPrecio = "ASC";

        // Para el caso 8 se cambia temporalmente a
        // "CAST(NULL AS CHAR) AS QuantityPerUnit" y después se restaura.
        private const string ColumnaPresentacion = "QuantityPerUnit";

        public static void BuscarPorPrecioYExistencias(MySqlConnection conexion)
        {
            Console.WriteLine();
            Console.WriteLine("================================================================");
            Console.WriteLine("BÚSQUEDA POR PRECIO MÁXIMO Y EXISTENCIAS MÍNIMAS");
            Console.WriteLine("================================================================");

            decimal precioMaximo = LeerDecimalNoNegativo("Precio máximo por unidad: ");
            int existenciasMinimas = LeerEnteroNoNegativo("Existencias mínimas: ");

            string consulta =
                "SELECT ProductID, ProductName, UnitPrice, UnitsInStock, " + ColumnaPresentacion + " " +
                "FROM Products " +
                "WHERE UnitPrice <= @precioMaximo " +
                "AND UnitsInStock >= @existenciasMinimas " +
                "AND Discontinued = 0 " +
                "ORDER BY UnitPrice " + OrdenPrecio + ", ProductID ASC;";

            int filasMostradas = 0;

            try
            {
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@precioMaximo", precioMaximo);
                    comando.Parameters.AddWithValue("@existenciasMinimas", existenciasMinimas);

                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            int id = lector.GetInt32(0);
                            string nombre = lector.GetString(1);
                            decimal precio = lector.GetDecimal(2);
                            int existencias = lector.GetInt32(3);
                            string presentacion = lector.IsDBNull(4)
                                ? "Presentación sin registrar"
                                : lector.GetString(4);

                            Console.WriteLine(
                                id + " | " + nombre + " | " + precio.ToString("F2") + " | " +
                                existencias + " | " + presentacion);
                            filasMostradas++;
                        }
                    }
                }

                if (filasMostradas == 0)
                {
                    Console.WriteLine("No hay productos que cumplan los filtros");
                }

                // El lector anterior ya se liberó: ahora se ejecuta el conteo.
                string consultaConteo =
                    "SELECT COUNT(*) " +
                    "FROM Products " +
                    "WHERE UnitPrice <= @precioMaximo " +
                    "AND UnitsInStock >= @existenciasMinimas " +
                    "AND Discontinued = 0;";

                using (MySqlCommand comandoConteo = new MySqlCommand(consultaConteo, conexion))
                {
                    comandoConteo.Parameters.AddWithValue("@precioMaximo", precioMaximo);
                    comandoConteo.Parameters.AddWithValue("@existenciasMinimas", existenciasMinimas);

                    long total = Convert.ToInt64(comandoConteo.ExecuteScalar());
                    Console.WriteLine("Filas mostradas: " + filasMostradas);
                    Console.WriteLine("Total según COUNT(*): " + total);
                    Console.WriteLine(total == filasMostradas
                        ? "El conteo coincide con el listado."
                        : "ATENCIÓN: el conteo no coincide con el listado.");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de MySQL (" + ex.Number + "): " + ex.Message);
            }
        }

        public static void BuscarPorNombre(MySqlConnection conexion)
        {
            Console.WriteLine();
            Console.WriteLine("================================================================");
            Console.WriteLine("BÚSQUEDA POR PARTE DEL NOMBRE");
            Console.WriteLine("================================================================");

            string fragmento = LeerTextoNoVacio("Fragmento del nombre del producto: ");

            string consulta =
                "SELECT ProductID, ProductName, UnitPrice, UnitsInStock " +
                "FROM Products " +
                "WHERE Discontinued = 0 " +
                "AND ProductName LIKE @patron " +
                "ORDER BY ProductName ASC, ProductID ASC;";

            int filasMostradas = 0;

            try
            {
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                {
                    // Los % se agregan al valor del parámetro, no al texto SQL.
                    comando.Parameters.AddWithValue("@patron", "%" + fragmento + "%");

                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            int id = lector.GetInt32(0);
                            string nombre = lector.GetString(1);
                            decimal precio = lector.GetDecimal(2);
                            int existencias = lector.GetInt32(3);

                            Console.WriteLine(
                                id + " | " + nombre + " | " + precio.ToString("F2") + " | " + existencias);
                            filasMostradas++;
                        }
                    }
                }

                if (filasMostradas == 0)
                {
                    Console.WriteLine("No hay productos activos que coincidan con el nombre");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error de MySQL (" + ex.Number + "): " + ex.Message);
            }
        }

        private static decimal LeerDecimalNoNegativo(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string? entrada = Console.ReadLine();

                if (!decimal.TryParse(entrada, out decimal valor))
                {
                    Console.WriteLine("Entrada no válida: se requiere un número decimal.");
                    continue;
                }
                if (valor < 0)
                {
                    Console.WriteLine("Entrada no válida: el valor no puede ser negativo.");
                    continue;
                }
                return valor;
            }
        }

        private static int LeerEnteroNoNegativo(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string? entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out int valor))
                {
                    Console.WriteLine("Entrada no válida: se requiere un número entero.");
                    continue;
                }
                if (valor < 0)
                {
                    Console.WriteLine("Entrada no válida: el valor no puede ser negativo.");
                    continue;
                }
                return valor;
            }
        }

        private static string LeerTextoNoVacio(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string texto = (Console.ReadLine() ?? string.Empty).Trim();

                if (texto.Length == 0)
                {
                    Console.WriteLine("Entrada no válida: el texto no puede estar vacío.");
                    continue;
                }
                return texto;
            }
        }
    }
}