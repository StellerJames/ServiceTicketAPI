using ServiceTicketAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Register Swagger/OpenAPI services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Logger;

// The connection string is stored in User Secrets so the password never gets committed.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found. Set it with: " +
        "dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<your connection string>\"");

//Create an instance of the TicketRepository class and use it to retrieve tickets from the database.
var repository = new TicketRepository(connectionString);

// Enable Swagger only while developing locally (Not intended for production).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var allowedStatuses = new[] { "New", "In Progress", "Closed" }; //A6-3 Restrict Invalid Status Values

app.MapPost("/tickets", (Ticket ticket) =>
{
    if (string.IsNullOrWhiteSpace(ticket.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    if (string.IsNullOrWhiteSpace(ticket.Description))
    {
        return Results.BadRequest("Description is required.");
    }

    // Case-insensitive match, normalized to the first letter capitalized casing
    var matchedStatus = allowedStatuses.FirstOrDefault(s =>
        string.Equals(s, ticket.Status, StringComparison.OrdinalIgnoreCase));

    if (matchedStatus == null)
    {
        logger.LogWarning("Failed to create ticket. Title: {Title}: invalid status.", ticket.Title); // Adds a warning message when validation fails
        return Results.BadRequest("Status must be New, In Progress, or Closed.");
    }

    ticket.Status = matchedStatus;

    repository.AddTicket(ticket);

    logger.LogInformation("Ticket created successfully. Title: {Title}", ticket.Title); // Adds a log message after a ticket is successfully created

    return Results.Created($"/tickets/{ticket.TicketId}", ticket);
});

app.MapPut("/tickets/{id:int}", (int id, Ticket updatedTicket) =>
{
    var existingTicket = repository.GetTicketById(id);

    if (existingTicket == null)
    {
        return Results.NotFound("Ticket Not Found.");
    }

    if (string.IsNullOrWhiteSpace(updatedTicket.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    if (string.IsNullOrWhiteSpace(updatedTicket.Description))
    {
        return Results.BadRequest("Description is required.");
    }

    var matchedStatus = allowedStatuses.FirstOrDefault(s =>
    string.Equals(s, updatedTicket.Status, StringComparison.OrdinalIgnoreCase));

    if (matchedStatus == null)
    {
        return Results.BadRequest("Status must be New, In Progress, or Closed.");
    }

    updatedTicket.Status = matchedStatus; // "in progress" → "In Progress", etc.

    updatedTicket.TicketId = id; // Ensure the ID is set correctly

    repository.UpdateTicket(updatedTicket);

    logger.LogInformation("Ticket {TicketId} updated successfully. New status: {Status}", updatedTicket.TicketId, updatedTicket.Status); // Adds a log message after the update succeeds

    return Results.Ok(updatedTicket);
});

app.MapDelete("/tickets/{id:int}", (int id) =>
{
    var ticket = repository.GetTicketById(id);

    if (ticket == null)
    {
        return Results.NotFound("Ticket not found."); //A6-4 Return a clear message when deleting a ticket that does not exist.
    }

    repository.DeleteTicket(id);

    return Results.Ok($"Ticket {id} was deleted.");
});

app.MapGet("/tickets", () =>
{
    return repository.GetAllTickets();
});

app.MapGet("/tickets/{id:int}", (int id) =>
{
    var ticket = repository.GetTicketById(id);

    if (ticket == null)
    {
        return Results.NotFound("Ticket not found."); //A6-4 Return a clear message when the requested ticket id does not exist.
    }

    return Results.Ok(ticket);
});

app.Run();
