# Ostad Forum

A **Q&A forum** web application built with **ASP.NET Core 8 MVC**, following **N-Tier architecture** and the **Repository pattern**. Learners and instructors can ask questions, categorize them, tag them for discovery, answer threads, vote on content, and discuss answers via nested comments. Administrators manage the category catalog.

---

## Business domain

### Purpose

Ostad Forum supports a **community Q&A model** similar to Stack Overflow–style sites:

- **Askers** post **questions** with a title, full description, one **category**, and zero or more **tags** so others can find and filter content.
- **Contributors** post **answers** to questions; readers can **vote** on questions, answers, and comments to surface quality.
- **Moderation / taxonomy** is handled by **admins**, who maintain **categories** (broad buckets like “Technology” or “Career”).
- **Tags** are a flexible, many-to-many facet on questions (e.g. `C#`, `Career`) for finer filtering than a single category.

### Core business rules (as implemented)

| Concept | Rule |
|--------|------|
| Question | Must belong to exactly one **Category** and one **User** (forum profile). Has **ViewCount** incremented when details are opened. |
| Tags | Optional; a question links to many tags through **QuestionTags** (junction). New tag names can be created at ask time (BLL deduplicates by name/slug). |
| Answer | Belongs to one question and one user; ordered by creation time on the details page. |
| Comment | Belongs to one **Answer**; optional **ParentCommentId** enables threaded replies. |
| Vote | One row targets **either** a question, an answer, or a comment (`VoteType` up/down). Database unique filtered indexes enforce **one vote per user per target**. |
| Identity vs forum user | **Login** uses **ASP.NET Identity** (`AspNetUsers`). **Display/ownership** for forum entities uses the domain **Users** table, often matched by **email** when posting content. |
| Admin | Users in the **Admin** **Identity** role access the **Admin** area to create/list **Categories**. |

### Personas

- **Guest** – Browse home (with search/filter), read question details.
- **Member** (authenticated) – Ask questions, answer, vote, see profile menu.
- **Admin** – Same as member plus category management.

---

## Features

- **Questions & answers** – Create questions with category and tags; post answers (authenticated users).
- **Voting** – Upvote/downvote on questions, answers, and comments; scores shown on the UI.
- **Categories & tags** – Questions belong to one category; many-to-many tags via `QuestionTags`. Home page **filter bar** (Search, Category, Tag, Sort with **Apply** / **Clear**) and **sidebar** (clickable categories and tags) filter the latest-questions list; sidebar updates the dropdowns and applies filters immediately.
- **Authentication** – ASP.NET Core Identity (login, register, cookie auth). Forum **Users** table stores profile-style data for content ownership.
- **Admin area** – Category management (create, list) for users in the **Admin** role.
- **Home page** – Latest questions (configurable take in BLL), search/sort/filter UX, sidebar navigation.
- **UI** – Bootstrap 5, card layout, colourful header and sidebar cards.

---

## How it is implemented

### Layered architecture

Data flows **Web → BLL → DAL → SQL Server**:

1. **Ostad.Forum.Web** – MVC controllers accept HTTP requests, build **view models**, call **BLL interfaces**, return Razor views.
2. **Ostad.Forum.BLL** – **Services** (`QuestionService`, `AnswerService`, `VoteService`, `CategoryService`, `TagService`) implement use cases: load DTOs, enforce rules, orchestrate repositories via **IUnitOfWork**.
3. **Ostad.Forum.DAL** – **ForumDbContext** (EF Core), **repositories**, **UnitOfWork** commit **`SaveChangesAsync`** once per operation where needed.
4. **Ostad.Forum.Domain** – POCO **entities** and enums (no EF attributes required; mapping is fluent in `ForumDbContext`).
5. **Ostad.Forum.Contract** – **DTOs** exchanged between Web and BLL (e.g. `QuestionListItemDto`, `QuestionDetailsDto`, `CategoryDto`) to avoid leaking persistence types into the UI layer.

Controllers stay thin; business rules live in BLL services.

### Example: loading the home page

1. `HomeController.Index` resolves `IQuestionService`, `ICategoryService`, `ITagService`.
2. `QuestionService.GetLatestQuestionsAsync` queries questions with **`Include`** for `Category`, `Answers`, and **`QuestionTags` → `Tag`**, maps to `QuestionListItemDto` (including **`CategoryId`** and **`TagIds`** for filtering).
3. Categories and tags are loaded for dropdowns and sidebar.
4. `HomeIndexViewModel` is passed to `Views/Home/Index.cshtml`.

