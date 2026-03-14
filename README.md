# Ostad Forum

A Q&A forum web application built with **ASP.NET Core 8 MVC**, following **N-Tier architecture** and the **Repository pattern**. Users can ask questions, post answers, vote on questions and answers, and browse by categories and tags. Admin can manage categories.

---

## Features

- **Questions & Answers** – Create questions with category and tags; post answers (authenticated users).
- **Voting** – Upvote/downvote on questions and answers; score displayed per question and answer.
- **Categories & Tags** – Questions belong to a category; multiple tags per question (many-to-many). Sidebar lists categories and tags (clickable placeholders for future filtering).
- **Authentication** – ASP.NET Core Identity (Login, Register, cookie auth). Forum user profile (Users table) linked by email for display names.
- **Admin area** – Category management (Create, List) for users in **Admin** role.
- **Home page** – Filter bar (Search, Category, Tag, Sort – UI only for now), latest questions, right sidebar with Categories and Tags cards.
- **Modern UI** – Colourful header, profile dropdown, Bootstrap 5, card-based layout.

---

## Tech Stack

| Layer / Area   | Technology |
|----------------|------------|
| Framework      | ASP.NET Core 8 (.NET 8) |
| UI             | MVC, Razor Views, Bootstrap 5 |
| Auth           | ASP.NET Core Identity (IdentityDbContext merged into ForumDbContext) |
| Data access    | Entity Framework Core 8, SQL Server |
| Architecture   | N-Tier (Web → BLL → DAL), Repository pattern, Unit of Work |

---

## Solution Structure

| Project | Description |
|---------|-------------|
| **Ostad.Forum.Web** | ASP.NET Core MVC app – controllers, views, view models, startup. |
| **Ostad.Forum.BLL** | Business logic – service interfaces and implementations (e.g. QuestionService, VoteService, CategoryService). |
| **Ostad.Forum.DAL** | Data access – `ForumDbContext`, generic/specific repositories, Unit of Work, EF Core migrations. |
| **Ostad.Forum.Domain** | Domain entities and enums (Question, Answer, User, Category, Tag, Vote, Comment, etc.). |
| **Ostad.Forum.Contract** | DTOs used between BLL and Web (e.g. QuestionDetailsDto, CategoryDto, TagDto). |

For dependency flow and repository usage, see [ARCHITECTURE.md](ARCHITECTURE.md).

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full) with a database for the app

---

## Getting Started

### 1. Clone and restore

```bash
git clone <repository-url>
cd Ostad.Forum
dotnet restore
```

### 2. Configure connection string

Edit **Ostad.Forum.Web/appsettings.json** and set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OstadForumDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Use `User Id=...;Password=...` if you rely on SQL authentication.

### 3. Apply migrations and run

Migrations are applied automatically on startup. Ensure the database server is running, then:

```bash
dotnet run --project Ostad.Forum.Web
```

Or from Visual Studio: set **Ostad.Forum.Web** as the startup project and run (F5).

The app will:

- Apply pending EF Core migrations (creates/updates tables).
- Seed an **Admin** role and an admin user (if not present).
- Seed a default **Technology** category (if not present).

### 4. Default admin account (after first run)

| Field    | Value |
|----------|--------|
| Email    | `admin@ostadforum.com` |
| Password | `Admin@123` |
| Role     | Admin |

Use this to access the **Admin** area and manage categories.

---

## Database and migrations

- **Database name:** `OstadForumDb` (or as in your connection string).
- **Migrations:** Stored in **Ostad.Forum.DAL/Migrations**. Context: `ForumDbContext`.

**Add a new migration (Package Manager Console in Visual Studio):**

```powershell
Add-Migration YourMigrationName -Project Ostad.Forum.DAL -StartupProject Ostad.Forum.Web -Context ForumDbContext
```

**Update database:**

```powershell
Update-Database -Project Ostad.Forum.DAL -StartupProject Ostad.Forum.Web -Context ForumDbContext
```

**CLI (from solution folder):**

```bash
dotnet ef migrations add YourMigrationName -p Ostad.Forum.DAL -s Ostad.Forum.Web --context ForumDbContext
dotnet ef database update -p Ostad.Forum.DAL -s Ostad.Forum.Web --context ForumDbContext
```

For table list and relationships, see [ENTITY-DIAGRAM.md](ENTITY-DIAGRAM.md).

---

## Project layout (high level)

```
Ostad.Forum/
├── Ostad.Forum.sln
├── README.md
├── ARCHITECTURE.md          # N-Tier and repository pattern
├── ENTITY-DIAGRAM.md        # Database/entity diagram (Mermaid)
├── Ostad.Forum.Web/         # MVC app
│   ├── Controllers/
│   ├── Views/
│   ├── Models/              # View models
│   ├── Data/                # IdentitySeeder, etc.
│   ├── Areas/Admin/         # Admin area (categories)
│   └── wwwroot/
├── Ostad.Forum.BLL/
│   ├── Interfaces/          # IQuestionService, IVoteService, ICategoryService, ITagService, etc.
│   └── Services/
├── Ostad.Forum.DAL/
│   ├── ForumDbContext.cs
│   ├── Interfaces/          # IUnitOfWork, IGenericRepository, entity repositories
│   ├── Repositories/
│   ├── UnitOfWork/
│   └── Migrations/
├── Ostad.Forum.Domain/
│   └── Entities/            # Question, Answer, User, Category, Tag, Vote, Comment, QuestionTag, VoteType
└── Ostad.Forum.Contract/
    └── *.Dto.cs             # DTOs for API/view boundaries
```

---

## Main flows

- **Home** – Lists latest questions; filter bar (Search, Category, Tag, Sort) and sidebar (Categories, Tags) are ready for you to wire to filtering.
- **Question details** – View question, answers, vote on question and each answer; post a new answer when logged in.
- **Ask question** – Pick category, select/create tags, submit (authenticated).
- **Profile (navbar)** – Dropdown with signed-in user, placeholder links (My Questions, My Answers), and Logout.
- **Admin** – Manage categories (list, create); requires **Admin** role.

---

## Architecture and diagram

- **Architecture (N-Tier, Repository, data flow):** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Database / entity diagram (Mermaid):** [ENTITY-DIAGRAM.md](ENTITY-DIAGRAM.md)

---

## License

Use as needed for the Ostad ASP.NET Batch 7 course or related learning.
