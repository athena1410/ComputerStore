//-----------------------------------------------------------------------
// <copyright file="GenericSorter.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ComputerStore.Structure.Extensions
{
    public static class GenericSorter
    {
        /// <summary>
        /// Extension method of iqueryable use to sort by column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string sortBy, string sortDirection)
        {
            var param = Expression.Parameter(typeof(T), "item");

            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

            return (sortDirection.ToLower()) switch
            {
                "asc" => source.OrderBy(sortExpression),
                _ => source.OrderByDescending(sortExpression),
            };
        }
    }
}
