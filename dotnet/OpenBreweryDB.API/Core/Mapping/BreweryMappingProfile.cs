using AutoMapper;
using OpenBreweryDB.API.Controllers.Dto;
using OpenBreweryDB.API.Data.Models;

public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Brewery, BreweryDto>();
        }
    }