using AutoMapper;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using HotChocolate;
using System.Threading;
using HotChocolate.Execution;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public partial class BreweryMutations
    {
        public UpdateBreweryPayload UpdateBrewery(
            UpdateBreweryInput input,
            [Service] IBreweryValidationConductor validationConductor,
            [Service] IBreweryConductor breweryConductor,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var dto = mapper.Map<DTO.Brewery>(input);
            if (!validationConductor.CanUpdate(dto.Id ?? default, dto, out var errors))
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

            var brewery = breweryConductor.Update(mapper.Map<Entity.Brewery>(dto));

            if (!brewery.HasErrors && !(brewery.ResultObject is null))
            {
                return new UpdateBreweryPayload
                {
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
