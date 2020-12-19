using System.Linq;
using Microsoft.EntityFrameworkCore;
using Entities = AndcultureCode.CSharp.Core.Models.Entities;

namespace AndcultureCode.CSharp.Core.Interfaces
{
    public static class IQueryableResultExtensions
    {
        public static IResult<IQueryable<T>> Take<T>(
            this IResult<IQueryable<T>> result,
            int take)
            where T : Entities.Entity
        {
            result.ResultObject = result.ResultObject.Take(take);

            return result;
        }

        public static IResult<IQueryable<T>> Include<T>(
            this IResult<IQueryable<T>> result,
            string includeProperties)
            where T : Entities.Entity
        {
            if (!string.IsNullOrEmpty(includeProperties?.Trim()))
            {
                result.ResultObject = result.ResultObject.Include(includeProperties);
            }

            return result;
        }

        public static IResult<IQueryable<T>> Include<T>(
            this IResult<IQueryable<T>> result,
            string includeProperties,
            bool includeIfTrue)
            where T : Entities.Entity
        {
            if (includeIfTrue && !string.IsNullOrEmpty(includeProperties?.Trim()))
            {
                result.ResultObject = result.ResultObject.Include(includeProperties);
            }

            return result;
        }
    }
}
