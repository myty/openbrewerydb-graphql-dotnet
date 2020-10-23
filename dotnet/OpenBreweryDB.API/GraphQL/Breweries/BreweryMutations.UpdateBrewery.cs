using System.Linq;
using System.Threading;
using AutoMapper;
using HotChocolate;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

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
                return new UpdateBreweryPayload(
                    errors.Select(err => new UserError(err.message, err.key)).ToList(),
                    input.ClientMutationId);
            }

            var brewery = breweryConductor.Update(mapper.Map<Entity.Brewery>(dto));

            if (!brewery.HasErrors && !(brewery.ResultObject is null))
            {
                return new UpdateBreweryPayload(brewery.ResultObject, input.ClientMutationId);
            }

            return new UpdateBreweryPayload(
                brewery.Errors.Select(err => new UserError(err.Message, err.Key)).ToList(),
                input.ClientMutationId);
        }
    }
}
