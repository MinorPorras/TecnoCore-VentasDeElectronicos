[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/MinorPorras/TecnoCore-VentasDeElectronicos)

# TecnoCore-VentasDeElectronicos
Tecno Core se trata de una aplicación web integral diseñada para la tienda Tecno Core para la gestión de puntos de ventas, gestión de inventario y venta en línea, por lo que cuenta con una variedad de funciones que facilitan de gran manera el desarrollo estando todas estas integradas en una misma solución informática que simplifique su futura expansión en caso de ser necesario y mantenimiento.

## Imágenes de la aplicación
### Inicio de sesión
<img width="1193" height="671" alt="Captura de pantalla 2025-07-22 211303" src="https://github.com/user-attachments/assets/e93b17bc-e713-4691-8e97-45bce72f671e" />

### Consulta de productos
<img width="1187" height="655" alt="Captura de pantalla 2025-07-22 211527" src="https://github.com/user-attachments/assets/a84acc1d-3ca7-4bb4-aa99-9661e7ecb188" />

### Detalles del carrito de compras
<img width="1196" height="643" alt="focus_btnTerminarCompraDetalleCarrito" src="https://github.com/user-attachments/assets/f6442253-b0e9-46b9-ac26-fb2c88473590" />

### Pestaña de inicio
<img width="1186" height="558" alt="alertSucessRegister" src="https://github.com/user-attachments/assets/c414ffa4-6f99-4115-8790-13317ba1df24" />

### Modal de busqueda de productos
<img width="1187" height="614" alt="focus_btnSearchProductModalKardex" src="https://github.com/user-attachments/assets/bc7385db-ef21-4af7-b2a2-4eccfca8dd27" />

### Dashboard de estadísticas general
<img width="1252" height="831" alt="Captura de pantalla 2025-08-01 184545" src="https://github.com/user-attachments/assets/83a109e0-f2a9-4d49-9027-03fb0259cf13" />


## Aspectos faltantes a desarrollar
- Autenticación en 2 pasos implementado de mejor forma
- Modificación de contraseña del usuario
- Gestion de roles mejorada con gestionm de accesos
- Implementación de la pasarela de pago real
- Implemntación de gráficos en el dashboard
- Diversificación de los reportes (Reportes de movimientos de kardex, de productos, de clientes, marcas categorías y subcategorías)


##  Instalación para desarrollo
### Requisitos previos para instalación de desarrollador
Asegúrate de tener instalado el siguiente software en tu sistema:
- SDK de .NET 9 (o superior): El entorno de ejecución y las herramientas de desarrollo de .NET.
- Un IDE o editor de código: JetBrains Rider, Visual Studio 2022 o Visual Studio Code con la extensión de C#.
- SQL Server: Una instancia local para la base de datos. ( SQL Server Express o Developer Edition son opciones gratuitas y adecuadas. )
- Git: Para clonar el repositorio.

### Clonación del repositorio
Para poder comenzar con el uso de este proyecto y proseguir con su desarrollo se debe de usar el siguiente comando:  

**git clone https://github.com/MinorPorras/TecnoCore-VentasDeElectronicos**

<img width="898" height="272" alt="imagen" src="https://github.com/user-attachments/assets/98c15441-2179-4a94-a954-200621d9f46a" />


Este agrega a la carpeta seleccionada la solución en su versión más reciente del proyecto para que podamos trabajar con ella. Luego, navega a la carpeta del proyecto recién creada y abre la solución (.sln) con el IDE que se desee.

### Configuración de la base de datos
El proyecto utiliza Entity Framework Core para gestionar la base de datos por lo que se deben de seguir los siguientes pasos para configurarlo correctamente:

#### Crear la base de datos:
- Abrir SQL Server Management Studio (SSMS) o alguna herramienta similar
- Crear una base de datos vacía. Al ser de desarrollo esta puede ser llamada TecnoCoreDB_Dev para reconecerla mejor o dejarle el nombre de TecnoCoreDB o el que se prefiera.

#### Configurar la cadena de conexión
- En el proyecto buscar el archivo appsetting.Development.json
- Localizar la sección de “ConnectionStrings”
- Modificar el valor del DefaultConnection para que apunte a la instancia local de sql server y la base de datos que se acaba de crear

<img width="887" height="157" alt="imagen" src="https://github.com/user-attachments/assets/70ae2853-60ba-4d4a-8d86-471c566d8f8d" />

### Aplicar migraciones
Hay 2 formas de hacer esto el primero es desde la consola del administrador de paquetes nuget desde visual studio:
- Para ingresar a esta se debe de ir a Herramientas > Administrador de paquetes NuGet > Consola del administrador de paquetes
- Ejecuta el siguiente comando para aplicar las migraciones y crear todas las tablas:
**Update-Database**

<img width="866" height="230" alt="imagen" src="https://github.com/user-attachments/assets/38992b64-0e7a-4902-ab21-3da8250cd1c1" />

La segunda es desde la terminal
- Asegúrate de estar en el directorio del proyecto principal (el que contiene el archivo .csproj).
- Ejecuta el siguiente comando para aplicar las migraciones y crear todas las tablas:
**dotnet ef database update**

<img width="704" height="286" alt="imagen" src="https://github.com/user-attachments/assets/2f62fc56-8270-4c45-b7fe-496c13f5084c" />

Al finalizar, tu base de datos estará creada y con toda la estructura de tablas necesaria.

### Ejecución de la aplicación
- **Construir y ejecutar:** Directamente desde el IDE se debe de hacer la build y compilación inicial del proyecto, esto se puede hacer simplemente por medio de la ejecucición del proyecto, esto iniciará el servidor web kestrel y abrirá la aplicación en el navegador web por defecto
- **Primer Inicio:** Por defecto el programa al ser ejecutado inicializa la base de datos por primera vez con los datos mínimos requeridos para su funcionamiento. En este caso son los siguientes:
  - **Roles:** "Administrador" y  "Cliente"
  - Provincias del país
  - Cantones asignados a cada provincia
  - **Usuario administrador:**
    - Usuario de la cuenta: “admin”
    - Contraseña: "Password123!"
  - **Usuario cliente:**
    - Usuario de la cuenta: “cliente”
    - Contraseña: "Password123!"

### (Opcional) Cargar datos de prueba
El repositorio incluye scripts SQL para poblar la base de datos con datos de prueba (pedidos, productos, etc.). Para esto ya se debe de tener correctamente configurada la conexión con la base de datos y el repositorio y proyecto locales ya funcionando. En este caso dentro de la carpeta “Data” se encuentran varios archivos y scripts de SQL. Ejecutar los archivos SQL bajo el nombre de “seed” agrega la información de prueba a la base de datos, mientras que los archivos de nombre “clean” limpiará la información que agregó.
