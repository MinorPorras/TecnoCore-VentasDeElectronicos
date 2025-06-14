-- Deshabilitar todas las restricciones de clave foránea
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'

-- Limpiar datos de tablas relacionadas con pedidos
DELETE FROM DetallePedidos;
DELETE FROM Pedidos;

-- Limpiar datos de productos y sus relaciones
DELETE FROM ListaDeseos;
DELETE FROM Kardex;
DELETE FROM Productos;

-- Limpiar datos de categorización
DELETE FROM Subcategorias;
DELETE FROM Categorias;
DELETE FROM Marcas;

-- Limpiar datos de usuarios y sus relaciones
DELETE FROM Direcciones;
DELETE FROM Usuarios;

-- Limpiar datos de configuración
DELETE FROM Roles;
DELETE FROM EstadosPedido;
DELETE FROM MetodosPago;
DELETE FROM TipoMovimientoKardex;
DELETE FROM Cupones;

-- Reiniciar los contadores de identidad
DBCC CHECKIDENT ('DetallePedidos', RESEED, 0);
DBCC CHECKIDENT ('Pedidos', RESEED, 0);
DBCC CHECKIDENT ('ListaDeseos', RESEED, 0);
DBCC CHECKIDENT ('Kardex', RESEED, 0);
DBCC CHECKIDENT ('Productos', RESEED, 0);
DBCC CHECKIDENT ('Subcategorias', RESEED, 0);
DBCC CHECKIDENT ('Categorias', RESEED, 0);
DBCC CHECKIDENT ('Marcas', RESEED, 0);
DBCC CHECKIDENT ('Direcciones', RESEED, 0);
DBCC CHECKIDENT ('Usuarios', RESEED, 0);
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('EstadosPedido', RESEED, 0);
DBCC CHECKIDENT ('MetodosPago', RESEED, 0);
DBCC CHECKIDENT ('TipoMovimientoKardex', RESEED, 0);
DBCC CHECKIDENT ('Cupones', RESEED, 0);

-- Habilitar todas las restricciones de clave foránea y verificar la integridad de datos
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
