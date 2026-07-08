using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Tenant.Application.CommandServices;
using VitaliaBackend.Tenant.Domain.Model;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Domain.Repositories;

namespace VitaliaBackend.Tenant.Application.Internal.CommandServices;

public class HealthcareCenterCommandService(
    IHealthcareCenterRepository healthcareCenterRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IHealthcareCenterCommandService
{
    public async Task<Result<HealthcareCenter>> Handle(CreateHealthcareCenterCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.HealthcareCenterName))
            return Result<HealthcareCenter>.Failure(
                TenantError.HealthcareCenterCreationError,
                localizer[nameof(TenantError.HealthcareCenterCreationError)]);

        var existsDuplicate = await healthcareCenterRepository.ExistsByPublicIdAsync(command.Code, cancellationToken: cancellationToken);

        if (existsDuplicate)
            return Result<HealthcareCenter>.Failure(
                TenantError.HealthcareCenterCreationError,
                localizer[nameof(TenantError.HealthcareCenterCreationError)]);

        var healthcareCenter = new HealthcareCenter(
            command.Code,
            command.HealthcareCenterName,
            command.AllianceStartDate,
            command.AllianceFinishDate,
            command.RucNumber,
            command.ImageUrl);

        try
        {
            await healthcareCenterRepository.AddAsync(healthcareCenter, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<HealthcareCenter>.Success(healthcareCenter);
        }
        catch (OperationCanceledException)
        {
            return Result<HealthcareCenter>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<HealthcareCenter>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<HealthcareCenter>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result<HealthcareCenter>> Handle(UpdateHealthcareCenterCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.HealthcareCenterName))
            return Result<HealthcareCenter>.Failure(
                TenantError.HealthcareCenterUpdateError,
                localizer[nameof(TenantError.HealthcareCenterUpdateError)]);

        var healthcareCenter = await healthcareCenterRepository.FindByPublicIdAsync(command.HealthcareCenterId, cancellationToken);

        if (healthcareCenter is null)
            return Result<HealthcareCenter>.Failure(
                TenantError.HealthcareCenterNotFoundError,
                localizer[nameof(TenantError.HealthcareCenterNotFoundError)]);

        healthcareCenter.UpdateDetails(
            command.HealthcareCenterName,
            command.AllianceStartDate,
            command.AllianceFinishDate,
            command.RucNumber,
            command.ImageUrl);

        try
        {
            healthcareCenterRepository.Update(healthcareCenter);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<HealthcareCenter>.Success(healthcareCenter);
        }
        catch (OperationCanceledException)
        {
            return Result<HealthcareCenter>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<HealthcareCenter>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<HealthcareCenter>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteHealthcareCenterCommand command, CancellationToken cancellationToken)
    {
        var healthcareCenter = await healthcareCenterRepository.FindByPublicIdAsync(command.HealthcareCenterId, cancellationToken);

        if (healthcareCenter is null)
            return Result.Failure(
                TenantError.HealthcareCenterNotFoundError,
                localizer[nameof(TenantError.HealthcareCenterNotFoundError)]);

        try
        {
            healthcareCenterRepository.Remove(healthcareCenter);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    private static bool IsValid(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }
}
