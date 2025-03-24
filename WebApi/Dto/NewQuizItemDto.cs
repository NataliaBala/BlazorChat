using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto;

public class NewQuizItemDto
{
    [Required]
    public string Question { get; set; }
    
    [Required]
    public List<string> Options { get; set; }
    
    [Required]
    public int CorrectOptionIndex { get; set; }
}
