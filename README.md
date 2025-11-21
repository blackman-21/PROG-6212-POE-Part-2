# Contract Monthly Claim System (CMCS)

The **Contract Monthly Claim System (CMCS)** is an ASP.NET Core MVC web application that automates the monthly claim process for contract lecturers.  
It streamlines the workflow across three main perspectives:

- **Lecturer** – captures and submits monthly claims
- **Programme Coordinator / Academic Manager** – reviews, approves, or rejects claims
- **HR** – processes approved claims for payment

This project was developed as Part 3 of a POE, with a focus on **automation**, **validation**, **workflow management**, and **SQL database integration**.

---

## Table of Contents

- [Features](#features)
  - [Lecturer Features](#lecturer-features)
  - [Coordinator / Academic Manager Features](#coordinator--academic-manager-features)
  - [HR Features](#hr-features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Database & Configuration](#database--configuration)
- [Getting Started](#getting-started)
- [Key Implementation Details](#key-implementation-details)
- [Lecturer Feedback & Improvements](#lecturer-feedback--improvements)
- [Screenshots](#screenshots)
- [Known Limitations / Future Work](#known-limitations--future-work)
- [Author](#author)

---

## Features

### Lecturer Features

- Submit monthly claims with the following fields:
  - Lecturer Name  
  - Month  
  - Total Hours Worked  
  - Hourly Rate  
  - Notes (optional)
- **Automatic calculation** of `TotalAmount` based on `TotalHours × HourlyRate`.
- Upload supporting documents (e.g. timesheets) per claim.
- Validation of captured data and uploaded files.
- View a list of submitted claims with status indicators (Pending, Approved, Rejected).

### Coordinator / Academic Manager Features

- View a dedicated **Pending Claims** list.
- Review each claim with:
  - Lecturer details  
  - Month, hours, hourly rate, total amount  
  - Notes and supporting documents
- **Approve** claims (status changes to `Approved`).
- **Reject** claims:
  - Status changes to `Rejected`
  - A **timestamped rejection reason** is automatically appended to the claim’s notes.
- Clear visual indication of statuses using Bootstrap badges.

### HR Features

- View **Approved** claims for payment processing.
- Access all relevant data:
  - Lecturer, month, hours, rate, total amount, submission time.
- Download supporting documents for verification.
- Data is structured to support export to CSV/Excel for payroll workflows.

---

## Technology Stack

- **Framework:** ASP.NET Core MVC
- **Language:** C#
- **ORM:** Entity Framework Core
- **Database:** SQL Server (LocalDB by default)  
- **Views:** Razor (cshtml)
- **Styling:** Bootstrap 5
- **Client-side:** JavaScript (with optional jQuery usage)
- **Version Control:** Git & GitHub

---

## Project Structure

Key folders and files:

- `Controllers/`
  - `ClaimsController.cs`  
    - Handles lecturer claim submission, pending list, approval/rejection, details, and file downloads.
- `Models/`
  - `Claim.cs` – main claim entity
  - `UploadFile.cs` – metadata for uploaded files
  - `ApplicationDbContext.cs` – EF Core DbContext
- `Views/Claims/`
  - `Index.cshtml` – list of claims for the lecturer
  - `Create.cshtml` – claim submission form
  - `Details.cshtml` – full claim details with file downloads
  - (Optionally) `Pending.cshtml` – pending claims for coordinators
- `wwwroot/uploads/`
  - Storage for uploaded claim documents (created at runtime)
- `appsettings.json`
  - Contains `ConnectionStrings` and `UploadSettings`
- `Program.cs`
  - Configures services, EF Core database context, and routing

---

## Database & Configuration

### Connection String

The application uses **SQL Server LocalDB** by default.  
In `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CMCS;Trusted_Connection=True;MultipleActiveResultSets=true"
}
