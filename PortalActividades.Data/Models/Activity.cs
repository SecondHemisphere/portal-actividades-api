using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int CategoryId { get; set; }

    public int OrganizerId { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly RegistrationDeadline { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Description { get; set; }

    public string? PhotoUrl { get; set; }

    public bool? Active { get; set; }

    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [JsonIgnore]
    public virtual User Organizer { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
