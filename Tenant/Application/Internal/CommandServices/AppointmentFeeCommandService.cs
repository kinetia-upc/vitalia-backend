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

public class AppointmentFeeCommandService(
    IAppointmentFeeRepository appointmentFeeRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IAppointmentFeeCommandService
{
    public async Task<Result<AppointmentFee>> Handle(CreateAppointmentFeeCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.BranchId, command.Price))
            return Result<AppointmentFee>.Failure(
                TenantError.AppointmentFeeCreationError,
                localizer[nameof(TenantError.AppointmentFeeCreationError)]);

        var existingFee = await appointmentFeeRepository.FindByBranchAndSpecialityAsync(
            command.BranchId,
            command.SpecialityId,
            cancellationToken);

        if (existingFee is not null)
        {
            existingFee.UpdateDetails(command.BranchId, command.SpecialityId, command.Price);

            try
            {
                appointmentFeeRepository.Update(existingFee);
                await unitOfWork.CompleteAsync(cancellationToken);
                return Result<AppointmentFee>.Success(existingFee);
            }
            catch (OperationCanceledException)
            {
                return Result<AppointmentFee>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
            }
            catch (DbUpdateException)
            {
                return Result<AppointmentFee>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
            }
            catch (Exception)
            {
                return Result<AppointmentFee>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
            }
        }

        var code = string.IsNullOrWhiteSpace(command.Code)
            ? GenerateCode()
            : command.Code.Trim();

        var existsDuplicate = await appointmentFeeRepository.ExistsByPublicIdAsync(code, cancellationToken: cancellationToken);

        if (existsDuplicate)
            return Result<AppointmentFee>.Failure(
                TenantError.AppointmentFeeCreationError,
                localizer[nameof(TenantError.AppointmentFeeCreationError)]);

        var appointmentFee = new AppointmentFee(
            code,
            command.BranchId,
            command.SpecialityId,
            command.Price);

        try
        {
            await appointmentFeeRepository.AddAsync(appointmentFee, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<AppointmentFee>.Success(appointmentFee);
        }
        catch (OperationCanceledException)
        {
            return Result<AppointmentFee>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<AppointmentFee>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<AppointmentFee>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result<AppointmentFee>> Handle(UpdateAppointmentFeeCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.BranchId, command.Price))
            return Result<AppointmentFee>.Failure(
                TenantError.AppointmentFeeUpdateError,
                localizer[nameof(TenantError.AppointmentFeeUpdateError)]);

        var appointmentFee = await appointmentFeeRepository.FindByIdAsync(command.AppointmentFeeId, cancellationToken);

        if (appointmentFee is null)
            return Result<AppointmentFee>.Failure(
                TenantError.AppointmentFeeNotFoundError,
                localizer[nameof(TenantError.AppointmentFeeNotFoundError)]);

        appointmentFee.UpdateDetails(command.BranchId, command.SpecialityId, command.Price);

        try
        {
            appointmentFeeRepository.Update(appointmentFee);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<AppointmentFee>.Success(appointmentFee);
        }
        catch (OperationCanceledException)
        {
            return Result<AppointmentFee>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<AppointmentFee>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<AppointmentFee>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteAppointmentFeeCommand command, CancellationToken cancellationToken)
    {
        var appointmentFee = await appointmentFeeRepository.FindByIdAsync(command.AppointmentFeeId, cancellationToken);

        if (appointmentFee is null)
            return Result.Failure(
                TenantError.AppointmentFeeNotFoundError,
                localizer[nameof(TenantError.AppointmentFeeNotFoundError)]);

        try
        {
            appointmentFeeRepository.Remove(appointmentFee);
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

    private static bool IsValid(string branchId, decimal price)
    {
        return !string.IsNullOrWhiteSpace(branchId) && price >= 0;
    }

    private static string GenerateCode()
    {
        return $"fee-{Guid.NewGuid():N}"[..16];
    }
}
