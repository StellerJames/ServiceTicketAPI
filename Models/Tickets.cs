namespace ServiceTicketAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Ticket
{
    public int TicketId { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

}
