using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }
}
