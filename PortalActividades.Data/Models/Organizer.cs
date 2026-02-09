using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Organizer
{
    public int UserId { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public string? Bio { get; set; }

    public string? Shifts { get; set; }

    public string? WorkDays { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
