# Task 2 - Refactoring

## Assumption
The current code did not make use of SOLID principles, as everything was put into one controller.

## Decision
I decided to implement the clean architecture pattern as much as possible, making use of Mediatr and CQRS where I saw fit.

## Reason
The single controller contained all the business logic, making it difficult to maintain, test, and extend. Clean architecture separates concerns into distinct layers (e.g., Presentation, Application, Domain, and Infrastructure). This enhances maintainability, scalability, and testability of the code by promoting separation of concerns and single responsibility.

### Mediatr
Mediatr was introduced to handle communication between the different layers of the application, specifically to manage command and query handlers in the CQRS pattern. Using Mediatr reduces direct dependencies between classes, promoting a more decoupled architecture.

### CQRS
CQRS was adopted to separate the operations that change state (commands) from those that read state (queries).

### Dependency Injection
Dependency Injection was used to manage dependencies between classes, promoting the Dependency Inversion principle of SOLID.

### Swagger
Swagger was added to automatically generate interactive API documentation, making it easier for developers to understand, test, and integrate with the API. 

### Carter
Carter was used to simplify the creation of lightweight and modular HTTP APIs in C#, enabling cleaner routing and better separation of concerns within the application.

### .NET 8 Upgrade
Upgrading to .NET 8 was done to leverage the latest performance improvements, security enhancements, and new features, ensuring the application remains modern, efficient, and well-supported

### EF Core In-Memory DB
EF Core In-Memory DB was used to facilitate both swagger and unit testing by providing a lightweight, in-memory database that simulates database operations without requiring a real database connection. This approach speeds up testing and avoids external dependencies.

### Ardalis.GuardClauses
Ardalis.GuardClauses was introduced to streamline input validation and guard against invalid data.

# Task 3 - Insurance Calculation for an Order

## Assumption
All products in the cart contribute to the total insurance cost.

## Decision
I created an endpoint that processes a list of products and sums their insurance costs.


# Task 4 - Digital Camera Insurance Surcharge

## Assumption
The additional charge is a flat €500 regardless of the number of digital cameras.

## Decision
Added a conditional check for digital cameras in the order and applied the surcharge.


# Task 5 - Surcharge Rates Upload

## Assumption
The endpoint needs to handle concurrent requests and maintain data integrity.

## Decision
- Implemented new endpoints to create and update surcharges within a database.
  - Made use of Mediatr and CQRS to send and receive data to a database.
- Implemented thread-safe mechanisms and validated input data.
