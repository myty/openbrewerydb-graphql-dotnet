using System;
using System.Collections.Generic;
using AndcultureCode.CSharp.Core.Interfaces;

namespace OpenBreweryDB.API.GraphQL.Errors
{
    public class ResultException : Exception
    {
        public ResultException(List<IError> errors)
        {
            Errors = errors;
        }

        public List<IError> Errors { get; }
    }
}
