# Vitalia Healthcare Backend API

Welcome to the backend repository of the **Vitalia** platform, a comprehensive healthcare management system. This API is built on **.NET 10** using a modular **Domain-Driven Design (DDD)** architecture and the **CQRS (Command Query Responsibility Segregation)** pattern.

---

## Architecture & Patterns

The codebase is organized following **Domain-Driven Design (DDD)** principles. Each domain functionality is segregated into its own **Bounded Context**:

- **Shared**: Base building blocks, repositories, custom middleware, global error localization, and mediator configurations.
- **Tenant**: Multi-tenancy configurations, branches, clinics, and user role management.
- **Clinical**: Patient diagnostics, medical records, treatments, and prescriptions.
- **Scheduling**: Management of doctor availability slots and appointments.
- **Pharmacy**: Inventory, metadata, and management of medicines.
- **Billing**: Processing of health insurance and billing claims.

### Key Frameworks & Technologies
- **Runtime & SDK**: .NET 10.0
- **Database ORM**: Entity Framework Core with MySQL (`MySql.EntityFrameworkCore`)
- **CQRS**: Command/Query pipeline implemented via `Cortex.Mediator`
- **Documentation**: Swagger UI & OpenAPI Annotations (`Swashbuckle.AspNetCore`)

---

## Getting Started

### Prerequisites

1. **.NET 10.0 SDK** or newer.
2. **MySQL Server** instance.

### Configuration

The application reads its connection strings and settings from `appsettings.json`. By default, it expects a local MySQL instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=vitalia-db;user=root;password=password;"
  }
}
```

Feel free to customize the credentials or database name to match your environment. You can also use **User Secrets** for local development to avoid committing sensitive details:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=your_server;database=vitalia-db;user=your_user;password=your_password;"
```

---

## Running the Application

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Database Migrations & Resiliency**:
   The application uses Entity Framework migrations. Upon startup, the application:
   - Checks if the database is accessible.
   - If MySQL is online, it automatically runs pending migrations and seeds initial data.
   - If MySQL is offline, it prints a warning to the console and allows the server to start anyway (useful if you are working offline or focusing on frontend routing).

   If you want to manually run or apply migrations:
   ```bash
   # Apply migrations manually to the database
   dotnet ef database update
   ```

3. **Run in Development Mode (Hot Reload)**:
   It's highly recommended to run the app using `dotnet watch`. It automatically detects changes, rebuilds, and restarts the web server.
   ```bash
   dotnet watch
   ```
   *Alternatively, you can run a single execution using `dotnet run`.*

---

## Database Seeding

The application features a `DbSeeder` which executes automatically upon startup (`Program.cs`) if the database connection is successful.
It:
1. Reads default/mock data from `db.json`.
2. Seeds the database with default records for users, availability slots, appointments, billing claims, and pharmacy inventory so you can start testing immediately.

---

## API Documentation (Swagger)

Once the application is running, you can access the interactive Swagger UI to explore and test the endpoints:

🔗 **URL**: `http://localhost:<port>/swagger` (or `https://localhost:<port>/swagger` depending on your launch settings).

The Swagger docs are organized by domain tags (e.g., `Medicines`, `Prescriptions`, `Billing Claims`, `Appointments`), having clean endpoint groupings without prefix clutter.
