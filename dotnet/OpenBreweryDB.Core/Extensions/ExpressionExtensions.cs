using System;
using System.Linq.Expressions;

namespace OpenBreweryDB.Core.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Adds another expression filter to original expression using AndAlso operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Additional filter expression on the same type of object as the main expression</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        /// <summary>
        /// Adds another expression filter to original expression using AndAlso operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <typeparam name="TNav">Type of the navigation property (can be deeply nested)</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Filter expression that filters on the navigated property</param>
        /// <param name="navigationProperty">Expression to the navigation property (eg e => e.PropA.PropB) </param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T, TNav>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<TNav, bool>> expr2,
            Expression<Func<T, TNav>> navigationProperty
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var navVisitor = new ReplaceExpressionVisitor(navigationProperty.Parameters[0], parameter);
            var nav = navVisitor.Visit(navigationProperty.Body);                                    // Reset the navigation prop expression to start from the shared parameter
            var right = Expression.Invoke(expr2, nav);                                                // Create an expression that navigates to the property

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        /// <summary>
        /// Adds another expression filter to original expression using Or operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Additional filter expression on the same type of object as the main expression</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.Or(left, right), parameter);
        }

        /// <summary>
        /// Adds another expression filter to original expression using Or operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <typeparam name="TNav">Type of the navigation property (can be deeply nested)</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Filter expression that filters on the navigated property</param>
        /// <param name="navigationProperty">Expression to the navigation property (eg e => e.PropA.PropB) </param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T, TNav>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<TNav, bool>> expr2,
            Expression<Func<T, TNav>> navigationProperty
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var navVisitor = new ReplaceExpressionVisitor(navigationProperty.Parameters[0], parameter);
            var nav = navVisitor.Visit(navigationProperty.Body);                                    // Reset the navigation prop expression to start from the shared parameter
            var right = Expression.Invoke(expr2, nav);                                                // Create an expression that navigates to the property

            return Expression.Lambda<Func<T, bool>>(Expression.Or(left, right), parameter);
        }

        /// <summary>
        /// Adds another expression filter to original expression using OrElse operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Additional filter expression on the same type of object as the main expression</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        /// <summary>
        /// Adds another expression filter to original expression using OrElse operator
        /// </summary>
        /// <typeparam name="T">Type of object in the main filter</typeparam>
        /// <typeparam name="TNav">Type of the navigation property (can be deeply nested)</typeparam>
        /// <param name="expr1">Main filter expression</param>
        /// <param name="expr2">Filter expression that filters on the navigated property</param>
        /// <param name="navigationProperty">Expression to the navigation property (eg e => e.PropA.PropB) </param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T, TNav>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<TNav, bool>> expr2,
            Expression<Func<T, TNav>> navigationProperty
        )
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var navVisitor = new ReplaceExpressionVisitor(navigationProperty.Parameters[0], parameter);
            var nav = navVisitor.Visit(navigationProperty.Body);                                    // Reset the navigation prop expression to start from the shared parameter
            var right = Expression.Invoke(expr2, nav);                                                // Create an expression that navigates to the property

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

    }

    class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _newValue;
        private readonly Expression _oldValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
