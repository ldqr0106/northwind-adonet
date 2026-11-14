# Creación de la base Northwind

La práctica utiliza una adaptación reducida de Northwind para MySQL. Contiene ocho tablas,
77 productos y 600 filas en total. Los datos provienen del ejemplo Northwind
de Microsoft; la muestra conserva el catálogo y una selección de pedidos.

## Archivos incluidos

| Archivo | Propósito |
|---|---|
| [northwind-reducida-mysql.sql](northwind-reducida-mysql.sql) | Crear la base, sus tablas, relaciones y datos iniciales. |
| [LICENCIA-MICROSOFT.txt](LICENCIA-MICROSOFT.txt) | Consultar la licencia de los datos de origen. |

## Procedimiento de importación

1. **Conexión al servidor.** Se deberá utilizar un cliente SQL conectado a un
   servidor MySQL 8.0.16 o posterior, con permisos para crear la base de práctica.
   La importación deberá realizarse en una instancia propia o asignada para
   este trabajo; no deberá ejecutarse sobre la base compartida de clase.
2. **Creación y carga.** El archivo `northwind-reducida-mysql.sql` deberá ejecutarse
   completo. El script crea y selecciona `northwind`, crea las tablas y carga
   los datos. Ante un error, se deberá detener la ejecución y revisar el mensaje
   antes de continuar.
3. **Comprobación.** Después de importar, actualiza la lista de esquemas del
   cliente SQL y comprueba que aparezcan `northwind` y sus ocho tablas. Ejecuta
   la consulta de conteo de productos incluida más abajo; debe devolver `77`.
4. **Conteos de referencia.** La base recién importada contiene las siguientes
   cantidades:

| Tabla | Filas |
|---|---:|
| Categories | 8 |
| Suppliers | 29 |
| Customers | 91 |
| Employees | 9 |
| Shippers | 3 |
| Products | 77 |
| Orders | 104 |
| OrderDetails | 279 |

En MySQL Workbench, el archivo SQL deberá abrirse dentro de la conexión al
servidor y ejecutarse completo. Al finalizar, la actualización de la lista
de esquemas permitirá comprobar la presencia de `northwind` y sus ocho tablas.

La comprobación inicial del proyecto C# corresponde a la siguiente consulta:

```sql
USE northwind;
SELECT COUNT(*) AS CantidadProductos FROM Products;
```

El resultado esperado es `77`. La configuración de la conexión del proyecto
se describe en el [README principal](../README.md).

## Condiciones de ejecución

La importación deberá realizarse una sola vez sobre una base vacía. El script
no borra tablas ni sustituye datos existentes; su repetición falla si las tablas
ya existen. Un fallo puede dejar parte del esquema creado. La corrección del
problema no deberá implicar la eliminación de una base con otros trabajos.

Los nombres de las tablas deberán conservarse, incluidas sus mayúsculas.
`OrderDetails` se escribe sin espacio. La cuenta del tutorial deberá tener
permisos de consulta sobre `Products` y de creación de tablas temporales.
Los ejemplos de errores y transacciones utilizan tablas temporales cuyos
datos desaparecen al cerrar sus sesiones.

## Procedencia

Adaptación educativa del
[Northwind de Microsoft](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql),
blob `ae61e5631d7f03029ec15213ce672b45ceb7e629`, consultado el 6 de noviembre de
2026. Incluye [la licencia de Microsoft](LICENCIA-MICROSOFT.txt).
El archivo original utiliza T-SQL. Para esta práctica deberá ejecutarse la
adaptación MySQL incluida en esta carpeta.
