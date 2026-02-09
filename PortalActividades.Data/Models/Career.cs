using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Career
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int FacultyId { get; set; }

    [JsonIgnore]
    public virtual Faculty Faculty { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
