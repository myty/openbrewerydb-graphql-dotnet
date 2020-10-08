using System;
using AndcultureCode.CSharp.Core.Interfaces;

namespace OpenBreweryDB.API.GraphQL.Errors
{
    public class ResultException : Exception
    {
        public ResultException(IError error)
        {
            Error = error;
        }

        public IError Error { get; }
    }
}
