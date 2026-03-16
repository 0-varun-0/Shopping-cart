# EShoppingZone API

EShoppingZone is a backend API for an e-commerce platform, built with ASP.NET Core. It provides core functionalities for user authentication, product management, shopping cart operations, order processing, wallet management, and user profile updates.

## Features

The following core functionalities have been implemented:

*   **User Authentication:**
    *   User Registration (`/api/Auth/signup`)
    *   User Login (`/api/Auth/login`) with JWT token generation
*   **Product Management:**
    *   View all products (`/api/Product`)
    *   View product by ID (`/api/Product/{id}`)
    *   Search products (`/api/Product/search`)
    *   Add new product (`/api/Product`) - *Authorized*
    *   Update product (`/api/Product/{id}`) - *Authorized*
    *   Delete product (`/api/Product/{id}`) - *Authorized*
*   **Shopping Cart:**
    *   View user's cart (`/api/Cart`) - *Authorized*
    *   Add item to cart (`/api/Cart/add`) - *Authorized*
    *   Update cart item quantity (`/api/Cart/update`) - *Authorized*
    *   Remove item from cart (`/api/Cart/remove/{productId}`) - *Authorized*
*   **Order Processing:**
    *   Place an order from cart (`/api/Order/checkout`) - *Authorized*
    *   View order history (`/api/Order/history`) - *Authorized*
    *   View order by ID (`/api/Order/{orderId}`) - *Authorized*
*   **Wallet Management:**
    *   View wallet balance and transactions (`/api/Wallet`) - *Authorized*
    *   Add funds to wallet (`/api/Wallet/add-funds`) - *Authorized*
    *   Process payment from wallet (`/api/Wallet/process-payment`) - *Authorized*
*   **User Profile:**
    *   View user profile (`/api/Profile`) - *Authorized*
    *   Update user profile (`/api/Profile`) - *Authorized*

## Technologies Used

*   .NET 10.0
*   ASP.NET Core Web API
*   Entity Framework Core
*   PostgreSQL
*   ASP.NET Core Identity for user management
*   JWT (JSON Web Tokens) for authentication
*   Swagger/OpenAPI for API documentation and testing

## Setup Instructions

### Prerequisites

*   .NET SDK 10.0 (or compatible version)
*   PostgreSQL server running and accessible

### 1. PostgreSQL Database Setup

Ensure you have a PostgreSQL server running. If not, here are instructions for Arch Linux:

```bash
# Install PostgreSQL
sudo pacman -S postgresql

# Initialize the database cluster
# Replace $LANG with your system's locale, e.g., en_US.UTF-8
sudo -u postgres initdb --locale $LANG -E UTF8 -D '/var/lib/postgres/data'

# Start and enable the PostgreSQL service
sudo systemctl enable postgresql
sudo systemctl start postgresql

# Set a password for the 'postgres' user (replace 'your_new_password')
sudo -u postgres psql postgres -c "ALTER USER postgres WITH PASSWORD 'your_new_password';"

# Create the 'eshop' database
sudo -u postgres createdb eshop
```

### 2. Configure Connection String

Update the `ConnectionStrings:DefaultConnection` in `appsettings.json` with your PostgreSQL server details, especially the password you set:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eshop;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "ThisIsAStrongSecretKeyForJWTGenerationAndValidation",
    "Issuer": "EShoppingZone",
    "Audience": "EShoppingZoneUsers",
    "ExpireDays": 7
  },
  // ... other settings
}
```
**Remember to replace `your_password` with the actual password for your PostgreSQL user.**

### 3. Apply Database Migrations

Navigate to the project root directory in your terminal and run the following commands to apply the database schema:

```bash
dotnet tool install --global dotnet-ef # Install EF Core CLI tools if not already installed
dotnet ef migrations add InitialCreate # This should already be done
/home/varun/.dotnet/tools/dotnet-ef database update
```

## Running the Application

From the project root directory, run the application:

```bash
dotnet run
```

The console output will display the URLs where the application is listening (e.g., `https://localhost:7xxx` and `http://localhost:5xxx`).

## API Documentation (Swagger UI)

Once the application is running, you can access the interactive API documentation (Swagger UI) in your web browser at:

`https://localhost:7xxx/swagger` (replace `7xxx` with the HTTPS port shown in your console)

Here you can explore all the available endpoints, their request/response models, and test them directly.

## Authentication Flow

1.  **Register:** Use `POST /api/Auth/signup` with `Email`, `Password`, and `Address`.
2.  **Login:** Use `POST /api/Auth/login` with `Email` and `Password` to receive a JWT token.
3.  **Authorize:** For protected endpoints (marked with `[Authorize]`), click the "Authorize" button in Swagger UI and paste your JWT token in the format `Bearer YOUR_TOKEN_HERE`.

## Possible Future Enhancements

*   Product recommendation system
*   Microservices architecture
*   Redis caching for cart
*   Event-driven order processing
*   Role-based authorization (e.g., Admin roles for product management)
*   More robust error handling and validation
*   Unit and Integration Tests
*   Payment Gateway Integration (beyond simple wallet/COD)
