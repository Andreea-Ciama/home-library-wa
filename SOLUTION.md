# Home Library – Solution

## How to run

### Prerequisites

Before running the application, make sure the following software is installed:

- Docker Desktop
- Docker Compose

### Build and start the application

From the root of the repository, run:

```bash
docker compose up -d --build
```

Docker Compose builds the API, Worker and Angular application, creates the required containers, and starts the entire system automatically.

The application will then be available at:

| Service | URL |
|---------|-----|
| Angular UI | http://localhost:4200 |
| ASP.NET API | http://localhost:5000 |
| RabbitMQ Management | http://localhost:15672 |
| PostgreSQL | localhost:5432 |

RabbitMQ credentials:

- Username: `guest`
- Password: `guest`

### Running the tests

Navigate to the `src` folder and execute:

```bash
dotnet test
```

This runs all unit tests for the solution.

---

# Design decisions

The application is split into multiple projects in order to keep responsibilities separated.

### HomeLibrary.Api

The API is responsible for:

- exposing REST endpoints;
- accepting CSV uploads;
- reading and validating CSV files;
- publishing one RabbitMQ message for each imported book;
- returning the list of books stored in the database.

The API does not insert books directly into the database. Instead, it publishes messages, allowing imports to be processed asynchronously.

---

### HomeLibrary.Worker

The Worker continuously listens to RabbitMQ.

Whenever a new message arrives, it:

1. deserializes the message;
2. creates a new `Book` object;
3. stores it in PostgreSQL using Entity Framework Core.

Separating this logic from the API keeps the application more scalable and prevents long-running imports from blocking HTTP requests.

---

### HomeLibrary.Contracts

This project contains the shared classes used by both the API and the Worker, such as:

- Book model;
- RabbitMQ message contracts.

Using a shared project avoids duplicated models and guarantees that both applications use exactly the same data structures.

---

### Angular Frontend

The Angular application provides a simple user interface where users can:

- upload CSV files;
- view all imported books.

The UI periodically refreshes the list by requesting the API every few seconds so newly imported books appear automatically.

---

### Database

PostgreSQL is used as the primary database.

Entity Framework Core is used for:

- database access;
- migrations;
- object-relational mapping.

Books use **GUID identifiers** instead of integer IDs. GUIDs are globally unique and avoid relying on database-generated identity values, making them suitable for distributed applications.

---

### Messaging

RabbitMQ is used to decouple the API from the import process.

The workflow is:

```
CSV Upload
     │
     ▼
API reads CSV
     │
     ▼
RabbitMQ queue
     │
     ▼
Worker
     │
     ▼
PostgreSQL
     │
     ▼
Angular displays books
```

This architecture makes the system more resilient and allows the Worker to be scaled independently from the API.

---

# Assumptions

The implementation assumes that:

- every CSV file contains the columns:
  - Name
  - Author
  - Genre
- every row represents exactly one book;
- RabbitMQ and PostgreSQL are available through Docker Compose;
- only valid CSV files are uploaded.

---

# Improvements with more time

If I had more time, I would:

- improve the user interface;
- add more automated tests;
- improve error handling and CSV validation;
- add user authentication.