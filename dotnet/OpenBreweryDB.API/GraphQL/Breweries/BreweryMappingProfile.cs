using System.Linq;
using AutoMapper;
using OpenBreweryDB.Data.Models;
using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class BreweryMappingProfile : Profile
    {
        public BreweryMappingProfile()
        {
            // From: CreateBreweryInput
            CreateMap<CreateBreweryInput, DTO.Brewery>();
            CreateMap<UpdateBreweryInput, DTO.Brewery>();
        }
    }
}
