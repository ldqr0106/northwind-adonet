# Taller 3: consultas de productos con ADO.NET

Luis Daniel Quintero Rodríguez · Programación III · Oteima

Todas las pruebas se hicieron sobre la misma base Northwind (77 productos). Las consultas de búsqueda no modifican registros; los ejemplos de escritura del tutorial trabajan con tablas temporales.

---

## 1. Exploración del tutorial

Primero se ejecutó el ejemplo original de cada método y después se cambiaron los valores que indican los comentarios. Las variantes se conservaron como comentarios en `AspectosAdoNet.cs` y el código activo quedó restaurado al original.

### 1. MostrarRecursos

| Cambio | Resultado |
|---|---|
| Original (`LIMIT 2`) | `State` antes de `Open`: Closed; después de `Open`: Open. Salieron 2 nombres: Chai y Chang. Tras liberar el lector la conexión seguía en Open. |
| `LIMIT 5` | Salieron 5 nombres: Chai, Chang, Aniseed Syrup, Chef Anton's Cajun Seasoning y Chef Anton's Gumbo Mix. |
| Segunda consulta después de liberar el lector (`SELECT COUNT(*) FROM Products`) | Devolvió 77 sin problema y la conexión seguía en Open. |

**Explicación:** liberar el lector no cierra la conexión, solo libera el resultado que ocupaba la conexión; por eso se pudo ejecutar otra consulta con el mismo comando. La conexión se libera con el `using` de la conexión, al salir de su bloque. El `using` de las llaves llama a `Dispose` y libera recursos; el `using` del inicio del archivo solo importa namespaces, así que no cumple la misma función.

### 2. ListarProductos y MostrarParametros (búsqueda por ID)

| ID ingresado | Resultado | ¿Llega a ejecutar SQL? |
|---|---|---|
| `1` | «Producto encontrado: Chai» | Sí |
| `99999` | «No existe un producto con ese ID.» | Sí (la consulta no devolvió filas) |
| `0` | «El ID debe ser un número entero mayor que cero.» | No |
| `abc` | «El ID debe ser un número entero mayor que cero.» | No |

**Explicación:** `0` y el texto se rechazan antes de enviar nada a MySQL, por la validación con `int.TryParse` y la comprobación `idProducto <= 0`. `1` y `99999` son enteros válidos, así que sí se envía la consulta con el parámetro `@id`; la diferencia es que el 1 existe y el 99999 no.

### 3. MostrarTiposYNulos

| Valor de `@id` | Resultado |
|---|---|
| 5 (original) | Chef Anton's Gumbo Mix, precio 21.35, más las dos filas simuladas: «Precio cero» (0.00) y «Sin precio» (sin registrar). |
| 1 | Chai, precio 18.00, más las dos filas simuladas. |
| 99999 | Solo las dos filas simuladas: desaparece la fila real, porque no existe un producto con ese ID. |

**Explicación:** el cero es un valor conocido (0.00); NULL significa que no hay valor (se detecta con `IsDBNull` y se muestra «sin registrar»); y una consulta sin filas es otro caso, porque `Read` devuelve `false` (con 99999 desaparece la fila real). Sustituir un precio ausente por cero cambiaría su significado: diría que el producto cuesta cero, cuando en realidad el precio no está registrado.

### 4. Errores (consulta, conexión y restricción)

| Caso | Original | Variante |
|---|---|---|
| 4.1 Consulta | Tabla inexistente: error **1146**, se activa el `catch` con `when`. La conexión sigue funcionando (`SELECT 1` devuelve 1). | Con la tabla `Products` no hay error: «La tabla existe; no se reprodujo el fallo esperado.» No se activa el `catch`. |
| 4.2 Conexión | Puerto 1 de prueba: error **1042** («Unable to connect to any of the specified MySQL hosts»). La conexión nunca llega a abrirse. | — |
| 4.3 Restricción | Primera inserción del ID 1 correcta; la segunda viola la clave primaria: error **1062** de clave duplicada. | Con un segundo ID distinto de 1 (ID 2) ambas inserciones son correctas y no hay excepción. |

**Explicación:** el fallo de conexión ocurre antes de enviar SQL (la conexión no llega a abrirse), mientras que el de consulta ocurre con la conexión ya abierta y esta sigue funcionando después. El `catch` con `when` solo atrapa el código indicado (1146 o 1062); si no hay error, no se activa. Un resultado vacío tampoco es una excepción.

### 5. MostrarTransacciones

| Importe | Después del Commit (caso A) | Después del Rollback (caso B, con error 1062) |
|---|---|---|
| 20 (original) | Cuenta 1: 80.00 y cuenta 2: 70.00 | Siguen en 80.00 y 70.00 |
| 10 (variante) | Cuenta 1: 90.00 y cuenta 2: 60.00 | Siguen en 90.00 y 60.00 |

**Explicación:** en el caso B el descuento se ejecuta y falla antes de abonar; el `Rollback` deshace también el descuento, y los saldos conservan lo que dejó el último `Commit`. Esto vale con ambos importes.

---

