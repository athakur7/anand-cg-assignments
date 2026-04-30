using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class PaymentErrror
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public string StackTrace { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
