# PocketCart

PocketCart is a Point of Sale (POS) and Inventory Management System built with ASP.NET Core MVC, Entity Framework Core, and ASP.NET Identity. The application is designed for small and medium-sized retail businesses to manage products, inventory, employees, sales, receipts, and promotional deals.

## Features

### User Management

* ASP.NET Identity authentication
* Role-based authorization
* Admin, Manager, and Cashier roles
* Employee management
* Account activation and deactivation

### Product Management

* Create, edit, and delete products
* Product categories
* Product manufacturers
* Expiration date tracking
* Quantity tracking
* EAN-13 barcode generation
* Barcode image generation
* Product export to Excel (.xlsx)

### Shopping Cart

* Barcode-based product scanning
* Add products manually
* Quantity editing
* Automatic stock validation
* Cart clearing functionality

### Receipt Management

* Receipt generation
* PDF receipt creation
* Receipt history
* Receipt export and viewing

### Inventory Control

* Automatic stock deduction after checkout
* Low stock monitoring
* Product quantity management
* Expiration date monitoring

### Deals & Promotions

* Price-off discounts
* Bundle promotions
* Automatic deal detection during barcode scanning
* Active deal dashboard

### Dashboard & Reporting

* Products near expiry
* Expired products
* Products grouped by category
* Products grouped by manufacturer
* Inventory statistics
* Active deals overview

## Technologies Used

### Backend

* ASP.NET Core MVC
* Entity Framework Core
* ASP.NET Identity
* SQL Server

### Frontend

* Razor Views
* Bootstrap 5
* JavaScript
* jQuery
* DataTables

### Libraries

* ClosedXML (Excel Export)
* ZXing.Net (Barcode Generation)
* iTextSharp (PDF Generation)

## System Roles

### Admin

* Full system access
* Manage employees
* Manage products
* Manage deals
* View all receipts
* View dashboards

### Manager

* Manage products
* Manage deals
* View reports and dashboards
* View receipts

### Cashier

* Manage shopping cart
* Scan products
* Process checkouts
* Generate receipts
* View active promotions

## Main Functionalities

### Product Scanning

Products can be added to the cart by entering or scanning an EAN-13 barcode.

### Checkout Process

1. Products are added to the cart.
2. System validates stock availability.
3. Active promotions are applied automatically.
4. Receipt is generated.
5. PDF receipt is created.
6. Product stock is updated.
7. Cart is cleared.

### Deal System

#### Price-Off Deal

A discounted price is applied automatically when the product is scanned.

Example:

* Product Price: 100 MKD
* Discount Price: 80 MKD

#### Bundle Deal

Two products can be sold together at a promotional bundle price.

Example:

* Product A: 100 MKD
* Product B: 120 MKD
* Bundle Price: 180 MKD

## Installation

### Prerequisites

* .NET 8 SDK
* SQL Server
* Visual Studio 2022

### Steps

1. Clone the repository

```bash
git clone https://github.com/yourusername/PocketCart.git
```

2. Update the connection string in:

```json
appsettings.json
```

3. Apply migrations

```powershell
Update-Database
```

4. Run the application

```powershell
dotnet run
```

## Project Architecture

The project follows the Onion Architecture pattern:

### Domain Layer

Contains:

* Entities
* DTOs
* Enums

### Repository Layer

Contains:

* Generic repository implementation
* Database context
* Data access logic

### Service Layer

Contains:

* Business logic
* Validation
* Inventory calculations
* Deal handling

### Web Layer

Contains:

* MVC Controllers
* Razor Views
* Authentication
* User interface

## Future Improvements

* Sales analytics
* Advanced reporting
* Supplier management
* Purchase orders
* Multi-store support
* REST API integration
* Mobile barcode scanner support
* Customer loyalty system

## Author

Developed as a software engineering project focused on retail inventory management, sales processing, and point-of-sale operations.
