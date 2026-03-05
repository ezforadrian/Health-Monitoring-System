using _VEHRSv1.Interface;
using _VEHRSv1.Repository;

namespace _VEHRSv1.Services
{
    public static class ServiceRegistration
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IAppReferenceRepository, AppReferenceRepository>();
            services.AddScoped<IAS400PlantillaRepository, AS400PlantillaRepository>();
            services.AddScoped<IECURepository, ECURepository>();
            services.AddScoped<IPEMERepository, PEMERepository>();
            services.AddScoped<IFileValidationService, FileValidationServices>();
            services.AddScoped<IAccountManagementRepository, AccountManagementRepository>();
            services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
            services.AddScoped<IAMERepository, AMERepository>();
            services.AddScoped<IMwrRepository, MwrRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<EncryptionService>();
        }
    }
}
