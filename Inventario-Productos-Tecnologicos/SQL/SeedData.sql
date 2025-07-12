-- Insertar Categorías principales
INSERT INTO TECO_M_Categoria (TC_Nombre, TB_Activo)
VALUES ('Componentes', 1),
       ('PC', 1),
       (N'Periféricos', 1);

-- Insertar Marcas comunes en el mercado
INSERT INTO TECO_M_Marca (TC_Nombre, TB_Activo)
VALUES ('AMD', 1),
       ('Intel', 1),
       ('NVIDIA', 1),
       ('ASUS', 1),
       ('MSI', 1),
       ('Corsair', 1),
       ('Kingston', 1),
       ('Western Digital', 1),
       ('Logitech', 1),
       ('HyperX', 1),
       ('Dell', 1),
       ('HP', 1);

-- Insertar Subcategorías para Componentes (ID: 1)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('Procesadores', 1, 1),
       ('Tarjetas Madre', 1, 1),
       (N'Tarjetas Gráficas', 1, 1),
       ('Memoria RAM', 1, 1),
       ('Almacenamiento', 1, 1),
       ('Fuentes de Poder', 1, 1),
       ('Gabinetes', 1, 1);

-- Insertar Subcategorías para PC (ID: 2)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('PC de Oficina', 2, 1),
       ('Laptops', 2, 1);

-- Insertar Subcategorías para Periféricos (ID: 3)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('Mouse', 3, 1),
       ('Teclado', 3, 1),
       (N'Audífonos', 3, 1);

-- Insertar algunos productos de ejemplo
-- Procesadores
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('AMD Ryzen 7 5800X', 'Procesador AMD Ryzen 7 5800X, 8 Cores, 16 Threads, hasta 4.7GHz', 299.99, 15, 1, 1, 1, 1),
       ('Intel Core i7-12700K', N'Procesador Intel Core i7 de 12va generación, 12 Cores, 20 Threads', 409.99, 10, 2, 1,
        1, 1);

-- Tarjetas Madre
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('ASUS ROG STRIX B550-F', 'Tarjeta madre AMD B550, Socket AM4, PCIe 4.0', 179.99, 8, 4, 2, 1, 0),
       ('MSI MPG B760', 'Tarjeta madre Intel B760, Socket LGA 1700', 189.99, 12, 5, 2, 1, 1);

-- Tarjetas Gráficas
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('ASUS TUF Gaming RTX 3060', N'Tarjeta gráfica NVIDIA GeForce RTX 3060 12GB GDDR6', 399.99, 5, 4, 3, 1, 0),
       ('MSI Gaming X RX 6700 XT', N'Tarjeta gráfica AMD Radeon RX 6700 XT 12GB GDDR6', 479.99, 7, 5, 3, 1, 1);

-- Memoria RAM
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('Corsair Vengeance RGB 32GB', 'Kit de memoria DDR4 32GB (2x16GB) 3600MHz', 129.99, 20, 6, 4, 1, 0),
       ('Kingston Fury Beast 16GB', 'Kit de memoria DDR4 16GB (2x8GB) 3200MHz', 79.99, 25, 7, 4, 1, 0);

-- Almacenamiento
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('WD Black SN850X 1TB', 'SSD NVMe PCIe Gen4 1TB', 149.99, 15, 8, 5, 1, 1),
       ('Kingston KC3000 2TB', 'SSD NVMe PCIe Gen4 2TB', 229.99, 10, 7, 5, 1, 0);

-- PC Prearmadas y Laptops
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('Dell OptiPlex 3000', 'PC de Oficina, Core i5, 8GB RAM, 256GB SSD', 699.99, 5, 11, 8, 1, 0),
       ('HP Pavilion Gaming', 'Laptop Gaming, Ryzen 5, 16GB RAM, 512GB SSD, RTX 3050', 999.99, 8, 12, 9, 1, 1);

-- Periféricos
INSERT INTO TECO_A_Producto (TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId, TB_Activo,
                             TB_Novedad)
VALUES ('Logitech G502 HERO', 'Mouse Gaming, 25600 DPI, RGB', 59.99, 30, 9, 10, 1, 0),
       ('HyperX Alloy Origins', N'Teclado Mecánico RGB, Switches Red', 109.99, 15, 10, 11, 1, 1),
       ('HyperX Cloud II', N'Audífonos Gaming 7.1, USB', 99.99, 20, 10, 12, 1, 0);

-- Insertar estados de pedido
INSERT INTO TECO_M_EstadoPedido (TC_NombreEstado, TB_Activo)
VALUES ('Pendiente', 1),
       ('Confirmado', 1),
       ('En Proceso', 1),
       ('Enviado', 1),
       ('Entregado', 1),
       ('Cancelado', 1);

-- Insertar métodos de pago
INSERT INTO TECO_M_MetodosPago (TB_NombreMetodo, TB_Activo)
VALUES (N'Tarjeta de Crédito', 1),
       (N'Tarjeta de Débito', 1),
       ('PayPal', 1);

-- Insertar tipos de movimiento para Kardex
INSERT INTO TECO_M_TipoMovimientoKardex (TC_Tipo, TB_Entrada, TB_Activo)
VALUES ('Entrada', 1, 1),
       ('Salida', 0, 1),
       (N'Devolución', 0, 1),
       (N'Dañado', 0, 1),
       ('Ajuste', 1, 1);

-- Insertar cupones de prueba
INSERT INTO TECO_M_Cupon (TC_Codigo, TC_Descripcion, TC_TipoDescuento, TN_Valor, TF_FechaInicio, TF_FechaFin,
                          TN_UsosMaximos, TN_UsosActuales, TB_Activo)
VALUES ('BIENVENIDA2025', '25% de descuento en tu primera compra', 'P', 25.00, '2025-01-01', '2025-12-31', 100, 0, 1),
       ('VERANO25', 'Descuento de verano ₡5000', 'M', 5000.00, '2025-06-01', '2025-08-31', 50, 0, 1),
       ('TECNO50', '50%', 'P', 50.00, '2025-07-01', '2025-07-31', 200, 0, 1);
