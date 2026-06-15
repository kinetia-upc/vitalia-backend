using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context for Vitalia
/// </summary>
/// <param name="options">
///     The options for the database context
/// </param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
    public DbSet<SchedulingDoctor> SchedulingDoctors { get; set; }
    public DbSet<SchedulingPatient> SchedulingPatients { get; set; }
    public DbSet<SchedulingBranch> SchedulingBranches { get; set; }
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <summary>
    ///     On creating the database model
    /// </summary>
    /// <remarks>
    ///     This method is used to create the database model for the application.
    /// </remarks>
    /// <param name="builder">
    ///     The model builder for the database context
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplySchedulingConfiguration();
        // General Naming Convention for the database objects
        builder.UseSnakeCaseNamingConvention();
    }
}
