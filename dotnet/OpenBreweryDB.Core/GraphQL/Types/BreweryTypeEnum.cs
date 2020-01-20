using GraphQL.Types;

namespace OpenBreweryDB.Core.GraphQL.Types
{
    public class BreweryTypeEnum : EnumerationGraphType
    {
        public BreweryTypeEnum()
        {
            Name = "BreweryType";
            Description = "Types of Breweries";

            var types = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            foreach (var type in types)
            {
                AddValue(type, type, type);
            }
        }
    }
}