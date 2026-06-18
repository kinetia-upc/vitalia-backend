using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class TreatmentRepository(AppDbContext context)
    : BaseRepository<Treatment>(context), ITreatmentRepository
{
}
