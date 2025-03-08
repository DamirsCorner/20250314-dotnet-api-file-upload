using System.ComponentModel.DataAnnotations;

namespace WebApiFileUpload.Models;

public class UploadRequest
{
    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }
    public List<IFormFile> Files { get; set; } = new();
}
