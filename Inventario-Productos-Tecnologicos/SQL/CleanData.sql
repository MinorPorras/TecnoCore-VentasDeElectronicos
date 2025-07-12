-- Deshabilitar todas las restricciones de clave foránea
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'

-- Limpiar datos de tablas relacionadas con pedidos
DELETE
FROM TECO_P_DetallePedido;
DELETE
FROM TECO_P_Pedido;

-- Limpiar datos de productos y sus relaciones
DELETE
FROM TECO_P_ListaDeseos;
DELETE
FROM TECO_P_Kardex;
DELETE
FROM TECO_A_Producto;

-- Limpiar datos de categorización
DELETE
FROM TECO_M_Subcategoria;
DELETE
FROM TECO_M_Categoria;
DELETE
FROM TECO_M_Marca;

-- Limpiar datos de usuarios y sus relaciones
DELETE
FROM TECO_A_Direccion;
DELETE
FROM TECO_A_User;

-- Limpiar datos de configuración
DELETE
FROM TECO_A_Role;
DELETE
FROM TECO_M_EstadoPedido;
DELETE
FROM TECO_M_MetodosPago;
DELETE
FROM TECO_M_TipoMovimientoKardex;
DELETE
FROM TECO_M_Cupon;

-- Reiniciar los contadores de identidad
DBCC CHECKIDENT ('TECO_P_DetallePedido', RESEED, 0);
DBCC CHECKIDENT ('TECO_P_Pedido', RESEED, 0);
DBCC CHECKIDENT ('TECO_P_ListaDeseos', RESEED, 0);
DBCC CHECKIDENT ('TECO_P_Kardex', RESEED, 0);
DBCC CHECKIDENT ('TECO_A_Producto', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_Subcategoria', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_Categoria', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_Marca', RESEED, 0);
DBCC CHECKIDENT ('TECO_A_Direccion', RESEED, 0);
DBCC CHECKIDENT ('TECO_A_User', RESEED, 0);
DBCC CHECKIDENT ('TECO_A_Role', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_EstadoPedido', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_MetodosPago', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_TipoMovimientoKardex', RESEED, 0);
DBCC CHECKIDENT ('TECO_M_Cupon', RESEED, 0);

-- Habilitar todas las restricciones de clave foránea y verificar la integridad de datos
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
