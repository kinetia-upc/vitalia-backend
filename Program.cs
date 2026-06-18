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

//Clinical Bounded Context
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.Internal.CommandServices;
using VitaliaBackend.Clinical.Application.Internal.QueryServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

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
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPrescriptionDetailRepository, PrescriptionDetailRepository>();

builder.Services.AddScoped<IAppointmentQueryService, AppointmentQueryService>();
builder.Services.AddScoped<IAvailabilitySlotQueryService, AvailabilitySlotQueryService>();
builder.Services.AddScoped<IMedicineQueryService, MedicineQueryService>();
builder.Services.AddScoped<IMedicalRecordQueryService, MedicalRecordQueryService>();
builder.Services.AddScoped<IDiagnosisQueryService, DiagnosisQueryService>();
builder.Services.AddScoped<ITreatmentQueryService, TreatmentQueryService>();
builder.Services.AddScoped<IPrescriptionQueryService, PrescriptionQueryService>();
builder.Services.AddScoped<IPrescriptionDetailQueryService, PrescriptionDetailQueryService>();

builder.Services.AddScoped<IAppointmentCommandService, AppointmentCommandService>();
builder.Services.AddScoped<IAvailabilitySlotCommandService, AvailabilitySlotCommandService>();
builder.Services.AddScoped<IMedicineCommandService, MedicineCommandService>();
builder.Services.AddScoped<IMedicalRecordCommandService, MedicalRecordCommandService>();
builder.Services.AddScoped<IDiagnosisCommandService, DiagnosisCommandService>();
builder.Services.AddScoped<ITreatmentCommandService, TreatmentCommandService>();
builder.Services.AddScoped<IPrescriptionCommandService, PrescriptionCommandService>();
builder.Services.AddScoped<IPrescriptionDetailCommandService, PrescriptionDetailCommandService>();

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

    DbSeeder.SeedAsync(context).GetAwaiter().GetResult();
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
