using System.Linq;
using Microsoft.EntityFrameworkCore;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace AndcultureCode.CSharp.Core.Interfaces
{
    public static class IQueryableResultExtensions
    {
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
