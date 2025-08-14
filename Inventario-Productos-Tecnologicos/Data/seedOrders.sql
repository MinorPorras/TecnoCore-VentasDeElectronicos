-- =================================================================
-- SCRIPT PARA GENERAR PEDIDOS DE PRUEBA EN LA BASE DE DATOS TECNOCORE
-- =================================================================
-- Este script asume que ya existen datos en las tablas:
-- TECO_A_User, TECO_A_Producto, TECO_M_MetodosPago, y TECO_M_EstadoPedido.
-- =================================================================

SET NOCOUNT ON;

-- =================================================================
-- CONFIGURACIÓN
-- Modifica estas variables para cambiar la cantidad de datos generados
-- =================================================================
DECLARE @NumeroDePedidosACrear INT = 50; -- ¿Cuántos pedidos quieres crear?
DECLARE @MaxItemsPorPedido INT = 8;      -- ¿Cuál es el número máximo de productos diferentes por pedido?
DECLARE @MaxCantidadPorItem INT = 3;     -- ¿Cuál es la cantidad máxima de unidades por cada producto en un pedido?
DECLARE @DiasAntiguedadMax INT = 365;    -- ¿Qué tan antiguos pueden ser los pedidos (en días)?

-- =================================================================
-- INICIO DEL SCRIPT
-- =================================================================

PRINT 'Iniciando la generación de pedidos de prueba...';

DECLARE @ContadorPedidos INT = 0;

WHILE @ContadorPedidos < @NumeroDePedidosACrear
BEGIN
    -- 1. OBTENER DATOS ALEATORIOS PARA EL ENCABEZADO DEL PEDIDO
    -----------------------------------------------------------------
    DECLARE @UsuarioId NVARCHAR(450);
    DECLARE @MetodoPagoId INT;
    DECLARE @EstadoPedidoId INT;
    DECLARE @FechaPedido DATETIME;
    DECLARE @TransaccionId UNIQUEIDENTIFIER = NEWID();
    DECLARE @NumeroTarjeta NVARCHAR(16);

    -- Seleccionar un usuario al azar
    SELECT TOP 1 @UsuarioId = TC_UserId FROM dbo.TECO_A_User ORDER BY NEWID();

    -- Seleccionar un método de pago al azar
    SELECT TOP 1 @MetodoPagoId = TN_Id FROM dbo.TECO_M_MetodosPago ORDER BY NEWID();

    -- Seleccionar un estado de pedido al azar (ej. 'Completado', 'Enviado')
    SELECT TOP 1 @EstadoPedidoId = TN_Id FROM dbo.TECO_M_EstadoPedido ORDER BY NEWID();

    -- Generar una fecha de pedido aleatoria en el último año
    SET @FechaPedido = DATEADD(DAY, -CAST(RAND() * @DiasAntiguedadMax AS INT), GETDATE());
    SET @NumeroTarjeta = CAST(CAST(RAND() * 9000000000000000 + 1000000000000000 AS BIGINT) AS NVARCHAR(16));

    -- 2. INSERTAR EL ENCABEZADO DEL PEDIDO CON TOTALES INICIALES EN 0
    -----------------------------------------------------------------
INSERT INTO dbo.TECO_P_Pedido (
    TN_UsuarioId,
    TN_MetodoPagoId,
    TN_EstadoPedidoId,
    TN_CuponId,
    TN_Subtotal,
    TN_Impuesto,
    TN_Descuento,
    TN_Total,
    TF_Fecha,
    TN_TransaccionId,
    TC_NumTarjeta,
    TB_Activo
)
VALUES (
           @UsuarioId,
           @MetodoPagoId,
           @EstadoPedidoId,
           NULL, -- No se aplicarán cupones en este script para simplificar
           0,    -- Se calculará después
           0,    -- Se calculará después
           0,
           0,    -- Se calculará después
           @FechaPedido,
           @TransaccionId,
           @NumeroTarjeta,
        1
       );

-- Obtener el ID del pedido recién creado
DECLARE @PedidoId INT = SCOPE_IDENTITY();
    DECLARE @SubtotalPedido DECIMAL(18, 2) = 0;

    -- 3. AGREGAR DETALLES (PRODUCTOS) AL PEDIDO
    -----------------------------------------------------------------
    DECLARE @NumeroDeItems INT = CAST(RAND() * @MaxItemsPorPedido AS INT) + 1;
    DECLARE @ContadorItems INT = 0;

    WHILE @ContadorItems < @NumeroDeItems
BEGIN
        DECLARE @ProductoId INT;
        DECLARE @PrecioUnitario DECIMAL(18, 2);
        DECLARE @StockDisponible INT;
        DECLARE @CantidadAComprar INT;

        -- Seleccionar un producto al azar que tenga stock y que no esté ya en este pedido
SELECT TOP 1
            @ProductoId = P.TN_Id,
    @PrecioUnitario = P.TN_Precio,
       @StockDisponible = P.TN_Stock
FROM dbo.TECO_A_Producto AS P
WHERE P.TN_Stock > 0
  AND NOT EXISTS (
    SELECT 1 FROM dbo.TECO_P_DetallePedido D
    WHERE D.TN_PedidoId = @PedidoId AND D.TN_ProductoId = P.TN_Id
)
ORDER BY NEWID();

-- Si se encontró un producto válido
IF @ProductoId IS NOT NULL
BEGIN
    -- Determinar una cantidad aleatoria, sin exceder el stock
    SET @CantidadAComprar = CAST(RAND() * @MaxCantidadPorItem AS INT) + 1;
    IF @CantidadAComprar > @StockDisponible
    BEGIN
        SET @CantidadAComprar = @StockDisponible;
    END

    -- Intentar actualizar el stock de forma segura, solo si hay suficientes unidades
    UPDATE dbo.TECO_A_Producto
    SET TN_Stock = TN_Stock - @CantidadAComprar
    WHERE TN_Id = @ProductoId AND TN_Stock >= @CantidadAComprar;

    -- Si la actualización fue exitosa (se modificó 1 fila), entonces registrar la venta.
    IF @@ROWCOUNT > 0
    BEGIN
        -- Insertar el detalle del pedido
        INSERT INTO dbo.TECO_P_DetallePedido (TN_PedidoId, TN_ProductoId, TN_Cantidad, TN_PrecioUnitario, TB_Activo)
        VALUES (@PedidoId, @ProductoId, @CantidadAComprar, @PrecioUnitario, 1);

        -- Acumular el subtotal
        SET @SubtotalPedido = @SubtotalPedido + (@CantidadAComprar * @PrecioUnitario);
    END
END

        SET @ContadorItems = @ContadorItems + 1;
END

    -- 4. ACTUALIZAR LOS TOTALES DEL PEDIDO
    -----------------------------------------------------------------
    DECLARE @Impuesto DECIMAL(18, 2) = @SubtotalPedido * 0.13; -- Asumiendo un 13% de impuesto

    UPDATE dbo.TECO_P_Pedido
    SET TN_Subtotal = @SubtotalPedido,
        TN_Impuesto = @Impuesto,
        TN_Descuento = 0, -- Simplificado para este script
        TN_Total = @SubtotalPedido + @Impuesto -- (Subtotal + Impuesto - Descuento)
    WHERE TN_Id = @PedidoId;

SET @ContadorPedidos = @ContadorPedidos + 1;
END

PRINT '-------------------------------------------------';
PRINT CONVERT(VARCHAR, @NumeroDePedidosACrear) + ' pedidos de prueba han sido generados exitosamente.';
PRINT '-------------------------------------------------';

SET NOCOUNT OFF;