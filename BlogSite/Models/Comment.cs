using System.ComponentModel.DataAnnotations;

namespace BlogSite.Models;

public class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Comment text is required")]
    public string Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    // Foreign key to BlogPost
    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; }

}
