using System.Linq;
using System.Threading;
using AutoMapper;
using HotChocolate;
using HotChocolate.Execution;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public partial class BreweryMutations
    {
        public CreateBreweryPayload CreateBrewery(
            CreateBreweryInput input,
            [Service] IBreweryValidationConductor validationConductor,
            [Service] IBreweryConductor breweryConductor,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            var dto = mapper.Map<DTO.Brewery>(input);
            if (!validationConductor.CanCreate(dto, out var errors))
            {
                return new CreateBreweryPayload(
                    errors.Select(err => new UserError(err.message, err.key)).ToList(),
                    input.ClientMutationId);
            }

            var brewery = breweryConductor.Create(mapper.Map<Entity.Brewery>(dto));

            if (brewery.HasErrors || !(brewery.ResultObject is object))
            {
                return new CreateBreweryPayload(
                    brewery.Errors.Select(err => new UserError(err.Message, err.Key)).ToList(),
                    input.ClientMutationId);
            }

            return new CreateBreweryPayload(brewery.ResultObject, input.ClientMutationId);
        }
    }
}
