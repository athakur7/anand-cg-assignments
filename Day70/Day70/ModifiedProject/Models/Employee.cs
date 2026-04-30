using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Age { get; set; }

    public string? ImagePath { get; set; }
}
