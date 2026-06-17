# 🔐 API Endpoints & Routing Matrix (Advanced Hierarchical RBAC)

This matrix defines every exposed HTTP endpoint, its business purpose, strict Role-Based Access Control (RBAC) rules, and system guardrails.

---

## 1. Identity & Corporate User Management
* **`POST /api/auth/register`**
    * **Access:** `[AllowAnonymous]`
    * **Purpose:** Allows public online customers to register.
* **`POST /api/auth/login`**
    * **Access:** `[AllowAnonymous]`
    * **Purpose:** Validates credentials; returns JWT with precise Hierarchical Roles.
* **`POST /api/superadmin/roles`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** Dynamically creates a new application security role.
* **`POST /api/superadmin/users/{id}/assign-role`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** Promotes/demotes users, changes system status, or reassigns roles globally.
* **`POST /api/branch-admin/staff/toggle-status/{id}`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Flips the `IsActive` flag to suspend/soft-lock staff. 
    * **Guardrail:** BranchAdmin can only suspend staff belonging strictly to their own `BranchId`.
* **`DELETE /api/customers/account`**
    * **Access:** `[Authorize(Roles = "Customer")]`
    * **Purpose:** **Self-Deletion Framework.** Allows an authenticated customer to permanently hard-delete their own profile from the platform.
* **`DELETE /api/superadmin/customers/{id}`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** **Administrative Hard Delete.** Allows the Super Admin to permanently wipe a customer profile upon official request.

---

## 2. Multi-Branch & Store Management
* **`POST /api/superadmin/branches`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** Registers a new physical store branch into the enterprise database.
* **`GET /api/superadmin/branches`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, GeneralAccountant")]`
    * **Purpose:** Lists all active physical store locations across the company.

---

## 3. Categories & Product Catalog Module (Factory Method Pattern)
* **`GET /api/products`**
    * **Access:** `[AllowAnonymous]`
    * **Purpose:** Paginated browsing and filtering for public users.
* **`POST /api/products`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Invokes `ProductFactory` to generate physical or digital products.
* **`PUT /api/products/{id}`** & **`DELETE /api/products/{id}`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
* **`POST /api/categories`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Adds a new core product category.
* **`DELETE /api/categories/{id}`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Deletes an empty category.
    * **Strict Guardrail:** The API instantly rejects the deletion and throws a validation error if the targeted category contains even a single active product mapping.

---

## 4. Advanced Inventory Control & Alert System (Observer Pattern)
* **`GET /api/inventory/low-stock`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Queries the inventory management system. 
    * **Guardrail:** Returns a real-time list of products that have dropped below their defined `LowStockThreshold`. BranchAdmin can only see alerts for their local store.
* **`POST /api/inventory/{id}/replenish`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Allows managers to restock a product, dynamically removing the automated "Out of Stock" lock tag and reopening the item for customer purchase.

---

## 5. Promotion & Dynamic Discount Engine (Strategy Pattern)
* **`POST /api/superadmin/discounts/global`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** Injects global promotion strategies (e.g., Seasonal Percentage Coupons) affecting all online checkouts.
* **`POST /api/branch-admin/discounts/local`**
    * **Access:** `[Authorize(Roles = "BranchAdmin")]`
    * **Purpose:** Applies localized strategy rules strictly to the admin's specific branch (e.g., Flash Sale or Buy-One-Get-One inside a specific brick-and-mortar location).

---

## 6. Order & Point-of-Sale Execution Module
* **`POST /api/orders/checkout`**
    * **Access:** `[Authorize(Roles = "Customer")]`
    * **Purpose:** Online customer checkout. Leverages `IDiscountStrategy`, `IPaymentAdapter`, and automates regional stock allocation via `LogisticsRoutingService`.
    * **Inventory Guardrail:** If `StockQuantity` hits zero during evaluation, the item status triggers an automatic "Out of Stock" lock, rejecting checkout pipelines and firing a background warning event.
* **`POST /api/orders/pos-checkout`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin, Cashier")]`
    * **Purpose:** Processes live in-store physical orders, deducts local stock, and issues print-ready receipts.
* **`POST /api/orders/pos-sync`**
    * **Access:** `[Authorize(Roles = "Cashier")]`
    * **Purpose:** **Offline Sync Engine.** Receives an array of locally cached offline transactions from the physical terminal and syncs them idempotently once network connection is restored.

---

## 7. Order Return Pipeline (State Pattern)
* **`POST /api/returns/submit`**
    * **Access:** `[Authorize(Roles = "Customer, Cashier")]`
    * **Purpose:** Submits an order item for return. Initializes the pipeline into `PendingInspectionState`.
* **`PUT /api/returns/{id}/process`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Payload:** `{ Action: "Approve" | "Reject", Comment: "..." }`
    * **Purpose:** Progresses the return state. If approved, shifts the object context to `ApprovedState`, triggering automatic local restocking and credit reversals via `IPaymentAdapter`.

---

## 8. Intelligent Guardrail Reviews Module
* **`POST /api/reviews/product`**
    * **Access:** `[Authorize(Roles = "Customer")]`
    * **Purpose:** Enforces purchase-verification logic. Blocked if the user has no successful transactional database history with the targeted product ID.
* **`POST /api/reviews/website`**
    * **Access:** `[Authorize(Roles = "Customer")]`
    * **Purpose:** Enforces a strict 6-month delta-time constraint before accepting a new platform-wide assessment.

---

## 9. Enterprise Business Intelligence & Audit Analytics
* **`GET /api/analytics/global-sales`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, GeneralAccountant")]`
    * **Purpose:** High-performance aggregation reports of revenues, profit margins, and sales ledger audits across all active branches.
* **`GET /api/analytics/branch-sales`**
    * **Access:** `[Authorize(Roles = "BranchAdmin")]`
    * **Purpose:** Read-only access to analytics strictly filtered and bounded to the manager's assigned `BranchId`.
* **`GET /api/analytics/audit-logs/local`**
    * **Access:** `[Authorize(Roles = "SuperAdmin, BranchAdmin")]`
    * **Purpose:** Queries telemetry logs of local operations (Cashier checkouts, product modifications).
* **`GET /api/analytics/audit-logs/global`**
    * **Access:** `[Authorize(Roles = "SuperAdmin")]`
    * **Purpose:** **Encrypted Scope.** Fetches high-level global corporate configurations (role changes, global discount shifts). Strictly off-limits to everyone except the SuperAdmin.