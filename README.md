# ServiceTicketAPI

A REST API for creating and tracking IT service tickets, built for my Server-side course at Ogden-Weber Technical College.

## Tech used

- C# / ASP.NET Core 8 (minimal APIs)
- MySQL with Dapper and MySqlConnector
- Swagger for testing endpoints

## Endpoints

Methods and routes for the API and what they do:

GET /tickets            List all tickets 
GET /tickets/{id}       Get one ticket  
POST /tickets           Create a ticket 
PUT /tickets/{id}       Update a ticket 
DELETE /tickets/{id}    Delete a ticket

Status must be `New`, `In Progress`, or `Closed`.

## Running it locally

1. Create the database:

   ```sql
   CREATE DATABASE ServiceTickets;
   USE ServiceTickets;
   CREATE TABLE Tickets (
       TicketId    INT AUTO_INCREMENT PRIMARY KEY,
       Title       VARCHAR(200) NOT NULL,
       Description TEXT NOT NULL,
       Status      VARCHAR(20) NOT NULL
   );
   ```
2. Store your connection string in User Secrets (from the project folder):
   ```
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ServiceTickets;User=<user>;Password=<password>"
   ```
3. Run `dotnet run` and open `/swagger` in the browser.

## What I learned

- Moving from an in-memory list in C# to a real database
- Input Validation 
- Logging
- Keeping secrets out of source control