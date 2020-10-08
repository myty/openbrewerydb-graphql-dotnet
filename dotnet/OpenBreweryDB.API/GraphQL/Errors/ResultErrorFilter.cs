using AndcultureCode.CSharp.Core.Interfaces;

namespace OpenBreweryDB.API.GraphQL.Errors
{
    public class ResultErrorFilter : HotChocolate.IErrorFilter
    {
        public HotChocolate.IError OnError(HotChocolate.IError error)
        {
            return error.Exception is ResultException resultException
                && resultException.Error is IError resultError
                ? error.WithCode(resultError.Key).WithMessage(resultError.Message)
                : error;
        }
    }
}
