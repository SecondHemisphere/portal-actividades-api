using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string Role { get; set; } = null!;

    public bool? Active { get; set; }

    public string? PhotoUrl { get; set; }

    [JsonIgnore]
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    [JsonIgnore]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [JsonIgnore]
    public virtual Organizer? Organizer { get; set; }

    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [JsonIgnore]
    public virtual Student? Student { get; set; }
}
