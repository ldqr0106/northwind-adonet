# Taller 3: consultas de productos con ADO.NET

**Autor:** Luis Daniel Quintero Rodríguez
**Curso:** Programación III · Universidad Tecnológica Oteima
**Repositorio:** https://github.com/ldqr0106/northwind-adonet

## Informe breve

En este taller trabajé con la base Northwind (77 productos) en MySQL 8.0 desde una aplicación de consola en C# (.NET 10), usando ADO.NET con `MySql.Data`. Primero comprobé la conexión y recorrí los ejemplos del tutorial, cambiando los valores que proponen los comentarios para ver qué ocurría. Después construí las tres consultas del taller en `BusquedaProductos.cs`.

### Qué contiene este repositorio

| Archivo | Para qué sirve |
|---|---|
| `Northwind-Catalogo/BusquedaProductos.cs` | Mis tres consultas: búsqueda por precio y existencias, su conteo con `COUNT(*)`, y búsqueda por parte del nombre. |
| `Northwind-Catalogo/Program.cs` | Conexión e integración: ejecuta el tutorial y las dos búsquedas. |
| `Northwind-Catalogo/AspectosAdoNet.cs` | Ejemplos del tutorial. El código original queda activo y mis variantes probadas están como comentarios («Variante probada»). |
| `RESULTADOS.md` | Mis exploraciones del tutorial, los 11 casos de prueba, la comparación de orden ascendente y descendente, y mis explicaciones. |
| `base-datos/` | Scripts para crear y verificar la base Northwind. |

### Cómo resolví las búsquedas

- **Parámetros:** ningún valor del usuario se concatena al SQL. Uso `@precioMaximo`, `@existenciasMinimas` y `@patron`; en la búsqueda por nombre los `%` van en el valor del parámetro.
- **Validación:** `decimal.TryParse` e `int.TryParse`, rechazo de negativos y de texto vacío, todo antes de ejecutar SQL.
- **Resultados:** lectura con `MySqlDataReader` y tipos correspondientes, `IsDBNull` para presentar «Presentación sin registrar», y mensajes claros cuando no hay coincidencias.
- **Conteo:** después de liberar el lector, `COUNT(*)` con `ExecuteScalar` para comparar con las filas mostradas.
- **Recursos y errores:** `using` en conexión, comando y lector, y `MySqlException` capturada aparte.

Los resultados completos de las pruebas están en [`RESULTADOS.md`](RESULTADOS.md).

## Cómo ejecutarlo

Requiere .NET 10, MySQL 8.0.16 o posterior y la base Northwind creada con los scripts de `base-datos` (ver [instrucciones](base-datos/README.md)).

1. Abrir `Northwind-Catalogo.slnx` y permitir la restauración del paquete `MySql.Data`.
2. En `Northwind-Catalogo/Program.cs`, sustituir `TU_USUARIO` y `TU_CONTRASENA` por los datos de tu servidor. Conservar `Database=northwind`.
3. Compilar y ejecutar:

```powershell
dotnet build Northwind-Catalogo.slnx
dotnet run --project Northwind-Catalogo
```

Al iniciar debe mostrar **Conexión abierta correctamente** y **Cantidad de productos: 77**.

## Seguridad

Los datos de acceso en `Program.cs` están sustituidos por los marcadores `TU_USUARIO` y `TU_CONTRASENA`. Ningún archivo ni commit del repositorio contiene una contraseña real.
