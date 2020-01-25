using System.Linq;
using AutoMapper;
using DTO = OpenBreweryDB.Core.Models;
using OpenBreweryDB.Data.Models;

public class BreweryProfile : Profile
{
    public BreweryProfile()
    {
        // From: Brewery
        CreateMap<Brewery, DTO.Brewery>()
            .ForMember(
                dest => dest.Tags,
                opt => opt
                    .MapFrom(src => src.BreweryTags
                        .Select(bt => bt.Tag.Name)
                        .Distinct())
            );

        CreateMap<Brewery, DTO.AutocompleteBrewery>();

        // From: BreweryDto
        CreateMap<DTO.Brewery, Brewery>()
            .ForMember(
                dest => dest.BreweryTags,
                opt => opt
                    .MapFrom(src => src.Tags.Select(t => new BreweryTag { Tag = new Tag { Name = t } }))
            );
    }
}
