using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Entities;

public partial class Doctor
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string? AcademicTitle { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Fullname { get; set; }

    public string? Speciality { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<WorkingTime> WorkingTimes { get; set; } = new List<WorkingTime>();
}
