using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Shared.Dtos.Authentication
{
    public record UserRegistrationDto(
            [Required]
            [MaxLength(60)]
            string FirstName,

            [Required]
            [MaxLength(60)]
            string LastName,

            [Required]
            [EmailAddress]
            string Email,

            [Required]
            string Password
    );
}
