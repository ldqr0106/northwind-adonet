-- Ejecutar después de importar, sin modificar datos.
USE `northwind`;

SELECT VERSION() AS VersionMySQL, DATABASE() AS BaseActual;

-- Cada diferencia debe ser cero. Estos son conteos exactos.
SELECT 'Categories' AS Tabla, 8 AS Esperados, COUNT(*) AS Actuales, COUNT(*)-8 AS Diferencia FROM Categories
UNION ALL SELECT 'Suppliers', 29, COUNT(*), COUNT(*)-29 FROM Suppliers
UNION ALL SELECT 'Customers', 91, COUNT(*), COUNT(*)-91 FROM Customers
UNION ALL SELECT 'Employees', 9, COUNT(*), COUNT(*)-9 FROM Employees
UNION ALL SELECT 'Shippers', 3, COUNT(*), COUNT(*)-3 FROM Shippers
UNION ALL SELECT 'Products', 77, COUNT(*), COUNT(*)-77 FROM Products
UNION ALL SELECT 'Orders', 104, COUNT(*), COUNT(*)-104 FROM Orders
UNION ALL SELECT 'OrderDetails', 279, COUNT(*), COUNT(*)-279 FROM OrderDetails;

-- No deben aparecer líneas sin pedido o producto ni pedidos sin líneas.
SELECT COUNT(*) AS DetallesSinRelacion
FROM OrderDetails d
LEFT JOIN Orders o ON o.OrderID=d.OrderID
LEFT JOIN Products p ON p.ProductID=d.ProductID
WHERE o.OrderID IS NULL OR p.ProductID IS NULL;

SELECT COUNT(*) AS PedidosSinDetalle
FROM Orders o LEFT JOIN OrderDetails d ON d.OrderID=o.OrderID
WHERE d.OrderID IS NULL;

-- Comprueba una consulta útil para el catálogo con sus relaciones.
SELECT p.ProductID, p.ProductName, c.CategoryName, s.CompanyName AS Supplier,
       p.UnitPrice, p.UnitsInStock, p.Discontinued
FROM Products p
LEFT JOIN Categories c ON c.CategoryID=p.CategoryID
LEFT JOIN Suppliers s ON s.SupplierID=p.SupplierID
ORDER BY p.ProductID
LIMIT 10;

-- Son estadísticas aproximadas de datos e índices de InnoDB, no la factura
-- ni el espacio total del servicio (logs, respaldos y archivos internos).
SET SESSION information_schema_stats_expiry=0;
SELECT TABLE_NAME AS Tabla, DATA_LENGTH AS DatosBytes,
       INDEX_LENGTH AS IndicesBytes,
       ROUND((DATA_LENGTH+INDEX_LENGTH)/1024,2) AS DatosEIndicesKiB
FROM information_schema.TABLES
WHERE TABLE_SCHEMA='northwind' AND TABLE_TYPE='BASE TABLE'
ORDER BY TABLE_NAME;

SELECT SUM(DATA_LENGTH+INDEX_LENGTH) AS DatosEIndicesBytes,
       ROUND(SUM(DATA_LENGTH+INDEX_LENGTH)/1024/1024,3) AS DatosEIndicesMiB
FROM information_schema.TABLES
WHERE TABLE_SCHEMA='northwind' AND TABLE_TYPE='BASE TABLE';
