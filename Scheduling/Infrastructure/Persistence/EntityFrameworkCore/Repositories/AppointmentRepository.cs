using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AppointmentRepository(AppDbContext context)
    : BaseRepository<Appointment>(context), IAppointmentRepository
{
    public async Task<Appointment?> FindByPublicIdAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Appointment>()
            .FirstOrDefaultAsync(appointment => appointment.Code == publicId, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> SearchAsync(
        string? doctorId,
        string? patientId,
        string? branchId,
        DateOnly? date,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Appointment>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(doctorId))
            query = query.Where(appointment => appointment.DoctorId.ToString() == doctorId);

        if (!string.IsNullOrWhiteSpace(patientId))
            query = query.Where(appointment => appointment.PatientId.ToString() == patientId);

        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(appointment => appointment.BranchId == branchId);

        if (date.HasValue)
        {
            var startOfDay = date.Value.ToDateTime(TimeOnly.MinValue);
            var nextDay = date.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);

            query = query.Where(appointment =>
                appointment.ScheduledAt >= startOfDay &&
                appointment.ScheduledAt < nextDay);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveAppointmentForDoctorAtAsync(
        string doctorId,
        DateTime scheduledAt,
        string? excludingPublicId = null,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Appointment>()
            .AnyAsync(appointment =>
                    appointment.DoctorId.ToString() == doctorId
                    && appointment.ScheduledAt == scheduledAt
                    && (appointment.Status == EAppointmentStatus.Scheduled
                        || appointment.Status == EAppointmentStatus.Confirmed
                        || appointment.Status == EAppointmentStatus.Arrived
                        || appointment.Status == EAppointmentStatus.InAttention)
                    && (excludingPublicId == null || appointment.Code != excludingPublicId),
                cancellationToken);
    }
}
