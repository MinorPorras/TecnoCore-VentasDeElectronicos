-- =================================================================
-- SCRIPT PARA ELIMINAR TODOS LOS PEDIDOS Y RESTAURAR EL STOCK
-- =================================================================
-- ADVERTENCIA: Este script eliminará TODOS los registros de las
-- tablas TECO_P_Pedido y TECO_P_DetallePedido.
-- Úselo con precaución en un entorno de desarrollo/pruebas.
-- =================================================================

SET NOCOUNT ON;

BEGIN TRANSACTION;

BEGIN TRY

    -- 1. CALCULAR EL STOCK A RESTAURAR
    -----------------------------------------------------------------
    -- Crear una tabla temporal para almacenar el stock que se devolverá a los productos.
    DECLARE @StockARestaurar TABLE (
        ProductoId INT,
        CantidadARestaurar INT
    );

    -- Llenar la tabla temporal con la suma de las cantidades de cada producto en todos los pedidos.
INSERT INTO @StockARestaurar (ProductoId, CantidadARestaurar)
SELECT
    TN_ProductoId,
    SUM(TN_Cantidad)
FROM
    dbo.TECO_P_DetallePedido
GROUP BY
    TN_ProductoId;

PRINT 'Stock a restaurar calculado.';

    -- 2. RESTAURAR EL STOCK EN LA TABLA DE PRODUCTOS
    -----------------------------------------------------------------
UPDATE P
SET
    P.TN_Stock = P.TN_Stock + S.CantidadARestaurar
    FROM
        dbo.TECO_A_Producto AS P
    INNER JOIN
        @StockARestaurar AS S ON P.TN_Id = S.ProductoId;

PRINT 'Stock de productos restaurado.';

    -- 3. ELIMINAR LOS DATOS DE LOS PEDIDOS
    -----------------------------------------------------------------
    -- Primero eliminar los detalles para no violar las restricciones de clave foránea.
    DELETE FROM dbo.TECO_P_DetallePedido;
    DBCC CHECKIDENT ('dbo.TECO_P_DetallePedido', RESEED, 0); -- Reinicia el contador de ID
PRINT 'Registros de TECO_P_DetallePedido eliminados.';

    -- Luego eliminar los encabezados de los pedidos.
    DELETE FROM dbo.TECO_P_Pedido;
    DBCC CHECKIDENT ('dbo.TECO_P_Pedido', RESEED, 0); -- Reinicia el contador de ID
PRINT 'Registros de TECO_P_Pedido eliminados.';

    -- 4. CONFIRMAR LA TRANSACCIÓN
    -----------------------------------------------------------------
COMMIT TRANSACTION;

PRINT '-------------------------------------------------';
    PRINT 'Proceso completado. Todos los pedidos han sido eliminados y el stock ha sido restaurado.';
    PRINT '-------------------------------------------------';

END TRY
BEGIN CATCH
    -- Si ocurre un error, deshacer todos los cambios.
ROLLBACK TRANSACTION;

    PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!';
    PRINT 'ERROR: Ocurrió un problema. Se han deshecho todos los cambios.';
    PRINT 'Mensaje de error: ' + ERROR_MESSAGE();
    PRINT '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!';
END CATCH

SET NOCOUNT OFF;