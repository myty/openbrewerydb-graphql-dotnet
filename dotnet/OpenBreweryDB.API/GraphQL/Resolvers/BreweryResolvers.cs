using AutoMapper;
using HotChocolate;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Resolvers
{
    public class BreweryResolvers
    {
        // public IEnumerable<DTO.Brewery> GetBreweries(
        //     [Parent]ICharacter character,
        //     [Service]CharacterRepository repository)
        // {
        //     foreach (string friendId in character.Friends)
        //     {
        //         ICharacter friend = repository.GetCharacter(friendId);
        //         if (friend != null)
        //         {
        //             yield return friend;
        //         }
        //     }
        // }
    }
}
