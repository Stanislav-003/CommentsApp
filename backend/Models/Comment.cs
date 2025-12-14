using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("comment")]
public class Comment : BaseModel
{
    [Column("text")] public string Text { get; set; } = string.Empty;

    [Column("parent_id")] public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }

    [Column("user_id")] public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Attachment? Attachment { get; set; }
    public ICollection<Comment> Children { get; set; } = new List<Comment>();
}