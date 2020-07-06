using System.Linq;
using AutoMapper;
using DTO = OpenBreweryDB.Core.Models;
using OpenBreweryDB.Data.Models;

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
