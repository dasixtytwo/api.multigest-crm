using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Contacts;

public class ContactDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string ContactOwner { get; set; } = string.Empty;

    [Required]
    public DateTime BecomeCustomerFrom { get; set; } = DateTime.Now;
}
