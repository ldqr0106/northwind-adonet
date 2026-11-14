# Northwind: práctica de ADO.NET con C# y MySQL

## Resultado esperado

La actividad comprende la creación de una base Northwind de práctica, la
comprobación de la conexión desde C# y el desarrollo de consultas sobre el
catálogo de productos. La solución deberá utilizar parámetros, leer los
resultados con los tipos correspondientes, manejar valores NULL y liberar
los recursos utilizados. El tutorial incluye ejemplos de errores y transacciones.

## Requisitos de ejecución

Se requiere .NET 10, Visual Studio con soporte para esa versión y acceso a un
servidor MySQL 8.0.16 o posterior. Un cliente SQL como MySQL Workbench permite
ejecutar los scripts; la instalación del cliente no incluye necesariamente
la instalación del servidor.

## Preparación y ejecución

1. **Descarga del proyecto.** El proyecto podrá descargarse desde GitHub mediante
   **Code > Download ZIP**, extrayendo el archivo completo, o mediante la clonación
   del repositorio con Git.
2. **Creación de la base.** La carpeta `base-datos` contiene el script de creación
   y los datos iniciales. La importación deberá
   realizarse según las [instrucciones de Northwind](base-datos/README.md).
3. **Apertura de la solución.** Se deberá abrir `Northwind-Catalogo.slnx` y permitir
   la restauración del paquete `MySql.Data`.
4. **Configuración de la conexión.** En `Northwind-Catalogo/Program.cs`, completa
   servidor, puerto, usuario y contraseña en la variable `cadenaConexion`.
   Conserva `Database=northwind` y la sintaxis de las cadenas de C#.
5. **Configuración de TLS.** `SslMode=Required` exige que el servidor acepte TLS.
   La configuración deberá corresponder al servidor utilizado. En servicios
   remotos deberán respetarse sus requisitos de TLS y certificados.
6. **Comprobación inicial.** Al compilar y ejecutar, la aplicación deberá mostrar
   **Conexión abierta correctamente** y **Cantidad de productos: 77** antes del
   recorrido del tutorial.

Antes de realizar un commit o entregar el proyecto, sustituye los datos reales
de acceso en `Program.cs` por `TU_USUARIO` y `TU_CONTRASENA`. Ningún archivo
ni commit del repositorio debe contener una contraseña real.

La ejecución desde terminal podrá realizarse con los siguientes comandos,
desde la carpeta principal del proyecto:

```powershell
dotnet build Northwind-Catalogo.slnx
dotnet run --project Northwind-Catalogo
```

## Comprobación de errores de conexión

Ante un fallo, se deberá comprobar primero el acceso al mismo servidor desde
el cliente MySQL y revisar servidor, puerto, base y credenciales. El registro
del problema deberá indicar si ocurre en `Open()` o al ejecutar SQL, el código
y mensaje del error y el paso realizado, sin revelar datos de acceso.

## Tutorial y taller

El archivo
[`Northwind-Catalogo/AspectosAdoNet.cs`](Northwind-Catalogo/AspectosAdoNet.cs)
contiene el orden de ejecución de los ejemplos y las explicaciones del tutorial
junto a cada método.

En el código encontrarás explicaciones y propuestas para cambiar valores
y observar qué sucede.

Continúa con el [taller de consultas](Northwind-Catalogo/README.md): construirás una búsqueda
por precio y existencias, su conteo y otra búsqueda por parte del nombre.
Allí encontrarás las
consultas que debes construir, las comprobaciones y los archivos que entregarás.
Para resolverlo, crea un archivo nuevo llamado `BusquedaProductos.cs` dentro
de la carpeta `Northwind-Catalogo`, siguiendo las indicaciones del taller.
Debes entregar ese archivo con las tres consultas y un documento
`RESULTADOS.md` en la raíz del repositorio con tus exploraciones, pruebas y
explicaciones. Incluye también el proyecto completo para poder ejecutarlo.

## Referencias

- [Cadenas de conexión de Connector/NET](https://dev.mysql.com/doc/connector-net/en/connector-net-connections-string.html).
- [Opciones de conexión y TLS](https://dev.mysql.com/doc/connector-net/en/connector-net-8-0-connection-options.html).
- [Creación y selección de una base MySQL](https://dev.mysql.com/doc/refman/8.0/en/creating-database.html).
