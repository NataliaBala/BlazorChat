using FluentValidation;
using WebApi.Dto;

namespace WebApi.Validators;

public class NewQuizItemDtoValidator : AbstractValidator<NewQuizItemDto>
{
    public NewQuizItemDtoValidator()
    {
        RuleFor(q => q.Question)
            .MaximumLength(200).WithMessage("Pytanie nie może być dłuższe niż 200 znaków.")
            .MinimumLength(3).WithMessage("Pytanie nie może być krótsze od 3 znaków!");
            
        RuleFor(q => q.Options)
            .NotEmpty().WithMessage("Lista opcji nie może być pusta!");
            
        RuleForEach(q => q.Options)
            .NotEmpty().WithMessage("Opcja nie może być pusta!")
            .MaximumLength(200).WithMessage("Opcja nie może być dłuższa niż 200 znaków.")
            .MinimumLength(1).WithMessage("Opcja nie może być krótsza niż 1 znak!");
            
        RuleFor(q => q.CorrectOptionIndex)
            .Must((dto, index) => index >= 0 && index < dto.Options.Count)
            .WithMessage("Indeks poprawnej odpowiedzi musi znajdować się w zakresie listy opcji!");
    }
}
