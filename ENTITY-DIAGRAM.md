# Ostad.Forum – Database / Entity Diagram

## Entity Relationship Diagram (Mermaid)

```mermaid
erDiagram
    Users {
        int UserId PK
        string Name
        string Email
        string PasswordHash
        datetime CreatedAt
    }

    Categories {
        int CategoryId PK
        string Name
        string Slug
        string Description "nullable"
        datetime CreatedAt
    }

    Tags {
        int TagId PK
        string Name
        string Slug
        datetime CreatedAt
    }

    Questions {
        int QuestionId PK
        int UserId FK
        int CategoryId FK
        string Title
        string Description
        int ViewCount
        datetime CreatedAt
        datetime UpdatedAt "nullable"
    }

    QuestionTags {
        int QuestionTagId PK
        int QuestionId FK
        int TagId FK
    }

    Answers {
        int AnswerId PK
        int QuestionId FK
        int UserId FK
        string Content
        datetime CreatedAt
    }

    Comments {
        int CommentId PK
        int AnswerId FK
        int UserId FK
        int ParentCommentId FK "nullable"
        string Content
        datetime CreatedAt
    }

    Votes {
        int VoteId PK
        int UserId FK
        int QuestionId FK "nullable"
        int AnswerId FK "nullable"
        int CommentId FK "nullable"
        int VoteType "enum: Upvote=1, Downvote=-1"
        datetime CreatedAt
    }

    AspNetUsers {
        string Id PK
        string UserName
        string NormalizedUserName
        string Email
        string NormalizedEmail
        bool EmailConfirmed
        string PasswordHash
        string SecurityStamp
        string ConcurrencyStamp
        string PhoneNumber
        bool PhoneNumberConfirmed
        bool TwoFactorEnabled
        datetimeoffset LockoutEnd "nullable"
        bool LockoutEnabled
        int AccessFailedCount
    }

    AspNetRoles {
        string Id PK
        string Name
        string NormalizedName
        string ConcurrencyStamp
    }

    AspNetUserRoles {
        string UserId PK,FK
        string RoleId PK,FK
    }

    Users ||--o{ Questions : "asks"
    Users ||--o{ Answers : "writes"
    Users ||--o{ Comments : "writes"
    Users ||--o{ Votes : "casts"

    Categories ||--o{ Questions : "contains"

    Questions ||--o{ QuestionTags : "has"
    Tags ||--o{ QuestionTags : "tagged"
    QuestionTags }o--|| Questions : ""
    QuestionTags }o--|| Tags : ""

    Questions ||--o{ Answers : "has"
    Questions ||--o{ Votes : "votes on"

    Answers ||--o{ Comments : "has"
    Answers ||--o{ Votes : "votes on"

    Comments ||--o| Comments : "replies to"
    Comments ||--o{ Votes : "votes on"

    AspNetUsers ||--o{ AspNetUserRoles : ""
    AspNetRoles ||--o{ AspNetUserRoles : ""
```

## Table Summary

| Table           | Purpose |
|-----------------|---------|
| **Users**       | Forum user profile (display name, email); links to content. Identity is in AspNetUsers. |
| **Categories**  | Question categories (e.g. Technology). |
| **Tags**        | Tags for questions (many-to-many via QuestionTags). |
| **Questions**   | Main question post (title, description, view count). |
| **QuestionTags**| Many-to-many link between Questions and Tags. |
| **Answers**     | Answers to a question. |
| **Comments**    | Comments on an answer; optional ParentCommentId for nested replies. |
| **Votes**       | Up/down votes on a Question, Answer, or Comment (one of the three IDs set). |
| **AspNetUsers** | ASP.NET Identity – login, password, email. |
| **AspNetRoles** | Roles (e.g. Admin). |
| **AspNetUserRoles** | User–Role assignment. |

*(Other Identity tables: AspNetRoleClaims, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens are part of the same database.)*

## Vote target

Each **Vote** row has exactly one of **QuestionId**, **AnswerId**, or **CommentId** set (the other two are null). Unique indexes enforce one vote per user per question, per user per answer, and per user per comment.
