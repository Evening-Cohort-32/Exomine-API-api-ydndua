# 🪐 Exomine API

---

## 1. Project Summary

Build a web API that replaces the `json-server` + `database.json` backend used by the original Exomine client. The API must support all existing client behavior (governors purchasing minerals from mining facilities) and expand the domain to be a more complete, realistic system of record, not just a pass-through for the client's exact calls.

---

## 2. Project Setup

Follow the [Web API setup instructions](https://github.com/Evening-Cohort-32/server-side-dotnet-curriculum/blob/main/book-2-web-apis/chapters/honeyrae-01-web-api-setup.md) from the .NET curriculum to create a Web API project called `ExomineAPI`. Delete the weather-forecast template code once it's running.

### 2.1 Directory Structure

Once setup is complete, your project should be organized roughly as follows. Exact naming can vary by team convention, but every model below needs a home.

```
ExomineAPI/
├── Models/
│   ├── Governor.cs
│   ├── Colony.cs
│   ├── MiningFacility.cs
│   ├── Mineral.cs
│   ├── ColonyInventory.cs
│   ├── FacilityInventory.cs
│   ├── GovernorHistory.cs
│   ├── Transaction.cs
│   └── DTOs/
│       ├── GovernorDTO.cs
│       ├── ColonyDTO.cs
│       ├── MiningFacilityDTO.cs
│       ├── MineralDTO.cs
│       ├── ColonyInventoryDTO.cs
│       ├── FacilityInventoryDTO.cs
│       ├── GovernorHistoryDTO.cs
│       └── TransactionDTO.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── ExomineAPI.csproj
```

Notes on the layout:

- `Program.cs` is where your in-memory collections (the "database" for this project) are declared and seeded, and where your endpoints are mapped.
- Everything under `Models/` is an internal representation of your data; everything under `Models/DTOs/` is what actually crosses the wire to the client. Keep them separate even when a DTO's properties look identical to its model. See section 3.2.

---

## 3. Domain & Data Requirements

### 2.1 Entities

**Governor**
- Has a name.
- Belongs to exactly one Colony at a time (a governor cannot govern more than one colony simultaneously).
- Has a status of `active` or `inactive`. Only active governors are relevant to colony operations.
- Must record when a governor's status changes (see 2.4, Governor History).

**Colony**
- Has a name and a location (e.g., "Mars", "Europa").
- Can have zero or more governors associated with it over time, and one or more *active* governors at once.
- Holds an inventory of minerals (quantities on hand).

**MiningFacility**
- Has a name.
- Has a status of `active` or `inactive`.
- Holds an inventory of minerals available for sale (quantities on hand).
- If inactive, no purchase against it should be permitted regardless of what the client sends.

**Mineral**
- Has a name (e.g., "Iron", "Magnesium") and should be unique by name.
- Is associated with many mining facilities (as a producible resource) and many colonies (as a purchasable/held resource). This is a many-to-many relationship in both directions and requires join entities with a quantity attribute, not a simple lookup table.

**ColonyInventory** *(join entity)*
- Links a Colony, a Mineral, and a quantity on hand.

**FacilityInventory** *(join entity)*
- Links a MiningFacility, a Mineral, and a quantity available for sale.

**GovernorHistory** *(new)*
- Records status changes for a governor: which governor, which colony, previous status, new status, and a timestamp.
- Exists so that "a governor took a leave of absence in March" is a queryable fact, not just an overwritten field.

**Transaction** *(new)*
- Records every mineral purchase: governor, colony, facility, mineral, quantity, and timestamp.
- This is the audit trail for the core purchase feature, and a prerequisite for several stretch goals below.

### 2.2 Relationships to Model Explicitly

- One Colony has many Governors.
- MiningFacility and Mineral are many-to-many, via FacilityInventory, with quantity.
- Colony and Mineral are many-to-many, via ColonyInventory, with quantity.
- One Governor has many GovernorHistory records.
- One Governor has many Transactions.
- One Colony has many Transactions.
- One MiningFacility has many Transactions.
- One Mineral has many Transactions.

### 2.3 Data Population

- Seed data must be migrated from the old `database.json` into the new collections/tables at startup, preserving existing relationships.
- Seed data should include at least one inactive governor, one inactive mining facility, and at least one mineral with a zero quantity somewhere, so that status/quantity rules are exercisable immediately.

### 2.4 Business Rules

- A governor's status change must be reflected in GovernorHistory automatically. This should not be something the client has to remember to do in a second request.
- A mineral with a quantity of 0 at a facility must still be returned by the API (the client, not the API, decides whether to hide it), but the API must reject a purchase against a 0-quantity mineral.
- A purchase against an inactive facility must be rejected regardless of quantity.
- A purchase must be an atomic operation from the API's point of view: the facility's inventory decrements and the colony's inventory increments together, or neither happens.
- Quantities may never go negative.

---

## 3. API Requirements

### 3.1 Resource Endpoints

For each of the following resources, the API must support the standard CRUD operations (Get All, Get by Id, Create, Update, Delete), unless a restriction is noted:

- Governors
- Colonies
- MiningFacilities
- Minerals
- ColonyInventory
- FacilityInventory
- GovernorHistory (read-only, created only as a side effect of governor status updates, never directly writable)
- Transactions (read-only, created only as a side effect of the purchase endpoint, never directly writable)

### 3.2 DTOs

- Every model must have a corresponding DTO used on all API boundaries; internal models should never be serialized directly to the client.
- Related data (e.g., a colony's inventory, a governor's colony) should be shaped into the DTO deliberately. Decide per-endpoint whether related data is nested, referenced by id only, or omitted, rather than defaulting to one style everywhere.

### 3.3 The Purchase Endpoint

- A single endpoint must perform the "purchase mineral" operation described in the original client requirements: given a governor, a facility, and a mineral, decrement the facility's inventory by 1 ton and increment the requesting governor's colony's inventory by 1 ton.
- This must use the PUT method, as in the original spec, and must create a Transaction record.
- The endpoint must validate: facility is active, governor is active, mineral has sufficient quantity at the facility, and the governor and facility both belong to entities that actually exist. Return the appropriate error status for each failure case rather than a generic error.

### 3.4 Query Support

- Get All endpoints for Governors, Colonies, MiningFacilities, and Minerals must support filtering by their status/active fields where applicable (e.g., `?active=true`).
- Get All for Transactions must support filtering by governor, colony, facility, mineral, and a date range.

### 3.5 Error Handling

- The API must return meaningful HTTP status codes (400, 404, 409, etc.) and a consistent error response shape across all endpoints, not just uncaught 500s.
- Validation errors (e.g., missing required fields, invalid foreign keys) must be caught before they reach the data layer.

---

## 4. 🚀 Stretch Goals

**Do not start these until the requirements above are fully working. MVP first.** Everything below assumes only what's been covered so far: console apps and a simple, in-memory-collection Web API. Nothing here requires a database, authentication, background services, or deployment pipelines. Those are good goals for a later project, not this one.

### 4.1 Client-facing stretch (extends what the client can do)

1. Better, faster, stronger CSS, possibly with a framework.
2. Refactor the client to allow a governor to purchase minerals from multiple facilities before finalizing a single "cart" of purchases (this is the original client's own stretch goal).
3. Add a governor-facing view of their own Transaction history.
4. Add search/filter controls in the client that make use of the new query support in 3.4.

### 4.2 API-only stretch (less tied to the existing client)

These push the API to do more on its own, using nothing beyond what a simple Web API and some LINQ can do. No new frameworks or infrastructure required.

1. **More query string support.** Beyond the `?active=true` filtering already required, add sorting (`?sortBy=name`) and simple text search (`?search=iron`) to Get All endpoints, all done with LINQ over the in-memory collections.
2. **Reporting endpoints.** Add a few read-only endpoints that compute something rather than just returning a collection, e.g., total tons currently held by a colony across all minerals, or which facility has the lowest inventory of a given mineral. This is a good exercise in LINQ aggregation (`Sum`, `GroupBy`, `OrderBy`) without touching the data model.
3. **Better validation with data annotations.** Add validation attributes to your DTOs (`[Required]`, `[Range]`, etc.) so bad requests are rejected automatically with a 400, before your own code ever runs.
4. **Consistent, deliberate status codes.** Go back through every endpoint and make sure it returns the right code on purpose: 201 with a location header on create, 204 on delete, 404 when an id doesn't exist, and 400 on bad input, rather than whatever the framework defaults to.
5. **Simple request logging.** Write a small piece of middleware that logs each incoming request (method, path, timestamp) to a text file. This only needs the file I/O you already know from console apps, applied inside the API pipeline.
6. **A companion console app.** Build a separate console application that talks to your API over HTTP (using `HttpClient`) to seed data, reset the database to a known state, or print a quick report. This reinforces both projects and is good practice consuming your own API as a client.
7. **Explore and clean up Swagger.** ASP.NET's Web API template generates Swagger/OpenAPI docs for free. Go through it, add summaries/descriptions to your endpoints, and make sure it's actually usable by a teammate who hasn't read your code.

---

## 5. Definition of Done for MVP

Before moving to any stretch goal, the team should be able to say yes to all of the following:

- [ ] All entities in section 2.1 exist as models, DTOs, and (if applicable) join entities.
- [ ] All CRUD endpoints in 3.1 exist and return DTOs, not raw models.
- [ ] The purchase endpoint (3.3) enforces every validation rule listed and creates a Transaction.
- [ ] Governor status changes create a GovernorHistory record automatically.
- [ ] Seed data is loaded from the original `database.json` and includes at least one inactive governor, one inactive facility, and one zero-quantity mineral.
- [ ] The original Exomine client, pointed at this API instead of json-server, works without modification to its core purchase flow.
