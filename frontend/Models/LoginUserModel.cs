using System.ComponentModel.DataAnnotations;

namespace frontend.Models;

public class LoginUserModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
