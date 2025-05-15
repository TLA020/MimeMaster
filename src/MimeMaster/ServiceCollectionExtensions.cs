using Microsoft.Extensions.DependencyInjection;
using MimeMaster.Services;

namespace MimeMaster;

/// <summary>
/// Extension methods for setting up MimeMaster services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MimeMaster services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMimeMaster(this IServiceCollection services)
    {
        services.AddScoped<IFileTypeService, FileTypeService>();
        services.AddScoped<IValidationService, ValidationService>();
        return services;
    }
}
