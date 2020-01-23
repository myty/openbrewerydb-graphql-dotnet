using GraphQL.Types;

namespace OpenBreweryDB.API.GraphQL.InputTypes
{
    public class BreweryInputType : InputObjectGraphType
    {
        public BreweryInputType()
        {
            Name = "BreweryInput";
            Field<NonNullGraphType<StringGraphType>>("brewery_type");
            Field<StringGraphType>("city");
            Field<StringGraphType>("country");
            Field<IdGraphType>("id");
            Field<DecimalGraphType>("longitude");
            Field<DecimalGraphType>("latitude");
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("phone");
            Field<StringGraphType>("postal_code");
            Field<NonNullGraphType<StringGraphType>>("state");
            Field<StringGraphType>("street");
            Field<ListGraphType<StringGraphType>>("tags");
            Field<DateTimeGraphType>("updated_at");
            Field<StringGraphType>("website_url");
        }
    }
}