using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Dto;

public class QuizItemUserAnswerDto
{
    [NotNull]
    [Length(minimumLength: 1, maximumLength: 200)]
    public string? Answer { get; set; }
}