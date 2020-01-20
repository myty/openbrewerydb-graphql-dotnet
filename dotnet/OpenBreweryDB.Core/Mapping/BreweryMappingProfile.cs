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
            )
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.BreweryId)
            );

        CreateMap<Brewery, DTO.AutocompleteBrewery>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.BreweryId)
            );

        // From: BreweryDto
        CreateMap<DTO.Brewery, Brewery>()
            .ForMember(
                dest => dest.BreweryId,
                opt => opt.MapFrom(src => src.Id));
    }
}