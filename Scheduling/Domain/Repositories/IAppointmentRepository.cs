using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Domain.Repositories;

public interface IAppointmentRepository : IBaseRepository<Appointment>
{
    Task<Appointment?> FindByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveAppointmentForDoctorAtAsync(
        string doctorId,
        DateTime scheduledAt,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Appointment>> SearchAsync(
        string? doctorId,
        string? patientId,
        string? branchId,
        DateOnly? date,
        CancellationToken cancellationToken = default); 
}