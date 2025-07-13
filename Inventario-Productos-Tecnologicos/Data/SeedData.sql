-- Insertar Categorías principales
INSERT INTO TECO_M_Categoria (TC_Nombre, TB_Activo)
VALUES ('Componentes', 1),
       ('PC', 1),
       (N'Periféricos', 1);


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
       ('HP', 1),
       ('Gigabyte', 1),
       ('ASRock', 1),
       ('Samsung', 1),
       ('Crucial', 1),
       ('Seagate', 1),
       ('SanDisk', 1),
       ('Acer', 1),
       ('Lenovo', 1),
       ('Apple', 1),
       ('Razer', 1),
       ('SteelSeries', 1),
       ('Sony', 1),
       ('BenQ', 1),
       ('NZXT', 1),
       ('Lian Li', 1),
       ('Fractal Design', 1),
       ('Cooler Master', 1),
       ('Phanteks', 1),
       ('Thermaltake', 1),
       ('G.Skill', 1),
       ('TeamGroup', 1);

---

-- Insertar Subcategorías para Componentes (ID: 1)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('Procesadores', 1, 1),
       ('Tarjetas Madre', 1, 1),
       (N'Tarjetas Gráficas', 1, 1),
       ('Memoria RAM', 1, 1),
       ('Almacenamiento', 1, 1),
       ('Fuentes de Poder', 1, 1),
       ('Gabinetes', 1, 1);

---

-- Insertar Subcategorías para PC (ID: 2)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('PC de Oficina', 2, 1),
       ('Laptops', 2, 1);

---

-- Insertar Subcategorías para Periféricos (ID: 3)
INSERT INTO TECO_M_Subcategoria (TC_Nombre, TN_CategoriaId, TB_Activo)
VALUES ('Mouse', 3, 1),
       ('Teclado', 3, 1),
       (N'Audífonos', 3, 1),
       ('Monitores', 3, 1),
       ('Webcams', 3, 1);

---

-- Insertar algunos productos de ejemplo
-- Procesadores
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (100001, 'AMD Ryzen 7 5800X', 'Procesador AMD Ryzen 7 5800X, 8 Cores, 16 Threads, hasta 4.7GHz', 159995, 15, 1,
        1, 1, 1),
       (100002, 'Intel Core i7-12700K', N'Procesador Intel Core i7 de 12va generación, 12 Cores, 20 Threads', 220995,
        10, 2, 1, 1, 1),
       (100003, 'AMD Ryzen 9 7950X3D',
        'Procesador de alto rendimiento AMD Ryzen 9 7950X3D, 16 Cores, ideal para gaming.', 450000, 7, 1, 1, 1, 1),
       (100004, 'Intel Core i9-13900K',
        N'Procesador Intel Core i9 de 13ra generación, 24 Cores, excelente para tareas exigentes.', 380000, 8, 2, 1, 1,
        1),
       (100005, 'AMD Ryzen 5 7600X',
        'Procesador AMD Ryzen 5 7600X, 6 Cores, buen equilibrio entre precio y rendimiento.', 185000, 18, 1, 1, 1, 0),
       (100006, 'Intel Core i5-13600K',
        N'Procesador Intel Core i5 de 13ra generación, ideal para gaming de gama media-alta.', 250000, 14, 2, 1, 1, 0),
       (100007, 'AMD Ryzen 3 4100', N'Procesador AMD Ryzen 3 4100, 4 Cores, opción económica para PCs de entrada.',
        75000, 25, 1, 1, 1, 0);

---

-- Tarjetas Madre
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (200001, 'ASUS ROG STRIX B550-F', 'Tarjeta madre AMD B550, Socket AM4, PCIe 4.0', 94995, 8, 4, 2, 1, 0),
       (200002, 'MSI MPG B760', 'Tarjeta madre Intel B760, Socket LGA 1700', 101995, 12, 5, 2, 1, 1),
       (200003, 'Gigabyte AORUS Elite AX Z790', 'Tarjeta madre Intel Z790 de alta gama con Wi-Fi 6E y soporte DDR5.',
        280000, 6, 13, 2, 1, 1), -- Marca Gigabyte (ID 13)
       (200004, 'ASRock B650E Steel Legend WiFi',
        'Tarjeta madre AMD B650E con PCIe 5.0 y Wi-Fi 6E, excelente para Ryzen 7000.', 195000, 9, 14, 2, 1,
        0),                      -- Marca ASRock (ID 14)
       (200005, 'MSI PRO B650M-A WIFI', 'Tarjeta madre micro-ATX AMD B650, ideal para construcciones compactas.',
        140000, 15, 5, 2, 1, 0),
       (200006, 'ASUS Prime H610M-E D4', 'Tarjeta madre Intel H610 de entrada, para procesadores de 12va/13ra Gen.',
        70000, 20, 4, 2, 1, 0),
       (200007, 'Gigabyte B550 AORUS Elite V2', 'Tarjeta madre AMD B550 con buen VRM y conectividad.', 110000, 10, 13,
        2, 1, 0);
