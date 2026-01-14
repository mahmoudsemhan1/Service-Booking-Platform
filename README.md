# Service Booking Platform API 🚀

A comprehensive, scalable RESTful API built with **ASP.NET Core 8**, following **Clean Architecture** principles. This platform is designed to bridge the gap between service providers (like maintenance, cleaning, or consulting) and customers, featuring a robust booking lifecycle and secure Stripe payment integration.

---

## 🛠 Tech Stack & Architecture

This project is built using modern technologies and industry-standard patterns:

* **Framework:** .NET 8 (Web API)
* **Architecture:** Clean Architecture (Domain, Application, Infrastructure, Web API)
* **Database:** SQL Server with Entity Framework Core
* **Security:** ASP.NET Core Identity & JWT (JSON Web Tokens)
* **External Auth:** Google OAuth 2.0 Integration
* **Payments:** Stripe API (Checkout Sessions & Webhooks)
* **Documentation:** Swagger (OpenAPI) with full XML English Documentation
* **Mapping:** AutoMapper for DTO-Entity transformations

---

## ✨ Key Features

### 👤 Identity & User Management
* **Role-Based Access Control (RBAC):** Distinct permissions for SuperAdmin, Admin, Provider, and User.
* **Secure Authentication:** JWT-based login and registration.
* **Profile Management:** Users and Providers can manage their bios and upload profile pictures.

### 📅 Booking Lifecycle
* **Workflow Engine:** Bookings transition through states: `Pending` ➡️ `Confirmed` ➡️ `Completed`.
* **Provider Actions:** Providers can accept or reject booking requests.
* **Cancellation Policy:** Rules-based cancellation for both users and providers.

### 💳 Payment Integration
* **Stripe Checkout:** Secure, PCI-compliant payment processing.
* **Automated Webhooks:** Real-time database updates and email notifications once payment is successful.
* **Refunds:** Admin-led refund processing directly through the API.

### 🌟 Service Catalog & Reviews
* **Dynamic Catalog:** Providers can assign global services to their profile with custom pricing.
* **Reviews & Ratings:** Authentic feedback system where users can rate providers after service completion.

---

## 📖 API Documentation (Swagger)

The API features professional English documentation accessible via Swagger UI. Every endpoint is detailed with:
* **Summaries:** What the endpoint does.
* **Status Codes:** Detailed response types (200, 201, 204, 400, 401, 403, 404).
* **Multipart Support:** Interactive testing for image and file uploads.



**Access it here:** `https://localhost:[YOUR_PORT]/swagger/index.html`

---

## 🚀 Getting Started

### 1. Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server
* [Stripe Account](https://stripe.com/) (for test API keys)

### 2. Installation
```bash
# Clone the repository
git clone [https://github.com/mahmoudsemhan/service-booking-platform.git](https://github.com/mahmoudsemhan/service-booking-platform.git)

# Navigate to the API project
cd Service_Booking_Platform

# Apply migrations and create the database
dotnet ef database update

# Run the application
dotnet run


Role,Description,Key Permissions
SuperAdmin,System Owner,"Manage all users, assign roles, full data access."
Admin,Platform Manager,"Monitor bookings, process refunds, manage services."
Provider,Service Seller,"Update business info, set prices, complete bookings."
User,Customer,"Browse services, create bookings, pay online, write reviews."




📬 Contact
Mahmoud Semhan * 📧 Email: mahmoudsemhan37@gmail.com

🔗 LinkedIn: Mahmoud Semhan
