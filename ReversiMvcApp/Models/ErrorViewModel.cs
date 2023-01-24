using System.ComponentModel.DataAnnotations;

namespace ReversiMvcApp.Models;

public class ErrorViewModel
{
    [Key]
    public Guid id { get; set; }
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}