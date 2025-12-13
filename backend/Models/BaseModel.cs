using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class BaseModel
{
    [Key]
    [Column("id", TypeName = "uuid")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("created_at")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
