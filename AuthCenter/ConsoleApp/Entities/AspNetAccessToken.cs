using System;
using System.Collections.Generic;

namespace ConsoleApp.Entities;

public partial class AspNetAccessToken
{
    public string Id { get; set; } = null!;

    public string ApplicationName { get; set; } = null!;

    public string TokenName { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string Secret { get; set; } = null!;

    public bool InActive { get; set; }

    public string RoleId { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual AspNetRole Role { get; set; } = null!;
}
