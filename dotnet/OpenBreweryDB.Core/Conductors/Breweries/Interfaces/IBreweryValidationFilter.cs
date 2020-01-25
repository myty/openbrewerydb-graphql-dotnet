using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryValidationConductor
    {
        bool CanSearch(
            string by_state,
            string by_type,
            out string[] errors);

        bool CanCreate(Brewery dto, out string[] errors);

        bool CanUpdate(long id, Brewery dto, out string[] errors);
    }
}
