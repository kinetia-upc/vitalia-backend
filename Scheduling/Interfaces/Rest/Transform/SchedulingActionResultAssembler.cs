using VitaliaBackend.Scheduling.Domain.Model;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class SchedulingActionResultAssembler
{
    private static int ToStatusCodeFromSchedulingError(SchedulingError error)
    {
        return error switch
        {
            SchedulingError.AppointmentNotFoundError => StatusCodes.Status404NotFound,
            SchedulingError.AvailabilitySlotNotFoundError => StatusCodes.Status404NotFound,
            SchedulingError.OperationCancelled => StatusCodes.Status409Conflict,
            SchedulingError.DatabaseError => StatusCodes.Status500InternalServerError,
            SchedulingError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult<T>(
        ControllerBase controller,
        Result<T> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess)
            return successAction(result.Value!);

        var error = (SchedulingError)result.Error!;
        var statusCode = ToStatusCodeFromSchedulingError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromResult(
        ControllerBase controller,
        Result result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<IActionResult> successAction)
    {
        if (result.IsSuccess)
            return successAction();

        var error = (SchedulingError)result.Error!;
        var statusCode = ToStatusCodeFromSchedulingError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        SchedulingError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
        where T : class
    {
        if (entity is not null)
            return successAction(entity);

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            ToStatusCodeFromSchedulingError(notFoundError),
            notFoundError,
            errorLocalizer[notFoundError.ToString()]);
    }
}
