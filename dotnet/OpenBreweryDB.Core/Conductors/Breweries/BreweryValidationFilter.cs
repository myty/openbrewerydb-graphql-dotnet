using System;
using System.Collections.Generic;
using System.Linq;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.Core.Conductors.Breweries
{
    public class BreweryValidationConductor : IBreweryValidationConductor
    {
        public bool CanSearch(
            string by_state,
            string by_type,
            out (string key, string message)[] validationErrors)
        {
            var errorList = new List<(string key, string err)>();

            if (!string.IsNullOrEmpty(by_state?.Trim()) && by_state.Contains(" "))
            {
                errorList.Add(("by_state", "by_state must contain the full state name in snake_case, no abbreviation (ex. \"new_york\")"));
            }

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (!(string.IsNullOrEmpty(by_type?.Trim()) || allowedTypes.Contains(by_type)))
            {
                errorList.Add(("by_type", "by_type must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor."));
            }

            validationErrors = errorList.ToArray();

            return !errorList.Any();
        }

        public bool CanCreate(Brewery dto, out (string key, string message)[] validationErrors)
        {
            var errorList = new List<(string key, string message)>();

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (string.IsNullOrEmpty(dto.BreweryType?.Trim()) || !allowedTypes.Contains(dto.BreweryType))
            {
                errorList.Add(("brewery_type", "BreweryType must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor."));
            }

            validationErrors = errorList.ToArray();

            return !errorList.Any();
        }

        public bool CanUpdate(long id, Brewery dto, out (string key, string message)[] validationErrors)
        {
            var errorList = new List<(string key, string message)>();

            if (id == default)
            {
                errorList.Add(("brewery_id", "Id must be provided in order to update a record."));
            }
            else if (id != dto.Id)
            {
                errorList.Add(("brewery_id", "Ids for the object and route must match"));
            }

            var allowedTypes = new string[] { "micro", "regional", "brewpub", "large", "planning", "bar", "contract", "proprietor" };
            if (string.IsNullOrEmpty(dto.BreweryType?.Trim()) || !allowedTypes.Contains(dto.BreweryType))
            {
                errorList.Add(("brewery_type", "BreweryType must be one of the following: micro, regional, brewpub, large, planning, bar, contract, proprietor."));
            }

            validationErrors = errorList.ToArray();

            return !errorList.Any();
        }
    }
}
