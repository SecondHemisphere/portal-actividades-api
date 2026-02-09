using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PortalActividades.Data.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public int StudentId { get; set; }

    public int Stars { get; set; }

    public string Comment { get; set; } = null!;

    public DateOnly RatingDate { get; set; }

    [JsonIgnore]
    public virtual Activity Activity { get; set; } = null!;

    [JsonIgnore]
    public virtual User Student { get; set; } = null!;
}
