using AutoMapper;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using HotChocolate.Types;
using HotChocolate;
using System.Threading;
using HotChocolate.Execution;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    [ExtendObjectType(Name = "Mutation")]
    public class BreweryMutations
    {
        public CreateBreweryPayload CreateBrewery(
            CreateBreweryInput input,
            [Service]IBreweryValidationConductor validationConductor,
            [Service]IBreweryConductor breweryConductor,
            [Service]IMapper mapper,
            CancellationToken cancellationToken)
        {
            var dto = mapper.Map<DTO.Brewery>(input);
            if (!validationConductor.CanCreate(dto, out var errors))
            {
                foreach (var (key, message) in errors)
                {
                    throw new QueryException(
                        ErrorBuilder.New()
                            .SetCode(key)
                            .SetMessage(message)
                            .Build()
                    );
                }

                return null;
            }

            var brewery = breweryConductor.Create(mapper.Map<Entity.Brewery>(dto));

            if (!brewery.HasErrors && brewery.ResultObject is object)
            {
                return new CreateBreweryPayload {
                    Brewery = brewery.ResultObject,
                    ClientMutationId = input.ClientMutationId
                };
            }

            foreach (var err in brewery.Errors)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetCode(err.Key)
                        .SetMessage(err.Message)
                        .Build()
                );
            }

            return null;
        }
    }
}
