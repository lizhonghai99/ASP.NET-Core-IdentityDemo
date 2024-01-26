using AuthCenter.Entities;
using System;
using System.Collections.Generic;

namespace ConsoleApp.Entities;

public partial class AspNetPermission
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ICollection<AspNetRolePermission> AspNetRolePermissions { get; set; }

}
