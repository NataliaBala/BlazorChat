using ApplicationCore.Models;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using WebApi.Dto;

namespace WebApi.Mapper;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<QuizItem, QuizItemDto>()
            .ForMember(
                q => q.Options,
                op => op.MapFrom(i => new List<string>(i.IncorrectAnswers) { i.CorrectAnswer }));
        CreateMap<Quiz, QuizDto>()
            .ForMember(
                q => q.Items,
                op => op.MapFrom<List<QuizItem>>(i => i.Items)
            );
        CreateMap<NewQuizDto, Quiz>();

        // Mapping for the FeedbackDto
        CreateMap<QuizItemUserAnswer, AnswerFeedbackDto>()
            .ForMember(
                dest => dest.Question,
                opt => opt.MapFrom(src => src.QuizItem.Question))
            .ForMember(
                dest => dest.Answer,
                opt => opt.MapFrom(src => src.Answer))
            .ForMember(
                dest => dest.IsCorrect,
                opt => opt.MapFrom(src => src.IsCorrect()));
                
        // Mapping from NewQuizItemDto to QuizItem
        CreateMap<NewQuizItemDto, QuizItem>()
            .ForMember(
                dest => dest.Question,
                opt => opt.MapFrom(src => src.Question))
            .ForMember(
                dest => dest.CorrectAnswer,
                opt => opt.MapFrom(src => src.Options[src.CorrectOptionIndex]))
            .ForMember(
                dest => dest.IncorrectAnswers,
                opt => opt.MapFrom(src => src.Options
                    .Where((_, idx) => idx != src.CorrectOptionIndex)
                    .ToList()));
    }
}
