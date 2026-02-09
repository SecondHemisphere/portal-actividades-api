using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Enrollment
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public int StudentId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    [JsonIgnore]
    public virtual Activity Activity { get; set; } = null!;

    [JsonIgnore]
    public virtual User Student { get; set; } = null!;
}
