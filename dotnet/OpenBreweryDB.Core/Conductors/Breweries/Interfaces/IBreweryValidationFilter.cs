using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries.Interfaces
{
    public interface IBreweryValidationConductor
    {
        bool CanSearch(
            string by_state,
            string by_type,
            out (string key, string message)[] validationErrors);

        bool CanCreate(Brewery dto, out (string key, string message)[] validationErrors);

        bool CanUpdate(long id, Brewery dto, out (string key, string message)[] validationErrors);
    }
}
