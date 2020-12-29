using System.Linq;
using AndcultureCode.CSharp.Core.Models.Errors;
using Microsoft.EntityFrameworkCore;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace AndcultureCode.CSharp.Core.Interfaces
{
    public static class IQueryableResultExtensions
    {
        public static IResult<T> FirstOrDefault<T>(
            this IResult<IQueryable<T>> result)
            where T : Entities.Entity
        {
            var transformedResult = new Result<T>
            {
                Errors = result.Errors
            };

            if (transformedResult.HasErrors)
            {
                return transformedResult;
            }

            transformedResult.ResultObject = result.ResultObject.FirstOrDefault();

            return transformedResult;
        }

        public static IResult<IQueryable<T>> Include<T>(
            this IResult<IQueryable<T>> result,
            string includeProperties,
            bool includeIfTrue = true)
            where T : Entities.Entity
        {
            if (result.HasErrors)
            {
                return result;
            }

            if (includeIfTrue && !string.IsNullOrEmpty(includeProperties?.Trim()))
            {
                result.ResultObject = result.ResultObject.Include(includeProperties);
            }

            return result;
        }
    }
}
