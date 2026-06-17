# 🗄️ Database Design & ERD Specification (Enterprise Multi-Branch Layout)

This project adopts the **Entity Framework Core Code-First** paradigm. Below is the complete relational mapping schema, structural data snapshot constraints, and dual-layer auditing layout.

---

## 1. Core Relational Schema & Tables Definition

### 1.1 Core Identity & Governance Tables
* **`ApplicationUsers` (Custom ASP.NET Core Identity)**
    * `Id` (int, Primary Key)
    * `FullName` (nvarchar(100))
    * `IsActive` (bool) $\rightarrow$ Default: `true`. Soft-lock flag for staff suspension.
    * `BranchId` (int, Foreign Key, Nullable) $\rightarrow$ Links employee to a specific store context. Remains `null` for public Customers.
* **`ApplicationRoles`**
    * `Id` (int, Primary Key)
    * `Name` (nvarchar(50)) $\rightarrow$ `SuperAdmin`, `GeneralAccountant`, `BranchAdmin`, `Cashier`, `Customer`.

### 1.2 Multi-Branch & Inventory Management Tables
* **`Branches`**
    * `Id` (int, Primary Key)
    * `Name` (nvarchar(100))
    * `LocationAddress` (nvarchar(250))
* **`Categories`**
    * `Id` (int, Primary Key)
    * `Name` (nvarchar(100))
* **`Products` (Factory Method Extensible Base)**
    * `Id` (int, Primary Key)
    * `Name` (nvarchar(150))
    * `Description` (nvarchar(max))
    * `Price` (decimal(18,2))
    * `StockQuantity` (int) $\rightarrow$ Current real-time branch/global stock.
    * `LowStockThreshold` (int) $\rightarrow$ Trigger limit for the automated alert engine.
    * `ProductType` (nvarchar(50)) $\rightarrow$ Flag for Factory Pattern instantiation (`Physical` or `Digital`).
    * `CategoryId` (int, Foreign Key)

### 1.3 Order Execution & Offline Synchronization Tables
* **`Orders`**
    * `Id` (int, Primary Key)
    * `UserId` (int, Foreign Key)
    * `OrderDate` (datetime)
    * `TotalPrice` (decimal(18,2))
    * `Status` (nvarchar(50)) $\rightarrow$ `Pending`, `Processing`, `Completed`, `Cancelled`.
    * `BranchId` (int, Foreign Key, Nullable) $\rightarrow$ Tracks if the order was processed via a physical POS branch.
    * `ClientSyncToken` (Guid, Nullable) $\rightarrow$ **Idempotency Token** used to prevent duplicate billing during Offline POS Data Synchronization.
* **`OrderItems` (Historical Price Snapshot Table)**
    * `Id` (int, Primary Key)
    * `OrderId` (int, Foreign Key)
    * `ProductId` (int, Foreign Key)
    * `Quantity` (int)
    * `UnitPrice` (decimal(18,2)) $\rightarrow$ *Snapshot Strategy:* Captures product price at the exact second of purchase to shield historical metrics from downstream price updates.

### 1.4 State-Driven Return Pipeline Tables
* **`ReturnRequests` (State Pattern Context)**
    * `Id` (int, Primary Key)
    * `OrderItemId` (int, Foreign Key) $\rightarrow$ Target item being returned.
    * `UserId` (int, Foreign Key) $\rightarrow$ Actor initiating the return request.
    * `CurrentState` (nvarchar(50)) $\rightarrow$ Enforces State Pattern boundaries (`PendingInspection`, `Approved`, `Rejected`).
    * `ManagerNotes` (nvarchar(max))
    * `CreatedAt` (datetime)

### 1.5 Intelligent Review Guardrail Tables
* **`ProductReviews`**
    * `Id` (int, Primary Key)
    * `ProductId` (int, Foreign Key)
    * `UserId` (int, Foreign Key)
    * `Rating` (int) $\rightarrow$ Constraint check: `1` to `5`.
    * `Comment` (nvarchar(max))
    * `CreatedAt` (datetime)
* **`WebsiteReviews`**
    * `Id` (int, Primary Key)
    * `UserId` (int, Foreign Key)
    * `Rating` (int)
    * `Feedback` (nvarchar(max))
    * `CreatedAt` (datetime) $\rightarrow$ Used by the API to enforce the 6-month cooling period.

---

## 2. Advanced Dual-Layer Audit Logging Infrastructure

To prevent data tampering and provide granular corporate compliance, the database overrides EF Core's interception pipeline to fork audit telemetry into two dedicated database schemas.

### 2.1 Schema Layout

#### 1. `LocalBranchAuditLogs`
* Tracks operations executed by Cashiers and local Branch Admins (e.g., local POS sales, inventory deductions, category additions).
* **Fields:** `Id` (int), `ActorUserId` (int), `BranchId` (int), `Action` (Create/Update/Delete), `TableName` (nvarchar), `Timestamp` (datetime), `KeyValues` (json), `OldValues` (json), `NewValues` (json).

#### 2. `GlobalCorporateAuditLogs` (Encrypted Scope)
* Tracks high-level configuration shifts executed by Branch Admins, General Accountants, or Super Admins (e.g., dynamic role swaps, soft-lock employee toggles, global discount injection).
* **Fields:** `Id` (int), `ActorUserId` (int), `Action`, `TableName`, `Timestamp` (datetime), `EncryptedPayload` (nvarchar(max)).