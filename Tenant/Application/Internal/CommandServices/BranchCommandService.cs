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

public class BranchCommandService(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IBranchCommandService
{
    public async Task<Result<Branch>> Handle(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.HealthcareCenterId, command.BranchName, command.Address))
            return Result<Branch>.Failure(
                TenantError.BranchCreationError,
                localizer[nameof(TenantError.BranchCreationError)]);

        var existsDuplicate = await branchRepository.ExistsByPublicIdAsync(command.Id, cancellationToken: cancellationToken);

        if (existsDuplicate)
            return Result<Branch>.Failure(
                TenantError.BranchCreationError,
                localizer[nameof(TenantError.BranchCreationError)]);

        var branch = new Branch(
            command.Id,
            command.HealthcareCenterId,
            command.AddressId,
            command.BranchName,
            command.Address);

        try
        {
            await branchRepository.AddAsync(branch, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Branch>.Success(branch);
        }
        catch (OperationCanceledException)
        {
            return Result<Branch>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Branch>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Branch>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result<Branch>> Handle(UpdateBranchCommand command, CancellationToken cancellationToken)
    {
        if (!IsValid(command.HealthcareCenterId, command.BranchName, command.Address))
            return Result<Branch>.Failure(
                TenantError.BranchUpdateError,
                localizer[nameof(TenantError.BranchUpdateError)]);

        var branch = await branchRepository.FindByPublicIdAsync(command.BranchId, cancellationToken);

        if (branch is null)
            return Result<Branch>.Failure(
                TenantError.BranchNotFoundError,
                localizer[nameof(TenantError.BranchNotFoundError)]);

        branch.UpdateDetails(
            command.HealthcareCenterId,
            command.AddressId,
            command.BranchName,
            command.Address);

        try
        {
            branchRepository.Update(branch);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Branch>.Success(branch);
        }
        catch (OperationCanceledException)
        {
            return Result<Branch>.Failure(TenantError.OperationCancelled, localizer[nameof(TenantError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Branch>.Failure(TenantError.DatabaseError, localizer[nameof(TenantError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Branch>.Failure(TenantError.InternalServerError, localizer[nameof(TenantError.InternalServerError)]);
        }
    }

    public async Task<Result> Handle(DeleteBranchCommand command, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FindByPublicIdAsync(command.BranchId, cancellationToken);

        if (branch is null)
            return Result.Failure(
                TenantError.BranchNotFoundError,
                localizer[nameof(TenantError.BranchNotFoundError)]);

        try
        {
            branchRepository.Remove(branch);
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

    private static bool IsValid(string healthcareCenterId, string branchName, string address)
    {
        return !string.IsNullOrWhiteSpace(healthcareCenterId)
               && !string.IsNullOrWhiteSpace(branchName)
               && !string.IsNullOrWhiteSpace(address);
    }
}
