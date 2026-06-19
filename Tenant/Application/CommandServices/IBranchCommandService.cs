using VitaliaBackend.Shared.Application.Model;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Commands;

namespace VitaliaBackend.Tenant.Application.CommandServices;

public interface IBranchCommandService
{
    Task<Result<Branch>> Handle(CreateBranchCommand command, CancellationToken cancellationToken);
    Task<Result<Branch>> Handle(UpdateBranchCommand command, CancellationToken cancellationToken);
    Task<Result> Handle(DeleteBranchCommand command, CancellationToken cancellationToken);
}