-- Marca Gigabyte (ID 13)

---

-- Tarjetas Gráficas
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (300001, 'ASUS TUF Gaming RTX 3060', N'Tarjeta gráfica NVIDIA GeForce RTX 3060 12GB GDDR6', 214995, 5, 4, 3, 1,
        0),
       (300002, 'MSI Gaming X RX 6700 XT', N'Tarjeta gráfica AMD Radeon RX 6700 XT 12GB GDDR6', 256995, 7, 5, 3, 1, 1),
       (300003, 'NVIDIA GeForce RTX 4090',
        N'La tarjeta gráfica más potente para gaming y creación de contenido, 24GB GDDR6X.', 1200000, 2, 3, 3, 1,
        1),                     -- Marca NVIDIA (ID 3)
       (300004, 'AMD Radeon RX 7900 XTX', N'Tarjeta gráfica de gama alta de AMD, 24GB GD6, excelente rendimiento.',
        950000, 3, 1, 3, 1, 1), -- Marca AMD (ID 1)
       (300005, 'NVIDIA GeForce RTX 4070 SUPER',
        N'Tarjeta gráfica de rendimiento intermedio-alto, ideal para 1440p gaming.', 580000, 8, 3, 3, 1,
        0),                     -- Marca NVIDIA (ID 3)
       (300006, 'AMD Radeon RX 6600', N'Tarjeta gráfica económica para 1080p gaming.', 170000, 15, 1, 3, 1,
        0),                     -- Marca AMD (ID 1)
       (300007, 'Intel Arc A770', N'Tarjeta gráfica de Intel con 16GB GDDR6, buena opción para gaming de gama media.',
        280000, 10, 2, 3, 1, 0);
-- Marca Intel (ID 2)

---

-- Memoria RAM
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (400001, 'Corsair Vengeance RGB 32GB', 'Kit de memoria DDR4 32GB (2x16GB) 3600MHz', 69995, 20, 6, 4, 1, 0),
       (400002, 'Kingston Fury Beast 16GB', 'Kit de memoria DDR4 16GB (2x8GB) 3200MHz', 44995, 25, 7, 4, 1, 0),
       (400003, 'G.Skill Trident Z5 RGB 32GB DDR5', 'Kit de memoria DDR5 de alta velocidad (2x16GB) 6000MHz CL30.',
        120000, 10, 32, 4, 1, 1), -- Marca G.Skill (ID 32)
       (400004, 'Corsair Dominator Platinum RGB 64GB DDR4', 'Kit de memoria DDR4 premium (2x32GB) 3200MHz CL16.',
        150000, 5, 6, 4, 1, 0),
       (400005, 'Crucial Pro RAM 16GB DDR5', N'Módulo de memoria DDR5 5600MHz para laptops, 1x16GB.', 60000, 18, 16, 4,
        1, 0),                    -- Marca Crucial (ID 16)
       (400006, 'TeamGroup T-Force Delta RGB 16GB DDR4', N'Kit de memoria DDR4 (2x8GB) 3600MHz con iluminación RGB.',
        55000, 22, 33, 4, 1, 0),  -- Marca TeamGroup (ID 33)
       (400007, 'Kingston ValueRAM 8GB DDR4', N'Módulo de memoria DDR4 2666MHz, ideal para sistemas básicos.', 25000,
        30, 7, 4, 1, 0);

---

