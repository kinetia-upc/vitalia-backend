using VitaliaBackend.Pharmacy.Domain.Model;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;

public static class PharmacyActionResultAssembler
{
    private static int ToStatusCodeFromPharmacyError(PharmacyError error)
    {
        return error switch
        {
            PharmacyError.MedicineNotFoundError => StatusCodes.Status404NotFound,
            PharmacyError.OperationCancelled => StatusCodes.Status409Conflict,
            PharmacyError.DatabaseError => StatusCodes.Status500InternalServerError,
            PharmacyError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = (PharmacyError)result.Error!;
        var statusCode = ToStatusCodeFromPharmacyError(error);
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

        var error = (PharmacyError)result.Error!;
        var statusCode = ToStatusCodeFromPharmacyError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        PharmacyError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
        where T : class
    {
        if (entity is not null)
            return successAction(entity);

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            ToStatusCodeFromPharmacyError(notFoundError),
            notFoundError,
            errorLocalizer[notFoundError.ToString()]);
    }
}
