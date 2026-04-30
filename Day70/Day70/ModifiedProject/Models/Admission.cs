using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Admission
{
    public int AdmissionId { get; set; }

    public DateTime AdmissionDate { get; set; }

    public string Status { get; set; } = null!;

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Student Student { get; set; } = null!;
}
