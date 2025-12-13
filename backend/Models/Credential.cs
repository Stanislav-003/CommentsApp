using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("credentials")]
public class Credential : BaseModel
{
    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("role")]
    public CredentialRole Role { get; set; }

    [Column("blocked")]
    public bool IsBlocked { get; set; } = false;

    [Column("user_id")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}

public enum CredentialRole
{
    USER = 1,
    ADMIN = 2,
}

public static class UsersRoles
{
    public const string USER = "USER";
    public const string ADMIN = "ADMIN";
}