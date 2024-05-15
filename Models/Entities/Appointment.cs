using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Entities;

public partial class Appointment
{
    public int Id { get; set; }

    public int? DoctorId { get; set; }

    public string? PatientEmail { get; set; }

    public string? PatientFullname { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? RoomName { get; set; }

    public string? Link { get; set; }

    public virtual Doctor? Doctor { get; set; }
}
