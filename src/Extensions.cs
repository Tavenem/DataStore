using System.Linq.Expressions;

namespace Tavenem.DataStorage;

/// <summary>
/// Extension methods.
/// </summary>
public static class DataStorageExtensions
{
    /// <summary>
    /// Returns a <see cref="IPagedList{T}"/> wrapper for the current collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The current collection.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
    /// <returns>An <see cref="IPagedList{T}"/> containing the items in the current collection.</returns>
    public static IPagedList<T> AsPagedList<T>(this IEnumerable<T> collection, long pageNumber, long pageSize, long totalCount)
        => new PagedList<T>(collection, pageNumber, pageSize, totalCount);

    /// <summary>
    /// Combines this <see cref="Expression{T}"/> with the given <see cref="Expression{T}"/> in
    /// a conditional AND operation which evaluates the second expression only if the first
    /// evaluates to <see langword="true"/>.
    /// </summary>
    /// <param name="first">
    /// An <see cref="Expression{T}"/> which represents a <see cref="Func{T}"/> that returns a
    /// <see cref="bool"/>.
    /// </param>
    /// <param name="second">
    /// An <see cref="Expression{T}"/> which represents a <see cref="Func{T}"/> that returns a
    /// <see cref="bool"/>.
    /// </param>
    /// <returns>A combined expression.</returns>
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        => Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                first.Body,
                new ExpressionParameterReplacer(second.Parameters, first.Parameters).Visit(second.Body)),
            first.Parameters);

    /// <summary>
    /// Combines this <see cref="Expression{T}"/> with the given <see cref="Expression{T}"/> in
    /// a conditional OR operation which evaluates the second expression only if the first
    /// evaluates to <see langword="false"/>.
    /// </summary>
    /// <param name="first">
    /// An <see cref="Expression{T}"/> which represents a <see cref="Func{T}"/> that returns a
    /// <see cref="bool"/>.
    /// </param>
    /// <param name="second">
    /// An <see cref="Expression{T}"/> which represents a <see cref="Func{T}"/> that returns a
    /// <see cref="bool"/>.
    /// </param>
    /// <returns>A combined expression.</returns>
    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        => Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                first.Body,
                new ExpressionParameterReplacer(second.Parameters, first.Parameters).Visit(second.Body)),
            first.Parameters);

    private class ExpressionParameterReplacer : ExpressionVisitor
    {
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; }

        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (var i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
            {
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
            }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (ParameterReplacements.TryGetValue(node, out var replacement))
            {
                node = replacement;
            }

            return base.VisitParameter(node);
        }
    }
}
