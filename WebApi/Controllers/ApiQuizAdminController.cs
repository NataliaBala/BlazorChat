using ApplicationCore.Interfaces.AdminService;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/v1/admin/quizzes")]
public class ApiQuizAdminController : ControllerBase
{
    private readonly IQuizAdminService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<QuizItem> _validator;

    public ApiQuizAdminController(IQuizAdminService service, IMapper mapper, IValidator<QuizItem> validator)
    {
        _service = service;
        _mapper = mapper;
        _validator = validator;
    }

    [HttpPost]
    public ActionResult<object> AddQuiz(LinkGenerator link, NewQuizDto dto)
    {
        var quiz = _service.AddQuiz(_mapper.Map<Quiz>(dto));
        return Created(
            link.GetPathByAction(
                HttpContext, 
                nameof(GetQuiz),         // nazwa metody kontrolera zwracająca quiz
                null,                    // kontroler, null oznacza bieżący
                new { quizId = quiz.Id }),// parametry ścieżki, id utworzonego quiz
            quiz
        );
    }

    [HttpGet]
    [Route("{quizId}")]
    public ActionResult<Quiz> GetQuiz(int quizId)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        return quiz is null ? NotFound() : quiz;
    }

    [HttpGet]
    [Route("{quizId}/items/{itemId}")]
    public ActionResult<QuizItem> GetQuizItem(int quizId, int itemId)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null)
        {
            return NotFound(new { error = $"Quiz with id {quizId} not found" });
        }
        
        var item = quiz.Items.FirstOrDefault(i => i.Id == itemId);
        return item is null ? NotFound(new { error = $"Item with id {itemId} not found in quiz {quizId}" }) : item;
    }

    [HttpPut]
    [Route("{quizId}")]
    public ActionResult<Quiz> UpdateQuiz(int quizId, Quiz quiz)
    {
        if (quizId != quiz.Id)
        {
            return BadRequest(new { error = "Quiz ID in URL does not match quiz ID in body" });
        }
        
        var existingQuiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (existingQuiz is null)
        {
            return NotFound(new { error = $"Quiz with id {quizId} not found" });
        }
        
        _service.UpdateQuiz(quiz);
        return Ok(quiz);
    }

    [HttpDelete]
    [Route("{quizId}")]
    public ActionResult DeleteQuiz(int quizId)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null)
        {
            return NotFound(new { error = $"Quiz with id {quizId} not found" });
        }
        
    
        return NoContent();
    }

    [HttpPatch]
    [Route("{quizId}")]
    [Consumes("application/json-patch+json")]
    public ActionResult<Quiz> AddQuizItem(int quizId, JsonPatchDocument<Quiz>? patchDoc)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null || patchDoc is null)
        {
            return NotFound(new
            {
                error = $"Quiz width id {quizId} not found"
            });
        }
        int previousCount = quiz.Items.Count;
        patchDoc.ApplyTo(quiz, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (previousCount < quiz.Items.Count)
        {
            QuizItem item = quiz.Items[^1];
            quiz.Items.RemoveAt(quiz.Items.Count - 1);
            
            // Validate the quiz item before adding
            var validationResult = _validator.Validate(item);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            _service.AddQuizItemToQuiz(quizId, item);
        }
        return Ok(_service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId));
    }
}
