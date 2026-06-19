using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using VitaliaBackend.Tenant.Domain.Model;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

/// <summary>
///     Converts a Result/nullable coming from the Tenant application layer into the
///     right HTTP response, the same pattern already used by BillingActionResultAssembler.
/// </summary>
public static class TenantActionResultAssembler
{
    private static int ToStatusCodeFromTenantError(TenantError error)
    {
        return error switch
        {
            TenantError.HealthcareCenterNotFoundError => StatusCodes.Status404NotFound,
            TenantError.BranchNotFoundError => StatusCodes.Status404NotFound,
            TenantError.AppointmentFeeNotFoundError => StatusCodes.Status404NotFound,
            TenantError.OperationCancelled => StatusCodes.Status409Conflict,
            TenantError.DatabaseError => StatusCodes.Status500InternalServerError,
            TenantError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = (TenantError)result.Error!;
        var statusCode = ToStatusCodeFromTenantError(error);
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

        var error = (TenantError)result.Error!;
        var statusCode = ToStatusCodeFromTenantError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        TenantError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
        where T : class
    {
        if (entity is not null)
            return successAction(entity);

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            ToStatusCodeFromTenantError(notFoundError),
            notFoundError,
            errorLocalizer[notFoundError.ToString()]);
    }
}
