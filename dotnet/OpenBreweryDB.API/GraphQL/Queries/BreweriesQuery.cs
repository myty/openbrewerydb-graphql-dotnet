using AndcultureCode.CSharp.Core.Extensions;
using AutoMapper;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBreweryDB.API.GraphQL.Queries
{
    public class BreweriesQuery : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor
                .Field("breweries")
                .UsePaging<BreweryType>()
                .Argument(
                    "state",
                    a => a
                        .Type<StringType>()
                        .Description("filter by state")
                )
                .Argument(
                    "type",
                    a => a
                        .Type<StringType>()
                        .Description("filter by type")
                )
                .Argument(
                    "city",
                    a => a
                        .Type<StringType>()
                        .Description("search by city name")
                )
                .Argument(
                    "name",
                    a => a
                        .Type<StringType>()
                        .Description("search by brewery name")
                )
                .Argument(
                    "search",
                    a => a
                        .Type<StringType>()
                        .Description("general search")
                )
                .Argument(
                    "sort",
                    a => a
                        .Type<ListType<StringType>>()
                        .Description("sort by")
                        .DefaultValue(Array.Empty<string>())
                )
                .Argument(
                    "tags",
                    a => a
                        .Type<ListType<StringType>>()
                        .Description("filter by tags")
                        .DefaultValue(Array.Empty<string>())
                )
                .Resolver(
                    ctx =>
                    {
                        // Arguments
                        var name = ctx.Argument<string>("name");
                        var state = ctx.Argument<string>("state");
                        var city = ctx.Argument<string>("city");
                        var type = ctx.Argument<string>("type");
                        var search = ctx.Argument<string>("search");
                        var sort = ctx.Argument<IEnumerable<string>>("sort");
                        var tags = ctx.Argument<IEnumerable<string>>("tags");

                        // Dependencies
                        var breweryConductor = ctx.Service<IBreweryConductor>();
                        var validationConductor = ctx.Service<IBreweryValidationConductor>();
                        var filterConductor = ctx.Service<IBreweryFilterConductor>();
                        var orderConductor = ctx.Service<IBreweryOrderConductor>();
                        var mapper = ctx.Service<IMapper>();

                        if (!validationConductor.CanSearch(state, type, out var errors))
                        {
                            foreach (var (key, message) in errors)
                            {
                                ctx.ReportError(
                                    ErrorBuilder.New()
                                        .SetCode(key)
                                        .SetPath(ctx.Path)
                                        .AddLocation(ctx.FieldSelection)
                                        .SetMessage(message)
                                        .Build()
                                );
                            }

                            return null;
                        }

                        var filter = filterConductor.BuildFilter(
                            by_name: name,
                            by_state: state,
                            by_type: type,
                            by_city: city,
                            by_tags: tags);

                        if (!string.IsNullOrEmpty(search))
                        {
                            filter = filter.AndAlso(filterConductor
                                .BuildFilter(by_name: search));
                        }

                        // Sorting
                        Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null;
                        if (sort != null)
                        {
                            orderBy = orderConductor.OrderByFields(
                                sort?
                                    .Select(s => s.FirstOrDefault() == '-'
                                        ? new KeyValuePair<string, SortDirection>(s.Substring(1), SortDirection.DESC)
                                        : new KeyValuePair<string, SortDirection>(s, SortDirection.ASC))
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                            );
                        }

                        var result = breweryConductor.FindAllQueryable(filter: filter, orderBy: orderBy);

                        if (!result.HasErrorsOrResultIsNull())
                        {
                            return result.ResultObject;
                        }

                        foreach (var err in result.Errors)
                        {
                            ctx.ReportError(
                                ErrorBuilder.New()
                                    .SetCode(err.Key)
                                    .SetPath(ctx.Path)
                                    .AddLocation(ctx.FieldSelection)
                                    .SetMessage(err.Message)
                                    .Build()
                            );
                        }

                        return null;
                    }
                );
        }
    }
}
