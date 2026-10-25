using Dapper;
using MySqlConnector;
using ServiceTicketAPI.Models;

// Stores the connection string and provides methods to retrieve, add, update, and delete tickets in the database.
public class TicketRepository 
{
    private readonly string _connectionString;
    public TicketRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    //Opens a connection to the database, executes a SQL query to retrieve all tickets,
    //and returns the result as an IEnumerable of Ticket objects.
    public IEnumerable<Ticket> GetAllTickets() 
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT * FROM Tickets";
        return connection.Query<Ticket>(sql);
    }

    //This method retrieves one ticket from the database based on the provided ticket ID.
    //It opens a connection to the database, executes a SQL query to retrieve the ticket with the specified ID,
    //and returns the result as a Ticket object.
    public Ticket? GetTicketById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);

        string sql = "SELECT * FROM Tickets WHERE TicketId = @Id";

        return connection.QueryFirstOrDefault<Ticket>(sql, new { Id = id });
    }

    //This method inserts a new row into the Tickets table and stores the new id back into the Ticket object.
    public void AddTicket(Ticket ticket)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string sql = @"
        INSERT INTO Tickets (Title, Description, Status)
        VALUES (@Title, @Description, @Status);
        SELECT LAST_INSERT_ID();";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Title", ticket.Title);
        cmd.Parameters.AddWithValue("@Description", ticket.Description);
        cmd.Parameters.AddWithValue("@Status", ticket.Status);

        ticket.TicketId = Convert.ToInt32(cmd.ExecuteScalar());
    }

    //This method updates the matching row in the Tickets table.
    public void UpdateTicket(Ticket ticket)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string sql = @"
        UPDATE Tickets
        SET Title = @Title, 
        Description = @Description, 
        Status = @Status
        WHERE TicketId = @TicketId";

        using var cmd = new MySqlCommand(sql, connection);

        cmd.Parameters.AddWithValue("@Title", ticket.Title);
        cmd.Parameters.AddWithValue("@Description", ticket.Description);
        cmd.Parameters.AddWithValue("@Status", ticket.Status);
        cmd.Parameters.AddWithValue("@TicketId", ticket.TicketId);
        
        cmd.ExecuteNonQuery();
    }

    //This method removes the matching row from the Tickets table based on the provided ticket ID.
    public void DeleteTicket(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string sql = "DELETE FROM Tickets WHERE TicketId = @Id";

        using var cmd = new MySqlCommand(sql, connection);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }  

}

