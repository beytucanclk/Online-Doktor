using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Entities;

public partial class WorkingTime
{
    public int Id { get; set; }

    public string? Monday { get; set; }

    public string? Tuesday { get; set; }

    public string? Wednesday { get; set; }

    public string? Thursday { get; set; }

    public string? Friday { get; set; }

    public string? Saturday { get; set; }

    public string? Sunday { get; set; }

    public int? DoctorId { get; set; }

    public virtual Doctor? Doctor { get; set; }
}
