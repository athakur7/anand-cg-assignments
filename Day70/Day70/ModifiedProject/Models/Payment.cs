using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public DateTime PaymentDate { get; set; }

    public int Amount { get; set; }

    public string PaymentMode { get; set; } = null!;

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int AdmissionId { get; set; }

    public virtual Admission Admission { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
