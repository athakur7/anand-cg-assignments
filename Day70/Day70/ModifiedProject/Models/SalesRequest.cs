using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class SalesRequest
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Status { get; set; } = null!;
}
