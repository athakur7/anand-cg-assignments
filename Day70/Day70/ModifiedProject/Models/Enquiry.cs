using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Enquiry
{
    public int EnquiryId { get; set; }

    public DateTime EnquiryDate { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string EnquiryType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ReplyMessage { get; set; }

    public DateTime? RepliedAt { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
