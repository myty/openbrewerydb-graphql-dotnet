using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;

namespace OpenBreweryDB.API.GraphQL.Errors
{
    public class ResultErrorFilter : HotChocolate.IErrorFilter
    {
        public HotChocolate.IError OnError(HotChocolate.IError error)
        {
            return error.Exception is ResultException resultException
                && resultException.Errors.FirstOrDefault() is IError firstError
                ? error.WithCode(firstError.Key).WithMessage(firstError.Message)
                : error;
        }
    }
}
