# Ostad.Forum – N-Tier & Repository Pattern

## Layers

| Layer | Project | Responsibility |
|-------|---------|-----------------|
| **Presentation** | Ostad.Forum.Web | Controllers, Views, ViewModels. Uses only BLL services and Contract DTOs. No direct DAL or Domain references in controllers. |
| **Business Logic** | Ostad.Forum.BLL | Services (interfaces in BLL.Interfaces, implementations in BLL.Services). Orchestrates use cases using Unit of Work. Returns Contract DTOs to the presentation layer. |
| **Data Access** | Ostad.Forum.DAL | Unit of Work, generic and specific repositories, DbContext. Exposes only repository interfaces and Unit of Work to BLL. |
| **Domain** | Ostad.Forum.Domain | Entities and enums. No dependencies on other projects. |
| **Contract** | Ostad.Forum.Contract | DTOs shared between BLL and Web (e.g. QuestionDetailsDto, CategoryDto). No dependencies. |

## Repository Pattern

- **IUnitOfWork** (DAL): Exposes repositories (Users, Questions, Categories, Tags, Answers, Comments, Votes, QuestionTags) and `SaveChangesAsync()`.
- **IGenericRepository&lt;T&gt;** (DAL): `GetByIdAsync`, `GetAllAsync`, `FindAsync`, `Query()`, `AddAsync`, `Update`, `Remove`.
- **Specific repositories** (e.g. IQuestionRepository) extend IGenericRepository for a concrete entity.
- **UnitOfWork** (DAL) is the single place that constructs repositories and shares one DbContext. Only BLL uses IUnitOfWork; Web does not.

## Data Flow

1. **Web** → calls **BLL** service interfaces (IQuestionService, ICategoryService, IVoteService, etc.).
2. **BLL** → uses **IUnitOfWork** to access repositories and coordinates transactions; maps Domain entities to **Contract** DTOs.
3. **DAL** → repositories and DbContext perform persistence; Domain entities stay inside BLL/DAL.
4. **Web** → receives DTOs from BLL and maps them to ViewModels for views.

## Dependency Direction

- Web → BLL, Contract (and DAL only in Program.cs for DbContext/UnitOfWork registration).
- BLL → DAL, Domain, Contract.
- DAL → Domain only.

Controllers do not reference Ostad.Forum.DAL or Ostad.Forum.Domain; they depend only on BLL interfaces and Contract/ViewModels.
