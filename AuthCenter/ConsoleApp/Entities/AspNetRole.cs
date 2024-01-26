using AuthCenter.Entities;
using System;
using System.Collections.Generic;

namespace ConsoleApp.Entities;

public partial class AspNetRole
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetAccessToken> AspNetAccessTokens { get; set; } = new List<AspNetAccessToken>();

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();

    public ICollection<AspNetRolePermission> AspNetRolePermissions { get; set; }
}