### Example: search, filter, and sort on the home page

Implementation is **client-side** on the rendered list (same question set returned from the server, default take = 20 in `GetLatestQuestionsAsync`):

- Each question card exposes **`data-*`** attributes: `data-category-id`, `data-tag-ids` (comma-separated), `data-title`, `data-created`, `data-answers`, `data-views`.
- A script on **`Index.cshtml`** (`@section Scripts`):
  - **Apply** – Matches **title** (case-insensitive substring), **category** (if selected), **tag** (if selected); **sort** reorders visible cards (newest, oldest, most answers, most views).
  - **Clear** – Resets all controls and shows the full list.
  - **Sidebar** – Category links set the category dropdown (and clear the opposing tag filter for a clean “facet” click); tag pills do the symmetric thing; clicking the same facet again clears it.
- To scale beyond the loaded page (server-side search, pagination, or URL query parameters), you would add a BLL method with **`IQueryable`** filters and optional parameters, then either return a partial view or JSON—reusing the same relation model described below.

### Example: creating a question

1. Authenticated user submits `QuestionController.Create` POST.
2. BLL **`CreateQuestionAsync`** resolves or creates a domain **`User`** by email, inserts **`Question`**, then **`QuestionTag`** rows for selected and newly named tags.
3. **`UnitOfWork.SaveChangesAsync`** persists the transaction.

### Example: voting

`VoteService` adds or updates **`Vote`** rows, respecting unique indexes (one vote per user per question/answer/comment). Scores are aggregated when building DTOs for the details view.

### Database access and migrations

- **`Program.cs`** registers **`ForumDbContext`** with SQL Server and runs **`Database.MigrateAsync()`** on startup.
- **`IdentitySeeder`** seeds **Admin** role and default admin user; category seeding may run as part of your data setup (see seeder code under **Web/Data**).

For a deeper walkthrough of repositories and the unit of work, see [ARCHITECTURE.md](ARCHITECTURE.md).

---

## Database: tables and relationships

The app uses **one SQL Server database** managed by **`ForumDbContext`**, which inherits **`IdentityDbContext`** so **Identity** tables and **forum** tables coexist.

### Forum tables (domain)

| Table | Primary key | Main foreign keys | Relationship summary |
|-------|-------------|-------------------|----------------------|
| **Users** | `UserId` | — | One user has many **Questions**, **Answers**, **Comments**, **Votes**. |
| **Categories** | `CategoryId` | — | One category has many **Questions**. |
| **Tags** | `TagId` | — | Many-to-many with **Questions** via **QuestionTags**. |
| **Questions** | `QuestionId` | `UserId` → **Users**, `CategoryId` → **Categories** | Each question has one author and one category; many **Answers**, **QuestionTags**, **Votes** (question votes). |
| **QuestionTags** | `QuestionTagId` | `QuestionId` → **Questions**, `TagId` → **Tags** | Junction table: **many questions ↔ many tags**. Indexed on both FKs. |
| **Answers** | `AnswerId` | `QuestionId` → **Questions**, `UserId` → **Users** | Many answers per question; many **Comments** and **Votes** (answer votes). |
| **Comments** | `CommentId` | `AnswerId` → **Answers**, `UserId` → **Users**, `ParentCommentId` → **Comments** (nullable) | Self-referencing tree for **replies**; **`OnDelete: Restrict`** on parent to avoid cascade cycles. |
| **Votes** | `VoteId` | `UserId` → **Users**; optional `QuestionId`, `AnswerId`, `CommentId` | Polymorphic target: exactly one of the three target IDs should be set per row. **Filtered unique indexes** on `(UserId, QuestionId)`, `(UserId, AnswerId)`, `(UserId, CommentId)` prevent duplicate votes. |

### Identity tables (ASP.NET Core Identity)

| Table | Role |
|-------|------|
| **AspNetUsers** | Login credentials, normalized email/username, security stamp, etc. |
| **AspNetRoles** | Named roles (e.g. Admin). |
| **AspNetUserRoles** | Many-to-many **User ↔ Role**. |
| **AspNetUserClaims**, **AspNetRoleClaims**, **AspNetUserLogins**, **AspNetUserTokens** | Standard Identity extensibility. |

**Note:** Domain **Users** and **AspNetUsers** are separate: the app links them by convention (e.g. email) when creating forum content; they are **not** a single shared PK.

### Delete behaviors (EF configuration highlights)

