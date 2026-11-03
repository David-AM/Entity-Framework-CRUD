using EF_CRUD.Common;
using Microsoft.AspNetCore.Mvc;

namespace EF_CRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>Turns a ServiceResult; into an HTTP response: 200/201 with the data, or a ProblemDetails error.</summary>
    protected IActionResult Respond<T>(
        ServiceResult<T> result,
        string? createdAtAction = null,
        Func<T, object>? routeValues = null)
    {
        if (!result.IsSuccess) return Failure(result);

        if (result.Status == ResultStatus.Created)
        {
            return createdAtAction is not null && routeValues is not null
                ? CreatedAtAction(createdAtAction, routeValues(result.Data!), result.Data)
                : StatusCode(StatusCodes.Status201Created, result.Data);
        }

        return Ok(result.Data);
    }

    /// <summary>For operations without data (delete): 204 on success, ProblemDetails on failure.</summary>
    protected IActionResult RespondEmpty(ServiceResult result) =>
        result.IsSuccess ? NoContent() : Failure(result);

    private IActionResult Failure(ServiceResult result) => result.Status switch
    {
        ResultStatus.ValidationFailed => ValidationProblem(
            new ValidationProblemDetails(result.Errors.ToDictionary(e => e.Key, e => e.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = result.Message
            }),
        ResultStatus.NotFound => Problem(detail: result.Message, statusCode: StatusCodes.Status404NotFound, title: "Not found"),
        ResultStatus.Conflict => Problem(detail: result.Message, statusCode: StatusCodes.Status409Conflict, title: "Conflict"),
        ResultStatus.Unauthorized => Problem(detail: result.Message, statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized"),
        ResultStatus.Forbidden => Problem(detail: result.Message, statusCode: StatusCodes.Status403Forbidden, title: "Forbidden"),
        _ => Problem(detail: result.Message, statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error")
    };
}