## 2. Los once casos de prueba (búsquedas del taller)

| # | Entrada | Resultado observado |
|---|---|---|
| 1 | Precio máximo 20, stock mínimo 10 | 32 filas, todas activas, con precio ≤ 20 y stock ≥ 10. Primera: 33 Geitost 2.50; última: 49 Maxilaku 20.00. `COUNT(*)` = 32, coincide con el listado. |
| 2 | Precio 0, stock 0 | «No hay productos que cumplan los filtros». 0 filas y conteo 0, sin error. |
| 3 | Precio 1000, stock 10000 | «No hay productos que cumplan los filtros». 0 filas y conteo 0. Sin `MySqlException`: es ausencia de coincidencias, no un error de conexión o SQL. |
| 4 | Precio `-5`; stock `-1` | Rechazados: «el valor no puede ser negativo». No se ejecutó SQL. |
| 5 | Texto `abc` en cada entrada | Precio: «se requiere un número decimal». Stock: «se requiere un número entero». El programa vuelve a pedir el dato y no se cierra. |
| 6 | Stock `5.5` | Rechazado: «se requiere un número entero». |
| 7 | Límites inclusivos: Geitost, precio 2.50 y stock 112 | Aparece `33 / Geitost / 2.50 / 112 / 500 g`. 1 fila y `COUNT(*)` = 1: el producto con los valores exactos entra. |
| 8 | Presentación NULL: `CAST(NULL AS CHAR) AS QuantityPerUnit`, precio 20 y stock 20 | 27 filas, todas con «Presentación sin registrar». Conteo 27, coincide. Después se restauró la consulta normal (`QuantityPerUnit`). |
| 9 | Nombre: `chai` (también `CHAI`, y `geitost`) | `1 / Chai / 18.00 / 39`; con `geitost`: `33 / Geitost / 2.50 / 112`. Solo productos activos, ordenados por nombre y luego por ID. `LIKE` no distinguió mayúsculas. |
| 10 | Nombre: `zzzzsincoincidencias999` | «No hay productos activos que coincidan con el nombre». Sin error. |
| 11 | Nombre: Enter vacío y luego solo espacios | Ambos rechazados («el texto no puede estar vacío») antes de ejecutar SQL. Después `chai` se ejecutó con normalidad. |

---

## 3. Orden ascendente y descendente

Con precio 20 y stock 10 se probó `ORDER BY UnitPrice DESC, ProductID ASC`.

- **Ascendente:** Geitost (2.50) primero y Maxilaku (20.00) al final. 32 filas.
- **Descendente:** Maxilaku (20.00) primero y Geitost (2.50) al final. 32 filas.
- **Son los mismos 32 productos; solo cambia su posición.** El filtro no cambia, solo el orden. Entre productos con el mismo precio (por ejemplo los de 18.00) el desempate sigue por `ProductID` ascendente.
- Para la entrega se restauró el orden ascendente.

---

## 4. Explicación con mis palabras

### Asignación de parámetros

Cuando el usuario escribe un precio o un nombre, yo no lo pego dentro del texto del SQL. Dejo un marcador, por ejemplo `@precioMaximo`, y después le doy su valor con `Parameters.AddWithValue`. Así MySQL recibe por un lado la consulta y por otro los datos, y lo que escriba la persona siempre se toma como un dato, nunca como parte de la instrucción. Con eso evito la inyección SQL. En la búsqueda por nombre los `%` no van en el SQL sino en el valor del parámetro (`"%" + fragmento + "%"`), y así el `LIKE` encuentra el fragmento en cualquier parte del nombre.

### Qué libera cada `using`

- **El del `MySqlDataReader`:** libera el lector cuando termino de recorrer las filas. La conexión queda libre para hacer otra consulta, pero sigue abierta.
- **El del `MySqlCommand`:** libera el comando, y tampoco cierra la conexión.
- **El del `MySqlConnection`:** cierra la conexión y la devuelve al pool. No apaga el servidor MySQL.

Cada uno se libera al salir de su bloque, y en orden inverso a como se abrieron, incluso si pasa una excepción.

### Cero coincidencias, NULL y excepción

Son tres cosas distintas y no las debo mezclar:

- **Cero coincidencias:** la consulta funcionó bien, pero ninguna fila cumple los filtros. `Read()` devuelve `false` de una vez y yo muestro un mensaje. No es un error. Lo comprobé con precio 0 y stock 0, y con precio 1000 y stock 10000.
- **Columna NULL:** sí hay fila, pero ese dato no tiene valor. Uso `IsDBNull` antes de leerlo y muestro «Presentación sin registrar». NULL no es lo mismo que cero ni que un texto vacío.
- **Excepción (`MySqlException`):** algo falló de verdad: no conectó (1042), la tabla no existe (1146) o se repitió una clave (1062). Eso lo atrapo con `catch` y aviso del error.

---

## 5. Seguridad

Los datos reales de acceso de `Program.cs` se sustituyeron por los marcadores `TU_USUARIO` y `TU_CONTRASENA` antes de publicar. Ningún archivo ni commit contiene una contraseña real.
