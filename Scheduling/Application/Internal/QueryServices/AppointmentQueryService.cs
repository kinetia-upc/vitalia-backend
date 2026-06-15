using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Domain.Repositories;

namespace VitaliaBackend.Scheduling.Application.Internal.QueryServices;

public class AppointmentQueryService(IAppointmentRepository appointmentRepository) : IAppointmentQueryService
{
    public async Task<Appointment?> Handle(GetAppointmentByIdQuery query, CancellationToken cancellationToken)
    {
        return await appointmentRepository.FindByPublicIdAsync(query.AppointmentId, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> Handle(GetAppointmentsQuery query, CancellationToken cancellationToken)
    {
        return await appointmentRepository.SearchAsync(
            query.DoctorId,
            query.PatientId,
            query.BranchId,
            query.Date,
            cancellationToken);
    }
}