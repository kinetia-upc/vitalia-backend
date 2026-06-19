using VitaliaBackend.Billing.Domain.Model;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Billing.Interfaces.Rest.Transform;

public static class BillingActionResultAssembler
{
    private static int ToStatusCodeFromBillingError(BillingError error)
    {
        return error switch
        {
            BillingError.BillingClaimNotFoundError => StatusCodes.Status404NotFound,
            BillingError.OperationCancelled => StatusCodes.Status409Conflict,
            BillingError.DatabaseError => StatusCodes.Status500InternalServerError,
            BillingError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = (BillingError)result.Error!;
        var statusCode = ToStatusCodeFromBillingError(error);
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

        var error = (BillingError)result.Error!;
        var statusCode = ToStatusCodeFromBillingError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        BillingError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
        where T : class
    {
        if (entity is not null)
            return successAction(entity);

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            ToStatusCodeFromBillingError(notFoundError),
            notFoundError,
            errorLocalizer[notFoundError.ToString()]);
    }
}
