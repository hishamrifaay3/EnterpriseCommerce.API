# 🛒 OmniChannel Enterprise E-Commerce API - Engineering Blueprint

## 1. Executive Summary
This project is an enterprise-grade, highly scalable Distributed E-Commerce Backend API built using **.NET 9 / .NET 8**. The core architecture demonstrates production-ready software engineering, strictly adhering to **SOLID Principles** and implementing advanced **Design Patterns**. Designed as a Multi-Branch Distributed System, it natively supports hybrid online/offline operations, multi-tiered corporate governance (Super Admin, Branch Admins, General Accountants), a global shipping/logistics routing engine, dynamic discounts, and a dual-layer cryptographic audit trail.

---

## 2. Architectural Overview (Clean Architecture)
The system is designed following the Clean Architecture (Onion Architecture) pattern to enforce strict separation of concerns and decouple business domain logic from external frameworks, databases, and infrastructure.

* **Domain Layer:** The absolute core. Contains business entities, value objects, domain aggregates, and core repository interfaces. It has zero external dependencies (Strict Single Responsibility).
* **Application Layer:** Orchestrates business workflows and use cases. Contains business services, DTOs, strategy contracts, and data mapping definitions.
* **Infrastructure Layer:** Implements data access via Entity Framework Core (SQL Server), customized Identity management, third-party payment/shipping SDK wrappers, and automated background telemetry.
* **Presentation Layer (API):** Managed by ASP.NET Core Controllers to expose secure, optimized RESTful endpoints with robust error handling.

---

## 3. Advanced Design Patterns Applied (Implemented)

### 3.1 Creational Patterns
* **Singleton Pattern (Bill Pugh Method):** Implemented via a thread-safe, lazy-loaded nested helper class for global configuration management (e.g., JWT and System Settings). This eliminates continuous, expensive I/O operations from reading JSON configuration files during concurrent API requests.
* **Factory Method Pattern:** Deployed within the Product Catalog. Object instantiation for varying product types (`PhysicalProduct` requiring logistics vs. `DigitalProduct` requiring instant key generation) is decoupled from controllers using a centralized `ProductFactory`.

### 3.2 Structural Patterns
* **Adapter Pattern:** Implemented in the Payment & Logistics Gateways. It acts as a unified wrapper (`IPaymentAdapter` & `IShippingAdapter`) around multiple third-party SDKs (e.g., Stripe, PayPal, Aramex), shielding core billing/shipping logic from external vendor API changes.

### 3.3 Behavioral Patterns
* **Strategy Pattern:** Powers the Dynamic Discount & Promotion Engine. Instead of utilizing nested, unmaintainable if-else blocks for checking promotional codes (e.g., Percentage Coupons, Fixed Amounts, Buy-One-Get-One), the checkout pipeline consumes pluggable classes implementing an `IDiscountStrategy` contract.
* **State Pattern:** Governs the complex Order Return & Refund Pipeline. The lifecycle of a return request is encapsulated into concrete state classes (`PendingInspectionState`, `ApprovedState`, `RejectedState`) inheriting from an `IReturnState` interface. State transitions automatically trigger sub-workflows like inventory restocking and payment reversals via the `IPaymentAdapter`.
* **Observer Pattern:** Drives the Decoupled Event System. Upon a successful order payment, an event is broadcasted. Registered observers (e.g., `EmailNotificationService`, `StockInventoryService`, `LogisticsRoutingService`) independently react to execute business operations without tight coupling.

---

## 4. Strict Adherence to SOLID Principles

### 4.1 Single Responsibility Principle (SRP)
Every component has exactly one reason to change. Core user management is isolated from sales workflows. The automated database auditing mechanism runs completely out-of-band, preventing system telemetry from polluting core transaction flows.

### 4.2 Open/Closed Principle (OCP)
The system is open for extension but closed for modification. Introducing a new discount rule, a new shipping carrier, or an alternative payment gateway vendor is achieved by creating a new class implementing their respective interfaces, requiring zero changes to existing checkout code.

### 4.3 Liskov Substitution Principle (LSP)
Subclasses like `PhysicalProduct` or `DigitalProduct` can be substituted for the base `Product` entity throughout the application layer without altering the correctness of inventory evaluations or catalog rendering.

