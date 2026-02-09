using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Faculty
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();
}
