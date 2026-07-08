using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class ClinicalActionResultAssembler
{
    private static int ToStatusCodeFromClinicalError(ClinicalError error)
    {
        return error switch
        {
            ClinicalError.MedicalRecordNotFound => StatusCodes.Status404NotFound,
            ClinicalError.DiagnosisNotFound => StatusCodes.Status404NotFound,
            ClinicalError.TreatmentNotFound => StatusCodes.Status404NotFound,
            ClinicalError.PrescriptionNotFound => StatusCodes.Status404NotFound,
            ClinicalError.PrescriptionDetailNotFound => StatusCodes.Status404NotFound,
            ClinicalError.MedicalRecordAlreadyExistsForAppointment => StatusCodes.Status409Conflict,
            ClinicalError.PrescriptionDetailAlreadyExists => StatusCodes.Status409Conflict,
            ClinicalError.OperationCancelled => StatusCodes.Status409Conflict,
            ClinicalError.DatabaseError => StatusCodes.Status500InternalServerError,
            ClinicalError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var error = (ClinicalError)result.Error!;
        var statusCode = ToStatusCodeFromClinicalError(error);
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

        var error = (ClinicalError)result.Error!;
        var statusCode = ToStatusCodeFromClinicalError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        ClinicalError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
        where T : class
    {
        if (entity is not null)
            return successAction(entity);

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            ToStatusCodeFromClinicalError(notFoundError),
            notFoundError,
            errorLocalizer[notFoundError.ToString()]);
    }
}
