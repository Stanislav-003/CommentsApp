using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("attachment")]
public class Attachment : BaseModel
{
    [Column("comment_id")] public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    [Column("file_url")] public string FileUrl { get; set; } = null!;

    [Column("file_name")] public string FileName { get; set; } = null!;

    [Column("file_type")] public AttachmentType FileType { get; set; }
}

public enum AttachmentType
{
    Image = 1,
    TextFile = 2
}