using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Student
{
    public int UserId { get; set; }

    public int? CareerId { get; set; }

    public int? Semester { get; set; }

    public string? Modality { get; set; }

    public string? Schedule { get; set; }

    [JsonIgnore]
    public virtual Career? Career { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
