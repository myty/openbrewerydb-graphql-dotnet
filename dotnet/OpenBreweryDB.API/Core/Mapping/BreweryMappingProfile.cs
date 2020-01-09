using System.Threading.Tasks.Dataflow;
using System.Linq;
using AutoMapper;
using OpenBreweryDB.API.Controllers.Dto;
using OpenBreweryDB.API.Data.Models;

public class BreweryProfile : Profile
{
    public BreweryProfile()
    {
        CreateMap<Brewery, BreweryDto>()
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

        CreateMap<BreweryDto, Brewery>()
            .ForMember(
                dest => dest.BreweryId,
                opt => opt.MapFrom(src => src.Id));
    }
}