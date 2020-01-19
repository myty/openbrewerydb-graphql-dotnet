using GraphQL.Types;

namespace OpenBreweryDB.Core.Types
{
    public class BreweryTypeEnum : EnumerationGraphType
    {
        public BreweryTypeEnum()
        {
            Name = "BreweryType";
            Description = "Types of Breweries";

            // "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor"
            AddValue("micro", "micro", BreweryType.MICRO);
            AddValue("regional", "regional", BreweryType.REGIONAL);
            AddValue("brewpub", "brewpub", BreweryType.BREWPUB);
            AddValue("large", "large", BreweryType.LARGE);
            AddValue("planning", "planning", BreweryType.PLANNING);
            AddValue("bar", "bar", BreweryType.BAR);
            AddValue("contract", "contract", BreweryType.CONTRACT);
            AddValue("proprietor", "proprietor", BreweryType.PROPRIETOR);
        }
    }

    public enum BreweryType
    {
        MICRO,
        REGIONAL,
        BREWPUB,
        LARGE,
        PLANNING,
        BAR,
        CONTRACT,
        PROPRIETOR
    }
}