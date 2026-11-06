![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)  ![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue?logo=dotnet)  ![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-purple?logo=dotnet)  ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver)  ![Bootstrap](https://img.shields.io/badge/Bootstrap-5-purple?logo=bootstrap)  ![jQuery](https://img.shields.io/badge/jQuery-blue?logo=jquery)  ![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)![License](https://img.shields.io/badge/license-Restricted%20Use-red)![Last Commit](https://img.shields.io/github/last-commit/MinorPorras/TecnoCore-VentasDeElectronicos)![GitHub repo size](https://img.shields.io/github/repo-size/MinorPorras/TecnoCore-VentasDeElectronicos)  ![GitHub issues](https://img.shields.io/github/issues/MinorPorras/TecnoCore-VentasDeElectronicos)  ![GitHub stars](https://img.shields.io/github/stars/MinorPorras/TecnoCore-VentasDeElectronicos)[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/MinorPorras/TecnoCore-VentasDeElectronicos)

# TecnoCore - Electronics Sales & Inventory Management System

TecnoCore is a comprehensive web application designed for retail operations, integrating Point of Sale (POS), inventory management, and e-commerce capabilities into a single, cohesive solution. Built with Clean Architecture principles, it is designed for scalability, ease of maintenance, and future expansion.

---

## 🖼️ Application Interface

### Secure Login
<img width="1193" height="671" alt="Captura de pantalla 2025-07-22 211303" src="https://github.com/user-attachments/assets/e93b17bc-e713-4691-8e97-45bce72f671e" />

### Product Query & Management
<img width="1187" height="655" alt="Captura de pantalla 2025-07-22 211527" src="https://github.com/user-attachments/assets/a84acc1d-3ca7-4bb4-aa99-9661e7ecb188" />

### Shopping Cart Details
<img width="1196" height="643" alt="focus_btnTerminarCompraDetalleCarrito" src="https://github.com/user-attachments/assets/f6442253-b0e9-46b9-ac26-fb2c88473590" />

### User Dashboard (Home)
<img width="1186" height="558" alt="alertSucessRegister" src="https://github.com/user-attachments/assets/c414ffa4-6f99-4115-8790-13317ba1df24" />

### Advanced Product Search Modal
<img width="1187" height="614" alt="focus_btnSearchProductModalKardex" src="https://github.com/user-attachments/assets/bc7385db-ef21-4af7-b2a2-4eccfca8dd27" />

### General Statistics Dashboard
<img width="1252" height="831" alt="Captura de pantalla 2025-08-01 184545" src="https://github.com/user-attachments/assets/83a109e0-f2a9-4d49-9027-03fb0259cf13" />

---

## 🗺️ Future Enhancements & Roadmap

- [ ] Implementation of robust Two-Factor Authentication (2FA).
- [ ] Secure user password modification workflow.
- [ ] Granular Role-Based Access Control (RBAC) with fine-grained permission management.
- [ ] Integration with a real-world payment gateway.
- [ ] Interactive data visualization charts for the admin dashboard.
- [ ] Expanded reporting module (Kardex movements, product performance, client analytics, brand/category breakdowns).

---

## 🚀 Getting Started (Local Development)

Follow these steps to set up and run the project on your local machine.

### Prerequisites
Ensure you have the following software installed:
- [.NET 9 SDK](https://dotnet.microsoft.com/download) (or higher)
- An IDE: JetBrains Rider, Visual Studio 2022, or VS Code with the C# Dev Kit
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition)
- [Git](https://git-scm.com/)

### 1. Clone the repository

```bash
git clone https://github.com/MinorPorras/TecnoCore-VentasDeElectronicos.git
cd TecnoCore-VentasDeElectronicos
```
### 2. Database Configuration
The project uses Entity Framework Core for database management.

**Create the Database:**
- Open SQL Server Management Studio (SSMS) or a similar tool.
- Create a new empty database (e.g., `TecnoCoreDB_Dev`).

**Configure the Connection String:**
- In the project, locate the `appsettings.Development.json` file.
- Find the `ConnectionStrings` section.
- Update the `DefaultConnection` value to point to your local SQL Server instance and the newly created database.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TecnoCoreDB_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
### 3. Apply Migrations
You can apply migrations using either the Package Manager Console (Visual Studio) or the .NET CLI.

**Option A: Package Manager Console (Visual Studio)**
- Go to Tools > NuGet Package Manager > Package Manager Console.
- Ensure the default project is set to the main infrastructure/data project.

```powershell
Update-Database
```
**Option B: .NET CLI (Terminal)**
- Ensure you are in the main project directory (containing the `.csproj` file).

[📝 BLOQUE DE CÓDIGO: bash — comando para actualizar la base de datos]
dotnet ef database update

### 4. Run the Application
- **Build & Run:** Open the solution in your preferred IDE and run the project. This will start the Kestrel web server and open the application in your default browser.
- **Initial Seed Data:** On the first run, the application automatically seeds the database with essential data:
  - **Roles:** "Administrator" and "Client"
  - **Geography:** Provinces and their corresponding cantons.
  - **Default Admin User:** Username: `admin` | Password: `Password123!`
  - **Default Client User:** Username: `cliente` | Password: `Password123!`

### 5. (Optional) Load Test Data
The repository includes SQL scripts in the `Data` folder to populate the database with sample data (orders, products, etc.).
- Execute the scripts prefixed with `seed` to add test data.
- Execute the scripts prefixed with `clean` to remove the test data and reset the tables.

---

## 🤝 License & Usage

This project was developed as a comprehensive retail management solution. The source code is provided for portfolio demonstration purposes.
For inquiries regarding usage or collaboration, please contact me via [LinkedIn](https://www.linkedin.com/in/minor-porras/) or [Email](mailto:minorp1415@protonmail.com).

💡 Developed with ❤️ and a focus on scalable architecture by [Minor Porras](https://github.com/MinorPorras)