-- Almacenamiento
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (500001, 'WD Black SN850X 1TB', 'SSD NVMe PCIe Gen4 1TB', 79995, 15, 8, 5, 1, 1),
       (500002, 'Kingston KC3000 2TB', 'SSD NVMe PCIe Gen4 2TB', 124995, 10, 7, 5, 1, 0),
       (500003, 'Samsung 990 Pro 2TB', 'SSD NVMe PCIe Gen4 de alta velocidad 2TB, ideal para gaming y workstation.',
        180000, 8, 15, 5, 1, 1), -- Marca Samsung (ID 15)
       (500004, 'Crucial P5 Plus 1TB', 'SSD NVMe PCIe Gen4 1TB, rendimiento equilibrado a buen precio.', 90000, 12, 16,
        5, 1, 0),                -- Marca Crucial (ID 16)
       (500005, 'Seagate Barracuda 4TB HDD', N'Disco duro mecánico de 4TB 7200RPM, para almacenamiento masivo.', 55000,
        20, 17, 5, 1, 0),        -- Marca Seagate (ID 17)
       (500006, 'SanDisk Ultra 3D SSD 1TB SATA', N'SSD SATA III 1TB, para actualización de laptops o PCs antiguas.',
        65000, 15, 18, 5, 1, 0), -- Marca SanDisk (ID 18)
       (500007, 'WD Blue SN570 500GB NVMe', N'SSD NVMe PCIe Gen3 500GB, opción económica para almacenamiento rápido.',
        45000, 25, 8, 5, 1, 0);

---

-- Gabinetes
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (600001, 'NZXT H5 Flow',
        'Gabinete ATX de torre media con excelente flujo de aire y panel lateral de vidrio templado.', 85000, 10, 26, 7,
        1, 1),                   -- Marca NZXT (ID 26), Subcat Gabinetes (ID 7)
       (600002, 'Corsair 4000D Airflow', 'Gabinete ATX de torre media con panel frontal optimizado para flujo de aire.',
        75000, 12, 6, 7, 1, 0),  -- Subcat Gabinetes (ID 7)
       (600003, 'Lian Li O11 Dynamic EVO',
        N'Gabinete modular de doble cámara, ideal para configuraciones de alto rendimiento y refrigeración líquida.',
        150000, 7, 27, 7, 1, 1), -- Marca Lian Li (ID 27), Subcat Gabinetes (ID 7)
       (600004, 'Fractal Design North', N'Gabinete ATX de torre media con diseño elegante y paneles de madera.', 110000,
        8, 28, 7, 1, 0),         -- Marca Fractal Design (ID 28), Subcat Gabinetes (ID 7)
       (600005, 'Cooler Master MasterBox Q300L',
        N'Gabinete Micro-ATX compacto y versátil con diseño magnético de filtro de polvo.', 45000, 15, 29, 7, 1,
        0),                      -- Marca Cooler Master (ID 29), Subcat Gabinetes (ID 7)
       (600006, 'Phanteks Eclipse P400A Digital',
        'Gabinete ATX con excelente flujo de aire y tres ventiladores ARGB preinstalados.', 95000, 9, 30, 7, 1,
        0),                      -- Marca Phanteks (ID 30), Subcat Gabinetes (ID 7)
       (600007, 'Thermaltake Core P3 TG Pro',
        N'Gabinete de marco abierto, ideal para mostrar componentes y refrigeración líquida personalizada.', 130000, 5,
        31, 7, 1, 1);
-- Marca Thermaltake (ID 31), Subcat Gabinetes (ID 7)

---

-- PC Prearmadas y Laptops
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (800001, 'Dell OptiPlex 3000', 'PC de Oficina, Core i5, 8GB RAM, 256GB SSD', 374995, 5, 11, 8, 1, 0),
       (900001, 'HP Pavilion Gaming', 'Laptop Gaming, Ryzen 5, 16GB RAM, 512GB SSD, RTX 3050', 534995, 8, 12, 9, 1, 1),
       (800002, 'Alienware Aurora R15', 'PC Gaming de alto rendimiento, Intel i9, RTX 4080, 32GB RAM.', 1500000, 3, 11,
        8, 1, 1),
       (800003, 'Acer Predator Orion 3000', 'PC Gaming compacta, Core i7, RTX 3060, 16GB RAM.', 750000, 6, 19, 8, 1,
        0),    -- Marca Acer (ID 19)
       (800004, 'Lenovo IdeaCentre 3', N'PC de Escritorio básica, AMD Ryzen 3, 8GB RAM, 512GB SSD.', 300000, 10, 20, 8,
        1, 0), -- Marca Lenovo (ID 20)
       (900002, 'MacBook Air M2', N'Laptop ultradelgada de Apple con chip M2, 8GB RAM, 256GB SSD.', 890000, 7, 21, 9, 1,
        1),    -- Marca Apple (ID 21)
       (900003, 'Asus ROG Zephyrus G14', N'Laptop Gaming potente y portátil, Ryzen 9, RTX 4070, 16GB RAM.', 980000, 4,
        4, 9, 1, 1);

