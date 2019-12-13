using System.Collections.Generic;
using System;
using OpenBreweryDB.API.Data.Core;
using OpenBreweryDB.API.Core.Dto;
using OpenBreweryDB.API.Data.Models;

namespace OpenBreweryDB.API.Controllers.Dto
{
    public class BreweryDto: BaseDto
    {
        public string Name { get; set; }
        public string BreweryType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string WebsiteURL { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Country { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}