- **Question** → **User** / **Category**: **`Restrict`** (cannot delete a user or category if referenced).
- **Answer** / **Comment** → **User**: **`Restrict`**.
- **Vote** → **User**: **`Cascade`** (if a user row were removed, votes go away—typical for test data; adjust for production policies).
- **Vote** → **Question** / **Answer** / **Comment**: **`NoAction`** to avoid multiple cascade paths on SQL Server.

### Visual ER diagram

A **Mermaid** entity-relationship diagram (same schema) lives in [ENTITY-DIAGRAM.md](ENTITY-DIAGRAM.md).

---

## Tech stack

| Layer / area | Technology |
|--------------|------------|
| Framework | ASP.NET Core 8 (.NET 8) |
| UI | MVC, Razor Views, Bootstrap 5 |
| Auth | ASP.NET Core Identity (`IdentityDbContext` merged into `ForumDbContext`) |
| Data access | Entity Framework Core 8, SQL Server |
| Architecture | N-Tier (Web → BLL → DAL), Repository pattern, Unit of Work |

---

## Solution structure

| Project | Description |
|---------|-------------|
| **Ostad.Forum.Web** | ASP.NET Core MVC – controllers, views, view models, startup, Identity seeder. |
| **Ostad.Forum.BLL** | Business logic – service interfaces and implementations. |
| **Ostad.Forum.DAL** | `ForumDbContext`, repositories, Unit of Work, EF Core migrations. |
| **Ostad.Forum.Domain** | Domain entities and enums. |
| **Ostad.Forum.Contract** | DTOs between BLL and Web. |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full) with a database for the app

---

## Getting started

### 1. Clone and restore

```bash
git clone <repository-url>
cd Ostad.Forum
dotnet restore
```

### 2. Configure connection string

Edit **Ostad.Forum.Web/appsettings.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OstadForumDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Use `User Id=...;Password=...` for SQL authentication.

### 3. Run

```bash
dotnet run --project Ostad.Forum.Web
```

Or set **Ostad.Forum.Web** as the startup project in Visual Studio (F5).

The app applies pending migrations on startup and seeds admin (see seeder).

### 4. Default admin (after first run)

| Field | Value |
|-------|--------|
| Email | `admin@ostadforum.com` |
| Password | `Admin@123` |
| Role | Admin |

Use this for the **Admin** area (categories).

---

## Migrations

- **Context:** `ForumDbContext` in **Ostad.Forum.DAL**.
- **Folder:** `Ostad.Forum.DAL/Migrations`.

**Package Manager Console:**

```powershell
Add-Migration YourMigrationName -Project Ostad.Forum.DAL -StartupProject Ostad.Forum.Web -Context ForumDbContext
Update-Database -Project Ostad.Forum.DAL -StartupProject Ostad.Forum.Web -Context ForumDbContext
```

**CLI:**

```bash
dotnet ef migrations add YourMigrationName -p Ostad.Forum.DAL -s Ostad.Forum.Web --context ForumDbContext
dotnet ef database update -p Ostad.Forum.DAL -s Ostad.Forum.Web --context ForumDbContext
```

---

## Project layout (high level)

```
Ostad.Forum/
├── Ostad.Forum.sln
├── README.md
├── ARCHITECTURE.md
├── ENTITY-DIAGRAM.md
├── Ostad.Forum.Web/
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   ├── Data/
│   ├── Areas/Admin/
│   └── wwwroot/
├── Ostad.Forum.BLL/
│   ├── Interfaces/
│   └── Services/
├── Ostad.Forum.DAL/
│   ├── ForumDbContext.cs
│   ├── Interfaces/
│   ├── Repositories/
│   ├── UnitOfWork/
│   └── Migrations/
├── Ostad.Forum.Domain/
│   └── Entities/
└── Ostad.Forum.Contract/
```

---

## Main user flows

- **Home** – Latest questions; search, category, tag, sort; sidebar category/tag shortcuts.
- **Question details** – Full question, answers, comments, voting; post answer when logged in.
- **Ask question** – Category, tags (existing + new names), submit (authenticated).
- **Admin** – Categories (list, create) for **Admin** role.

---

## Further reading

- [ARCHITECTURE.md](ARCHITECTURE.md) – N-Tier flow, repositories, unit of work.
- [ENTITY-DIAGRAM.md](ENTITY-DIAGRAM.md) – Mermaid ER diagram and table summary.

---

## License

Use as needed for the Ostad ASP.NET Batch 7 course or related learning.
