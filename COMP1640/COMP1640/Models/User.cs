using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? RoleId { get; set; }

    public int? FacultyId { get; set; }

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Faculty? Faculty { get; set; }

    public virtual Role? Role { get; set; }
}
