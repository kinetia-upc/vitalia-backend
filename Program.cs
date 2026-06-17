using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using VitaliaBackend.Resources.Errors;

//Shared Bounded Context
using VitaliaBackend.Resources.Shared;
using VitaliaBackend.Shared.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Mediator.Cortex.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using ProblemDetailsFactory = VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

//Scheduling Bounded Context
using VitaliaBackend.Scheduling.Application.CommandServices;
using VitaliaBackend.Scheduling.Application.Internal.CommandServices;
using VitaliaBackend.Scheduling.Application.Internal.QueryServices;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Repositories;
using VitaliaBackend.Scheduling.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Pharmacy.Application.CommandServices;
using VitaliaBackend.Pharmacy.Application.Internal.CommandServices;
using VitaliaBackend.Pharmacy.Application.Internal.QueryServices;
using VitaliaBackend.Pharmacy.Application.QueryServices;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Pharmacy.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();
builder.Services.AddProblemDetails();


//Builders Scheduling Bounded Context
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAvailabilitySlotRepository, AvailabilitySlotRepository>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();

builder.Services.AddScoped<IAppointmentQueryService, AppointmentQueryService>();
builder.Services.AddScoped<IAvailabilitySlotQueryService, AvailabilitySlotQueryService>();
builder.Services.AddScoped<IMedicineQueryService, MedicineQueryService>();

builder.Services.AddScoped<IAppointmentCommandService, AppointmentCommandService>();
builder.Services.AddScoped<IAvailabilitySlotCommandService, AvailabilitySlotCommandService>();
builder.Services.AddScoped<IMedicineCommandService, MedicineCommandService>();

//-----------------------


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services.AddSingleton<IStringLocalizer<CommonMessages>, StringLocalizer<CommonMessages>>();
builder.Services.AddSingleton<ProblemDetailsFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Vitalia Backend",
            Version = "v1",
            Description = "Vitalia Platform API",
            Contact = new OpenApiContact
            {
                Name = "Vitalia",
                Email = "contact@vitalia.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
    options.EnableAnnotations();
});

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mediator Configuration
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Set<VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine>().Any())
    {
        context.Set<VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine>().AddRange(
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Paracetamol", 500, "mg", 5.50m, 100),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Ibuprofeno", 400, "mg", 7.20m, 50),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Amoxicilina", 500, "mg", 12.00m, 30),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Losartán", 50, "mg", 15.00m, 45),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Omeprazol", 20, "mg", 8.50m, 60),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Atorvastatina", 20, "mg", 18.00m, 25),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Metformina", 850, "mg", 10.00m, 80),
            new VitaliaBackend.Pharmacy.Domain.Model.Aggregates.Medicine("Salbutamol", 100, "mcg", 22.00m, 15)
        );
        context.SaveChanges();
    }

    if (!context.Set<VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot>().Any())
    {
        context.Set<VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot>().AddRange(
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("slot-003", "doc-003", "branch-001", DateOnly.Parse("2026-06-10"), TimeOnly.Parse("15:15"), TimeOnly.Parse("15:45"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("slot-004", "doc-001", "branch-001", DateOnly.Parse("2026-05-12"), TimeOnly.Parse("16:00"), TimeOnly.Parse("16:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Booked),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("slot-005", "doc-002", "branch-001", DateOnly.Parse("2026-05-14"), TimeOnly.Parse("10:30"), TimeOnly.Parse("11:00"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("slot-006", "doc-003", "branch-001", DateOnly.Parse("2026-05-15"), TimeOnly.Parse("09:00"), TimeOnly.Parse("09:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("8a1b163d-a15d-40f9-a136-730a4dd2bae9", "doc-004", "branch-001", DateOnly.Parse("2026-05-12"), TimeOnly.Parse("08:00"), TimeOnly.Parse("08:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("c0d6ceee-f26e-441c-af28-c002466e3310", "doc-004", "branch-001", DateOnly.Parse("2026-05-12"), TimeOnly.Parse("09:00"), TimeOnly.Parse("09:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("087b303d-5231-4554-9415-086a0171af97", "doc-001", "branch-001", DateOnly.Parse("2026-04-24"), TimeOnly.Parse("11:00"), TimeOnly.Parse("11:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Booked),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("a145580e-c559-47d2-b321-b630295d1d57", "doc-003", "branch-001", DateOnly.Parse("2026-05-12"), TimeOnly.Parse("12:00"), TimeOnly.Parse("12:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("519f6cf7-73b5-41f9-a664-29b748601ce1", "doc-003", "branch-001", DateOnly.Parse("2026-05-12"), TimeOnly.Parse("13:00"), TimeOnly.Parse("13:30"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Available),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("c79caf3c-c323-4c37-82c5-67c78db90c33", "doc-001", "branch-001", DateOnly.Parse("2026-05-11"), TimeOnly.Parse("12:20"), TimeOnly.Parse("12:50"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Booked),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.AvailabilitySlot("46f5fe5f-e71f-486f-aed7-faa8f2b0a56d", "doc-001", "branch-001", DateOnly.Parse("2026-05-11"), TimeOnly.Parse("10:15"), TimeOnly.Parse("10:45"), VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAvailabilitySlotStatus.Booked)
        );
        context.SaveChanges();
    }

    if (!context.Set<VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment>().Any())
    {
        context.Set<VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment>().AddRange(
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-001", "doc-001", "pat-001", "branch-001", DateTime.Parse("2026-04-24T09:30:00"), "Advanced Cardiology Consultation", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Paid),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-002", "doc-001", "pat-002", "branch-001", DateTime.Parse("2026-04-24T11:00:00"), "Diabetes Screening", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Released, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Paid),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-003", "doc-001", "pat-003", "branch-001", DateTime.Parse("2026-04-24T14:00:00"), "Cardiology Review", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Arrived, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Paid),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-004", "doc-002", "pat-001", "branch-001", DateTime.Parse("2026-05-14T10:30:00"), "Neurological Screening", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-005", "doc-003", "pat-001", "branch-001", DateTime.Parse("2026-06-10T15:15:00"), "General Wellness Check", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("apt-006", "doc-004", "pat-001", "branch-001", DateTime.Parse("2026-06-12T09:00:00"), "Physical Therapy", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Refunded),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("1856ba33-0768-4189-b8c7-e61412e12c13", "doc-001", "pat-001", "branch-001", DateTime.Parse("2026-04-24T09:30:00"), "General consultation", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("80246866-4aed-4675-99d1-d03b89654dc0", "doc-001", "pat-001", "branch-001", DateTime.Parse("2026-04-24T09:30:00"), "General consultation", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Released, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("67443c08-d81b-4458-9bc1-a3142ef7b714", "doc-003", "pat-001", "branch-001", DateTime.Parse("2026-05-15T09:00:00"), "General consultation", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending),
            new VitaliaBackend.Scheduling.Domain.Model.Aggregates.Appointment("02caea11-7484-41ee-8015-3da5e7a8905e", "doc-001", "pat-001", "branch-001", DateTime.Parse("2026-05-12T16:00:00"), "General consultation", VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EAppointmentStatus.Cancelled, VitaliaBackend.Scheduling.Domain.Model.ValueObjects.EPaymentStatus.Pending)
        );
        context.SaveChanges();
    }
}

app.UseGlobalExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