---

-- Periféricos
INSERT INTO TECO_A_Producto (TC_Codigo, TC_Nombre, TC_Descripcion, TN_Precio, TN_Stock, TN_MarcaId, TN_SubcategoriaId,
                             TB_Activo, TB_Novedad)
VALUES (100008, 'Logitech G502 HERO', 'Mouse Gaming, 25600 DPI, RGB', 31995, 30, 9, 10, 1, 0),
       (110002, 'HyperX Alloy Origins', N'Teclado Mecánico RGB, Switches Red', 59995, 15, 10, 11, 1, 1),
       (120003, 'HyperX Cloud II', N'Audífonos Gaming 7.1, USB', 54995, 20, 10, 12, 1, 0),
       (100009, 'Razer DeathAdder V3 Pro', N'Mouse Gaming inalámbrico ultraligero, sensor Focus Pro 30K DPI.', 70000,
        20, 22, 10, 1, 1),        -- Marca Razer (ID 22)
       (110005, 'SteelSeries Apex Pro TKL Wireless',
        N'Teclado Mecánico inalámbrico con switches OmniPoint 2.0 ajustables.', 120000, 10, 23, 11, 1,
        1),                       -- Marca SteelSeries (ID 23)
       (120006, 'Sony WH-1000XM5', N'Audífonos con cancelación de ruido líderes en la industria, excelente sonido.',
        180000, 8, 24, 12, 1, 0), -- Marca Sony (ID 24)
       (150007, 'Logitech C920S', N'Webcam Full HD 1080p con enfoque automático y tapa de privacidad.', 45000, 25, 9,
        14, 1, 0),                -- Subcat Webcams (ID 14)
       (130008, 'BenQ ZOWIE XL2546K', 'Monitor Gaming eSports 24.5 pulgadas, 240Hz, 1ms, DyAc+.', 350000, 5, 25, 13, 1,
        1);
-- Marca BenQ (ID 25), Subcat Monitores (ID 13)

---

-- Insertar estados de pedido
INSERT INTO TECO_M_EstadoPedido (TC_NombreEstado, TB_Activo)
VALUES ('Pendiente', 1),
       ('Confirmado', 1),
       ('En Proceso', 1),
       ('Enviado', 1),
       ('Entregado', 1),
       ('Cancelado', 1);

---

-- Insertar métodos de pago
INSERT INTO TECO_M_MetodosPago (TB_NombreMetodo, TB_Activo)
VALUES (N'Tarjeta de Crédito', 1),
       (N'Tarjeta de Débito', 1),
       ('PayPal', 1);

---

-- Insertar tipos de movimiento para Kardex
INSERT INTO TECO_M_TipoMovimientoKardex (TC_Tipo, TB_Entrada, TB_Activo)
VALUES ('Entrada', 1, 1),
       ('Salida', 0, 1),
       (N'Devolución', 0, 1),
       (N'Dañado', 0, 1),
       ('Ajuste', 1, 1);

---

-- Insertar cupones de prueba
INSERT INTO TECO_M_Cupon (TC_Codigo, TC_Descripcion, TC_TipoDescuento, TN_Valor, TF_FechaInicio, TF_FechaFin,
                          TN_UsosMaximos, TN_UsosActuales, TB_Activo)
VALUES ('BIENVENIDA2025', '25% de descuento en tu primera compra', 'P', 25.00, '2025-01-01', '2025-12-31', 100, 0, 1),
       ('VERANO25', N'Descuento de verano ₡5000', 'M', 5000.00, '2025-06-01', '2025-08-31', 50, 0, 1),
       ('TECNO50', '50%', 'P', 50.00, '2025-07-01', '2025-07-31', 200, 0, 1);