### 4.4 Interface Segregation Principle (ISP)
Massive service contracts are broken down into narrow, highly specialized interfaces (e.g., `IOrderCheckoutService`, `IPOSOrderService`, `IOrderAnalyticsService`). Clients only depend on the precise operations they execute.

### 4.5 Dependency Inversion Principle (DIP)
High-level business use cases depend exclusively on abstractions. The concrete data access configurations (EF Core, SQL Server indexes) are injected into application interfaces via Dependency Injection at the infrastructure boundary.

---

## 5. Enterprise Multi-Tiered Governance (Hierarchical RBAC)
The application implements an advanced Role-Based Access Control (RBAC) architecture built on top of ASP.NET Core Identity (utilizing `int` Primary Keys for optimized relational indexing) to support multi-branch corporate structures.

### 5.1 Roles, Scopes & Capabilities Matrix

| Role | Business Scope | System Permissions & API Capabilities |
| :--- | :--- | :--- |
| **Super Admin / General Manager** | Global Corporate Level (God Mode) | Complete cross-branch control. Manages global roles, branch creations, system configurations, and views company-wide aggregated audit logs. |
| **General Accountant** | Corporate Financial Level | Read-only access to global financial analytics, profit margins, and sales ledger audits across all physical and online branches. Cannot modify data or users. |
| **Branch Admin / Local Manager** | Isolated Branch Level | Full administrative control strictly bounded to their assigned `BranchId`. Manages local staff, inventory levels, local discount campaigns, and branch-specific sales reports. |
| **Cashier** | Point of Sale (POS) Operations | Process In-Store Physical Orders within their specific branch. Generates print-ready receipts. Supports offline data queue syncing. |
| **Customer** | End-User Browsing & Ordering | Cart operations, online checkout, real-time shipment tracking, and submitting purchased-verified reviews. |

### 5.2 Dynamic Permission Management
The Super Admin possesses complete runtime control over the security schema, with endpoints dedicated to creating/removing security roles dynamically and modifying user access roles on-the-fly. It enforces a soft-lockout status (`IsActive` flag) to instantly revoke system access for suspended staff across specific physical branches without breaking database cascading references.

---

## 6. Advanced Business Logic & Distributed Infrastructure

### 6.1 Hybrid Online/Offline POS Data Synchronization
To combat unexpected network downtimes at brick-and-mortar stores, the POS architecture supports an offline queue mechanism. In-store cashiers can process physical orders completely offline. Transactions are queued locally in a client-side cache and securely dispatched to the main API via an automated data-synchronization pipeline as soon as connection is re-established, resolving database concurrency via idempotent tracking.

### 6.2 Intelligent Logistics & Shipping Routing Engine
Online customer orders are processed by an automated fulfillment engine. Upon payment confirmation, the `LogisticsRoutingService` calculates the closest physical branch containing sufficient inventory relative to the customer’s delivery address. The system reserves the stock at that specific branch and triggers the third-party carrier via the `IShippingAdapter` pipeline.

### 6.3 Dual-Layer Isolated Audit Tracking
The database architecture isolates security monitoring into two distinct auditing boundaries:
* **Local Branch Audit Scope:** Automatic logging tracking all `Create`, `Update`, or `Delete` commands executed by local Cashiers and Store Managers. This data is fully queryable by both the Branch Admin and the Super Admin.
* **Global Corporate Audit Scope:** Tracks sensitive configuration changes executed by Branch Admins and General Accountants (e.g., changing a user's role, modifying global tax settings). This log tier is encrypted and strictly accessible only by the Super Admin/General Manager.

### 6.4 Intelligent Double-Tier Review Guardrails
* **Product Reviews:** Users are restricted from rating products unless they possess a verified, successful `OrderItem` transaction record for that specific product ID within the database, completely preventing spam reviews.
* **Website Evaluations:** A delta-time evaluation rule checks the timestamp of a user's historical feedback, enforcing a strict 6-month cooling period before a user can resubmit a platform-wide review.

### 6.5 High-Performance Analytics Aggregations
To prevent the Admin Dashboard from lagging due to massive row calculations, the analytical queries are optimized using selective database indexing on temporal (`OrderDate`) and relational fields (`BranchId`, `Status`). Computations for complex matrices (revenues by demographic address, top-selling items per branch) are processed through a read-optimized service layout.