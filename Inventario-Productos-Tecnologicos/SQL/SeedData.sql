-- Insertar Categorías principales
INSERT INTO Categorias (Nombre, Activo) VALUES 
('Componentes', 1),
('PC', 1),
(N'Periféricos', 1);

-- Insertar Marcas comunes en el mercado
INSERT INTO Marcas (Nombre, Activo) VALUES 
('AMD', 1),
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
INSERT INTO Subcategorias (Nombre, CategoriaId, Activo) VALUES
('Procesadores', 1, 1),
('Tarjetas Madre', 1, 1),
(N'Tarjetas Gráficas', 1, 1),
('Memoria RAM', 1, 1),
('Almacenamiento', 1, 1),
('Fuentes de Poder', 1, 1),
('Gabinetes', 1, 1);

-- Insertar Subcategorías para PC (ID: 2)
INSERT INTO Subcategorias (Nombre, CategoriaId, Activo) VALUES
('PC de Oficina', 2, 1),
('Laptops', 2, 1);

-- Insertar Subcategorías para Periféricos (ID: 3)
INSERT INTO Subcategorias (Nombre, CategoriaId, Activo) VALUES
('Mouse', 3, 1),
('Teclado', 3, 1),
(N'Audífonos', 3, 1);

-- Insertar algunos productos de ejemplo
-- Procesadores
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('AMD Ryzen 7 5800X', 'Procesador AMD Ryzen 7 5800X, 8 Cores, 16 Threads, hasta 4.7GHz', 299.99, 15, 1, 1, 1, 1),
('Intel Core i7-12700K', N'Procesador Intel Core i7 de 12va generación, 12 Cores, 20 Threads', 409.99, 10, 2, 1, 1, 1);

-- Tarjetas Madre
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('ASUS ROG STRIX B550-F', 'Tarjeta madre AMD B550, Socket AM4, PCIe 4.0', 179.99, 8, 4, 2, 1, 0),
('MSI MPG B760', 'Tarjeta madre Intel B760, Socket LGA 1700', 189.99, 12, 5, 2, 1, 1);

-- Tarjetas Gráficas
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('ASUS TUF Gaming RTX 3060', N'Tarjeta gráfica NVIDIA GeForce RTX 3060 12GB GDDR6', 399.99, 5, 4, 3, 1, 0),
('MSI Gaming X RX 6700 XT', N'Tarjeta gráfica AMD Radeon RX 6700 XT 12GB GDDR6', 479.99, 7, 5, 3, 1, 1);

-- Memoria RAM
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('Corsair Vengeance RGB 32GB', 'Kit de memoria DDR4 32GB (2x16GB) 3600MHz', 129.99, 20, 6, 4, 1, 0),
('Kingston Fury Beast 16GB', 'Kit de memoria DDR4 16GB (2x8GB) 3200MHz', 79.99, 25, 7, 4, 1, 0);

-- Almacenamiento
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('WD Black SN850X 1TB', 'SSD NVMe PCIe Gen4 1TB', 149.99, 15, 8, 5, 1, 1),
('Kingston KC3000 2TB', 'SSD NVMe PCIe Gen4 2TB', 229.99, 10, 7, 5, 1, 0);

-- PC Prearmadas y Laptops
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('Dell OptiPlex 3000', 'PC de Oficina, Core i5, 8GB RAM, 256GB SSD', 699.99, 5, 11, 8, 1, 0),
('HP Pavilion Gaming', 'Laptop Gaming, Ryzen 5, 16GB RAM, 512GB SSD, RTX 3050', 999.99, 8, 12, 9, 1, 1);

-- Periféricos
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, MarcaId, SubcategoriaId, Activo, Novedad) VALUES
('Logitech G502 HERO', 'Mouse Gaming, 25600 DPI, RGB', 59.99, 30, 9, 10, 1, 0),
('HyperX Alloy Origins', N'Teclado Mecánico RGB, Switches Red', 109.99, 15, 10, 11, 1, 1),
('HyperX Cloud II', N'Audífonos Gaming 7.1, USB', 99.99, 20, 10, 12, 1, 0);

-- Insertar roles básicos
INSERT INTO Roles (Name, Activo) VALUES
('Administrador', 1),
('Cliente', 1);

-- Insertar estados de pedido
INSERT INTO EstadosPedido (NombreEstado, Activo) VALUES
('Pendiente', 1),
('Confirmado',  1),
('En Proceso',  1),
('Enviado',  1),
('Entregado',  1),
('Cancelado',  1);

-- Insertar métodos de pago
INSERT INTO MetodosPago (NombreMetodo, Activo) VALUES
(N'Tarjeta de Crédito',  1),
(N'Tarjeta de Débito', 1),
('PayPal', 1);

-- Insertar tipos de movimiento para Kardex
INSERT INTO TipoMovimientoKardex (Tipo, Entrada, Activo) VALUES
('Entrada', 1, 1),
('Salida', 0, 1),
(N'Devolución', 0, 1),
(N'Dañado', 0, 1),
('Ajuste', 1, 1);

INSERT INTO Usuarios (Email, Nombre, Apellidos, Contrasena, Telefono, Rol, Activo) VALUES 
('minorp1415@gmail.com','Minor','Porras','1234','12345678',1,true),
('1', 'cliente', 'cliente', '1', '12345678', 1, true),
('cliente', 'cliente', 'cliente', '1', '12345678', 2, true),
('admin', 'admin', 'admin', '1', '12345678', 1, true);
