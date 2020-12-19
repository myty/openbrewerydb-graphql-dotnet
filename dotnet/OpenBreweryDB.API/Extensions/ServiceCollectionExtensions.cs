using Microsoft.Extensions.DependencyInjection;
using OpenBreweryDB.Core.Conductors.Breweries;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Conductors.BreweryTags;
using OpenBreweryDB.Core.Conductors.BreweryTags.Interfaces;
using OpenBreweryDB.Core.Conductors.Tags;
using OpenBreweryDB.Core.Conductors.Tags.Interfaces;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;

namespace OpenBreweryDB.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenBreweryServices(this IServiceCollection services) => services
            .AddScoped<IBreweryConductor, BreweryConductor>()
            .AddScoped<ITagConductor, TagConductor>()
            .AddScoped<IBreweryTagConductor, BreweryTagConductor>()
            .AddScoped<IUserConductor, UserConductor>()
            .AddScoped<IBreweryFilterConductor, BreweryFilterConductor>()
            .AddScoped<IBreweryOrderConductor, BreweryOrderConductor>()
            .AddScoped<IBreweryValidationConductor, BreweryValidationConductor>();
    }
}
