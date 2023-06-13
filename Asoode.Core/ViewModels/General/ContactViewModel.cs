using System;
using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.General;

public class ContactViewModel
{
    [Required] [MaxLength(100)] public string FirstName { get; set; }
    [Required] [MaxLength(100)] public string LastName { get; set; }
    [Required] [MaxLength(200)] public string Email { get; set; }
    [Required] [MaxLength(1000)] public string Message { get; set; }
}

public class ContactListViewModel : ContactViewModel
{
    public bool Seen { get; set; }
    public int Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
}