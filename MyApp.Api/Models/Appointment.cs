using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Api.Models;

[Table("appointments")]
public class Appointment
{
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    [Required]
    public long UserId { get; set; }

    [Column("appointment_time")]
    [Required]
    public DateTimeOffset AppointmentTime { get; set; }

    [Column("appointee_name")]
    [Required]
    public string AppointeeName { get; set; } = string.Empty;

    [Column("location")]
    [Required]
    public string Location { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public User? User { get; set; }
}
