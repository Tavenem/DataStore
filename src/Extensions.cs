using System.Linq.Expressions;

namespace Tavenem.DataStorage;

/// <summary>
/// Extension methods.
/// </summary>
public static class DataStorageExtensions
{
    /// <summary>
    /// Extensions for <see cref="IIdItem{T}"/> types.
    /// </summary>
    extension<T>(IIdItem<T> item) where T : IIdItem<T>
    {
        /// <summary>
        /// <para>
        /// Gets the <see cref="IIdItem.IdItemTypeName"/> for any instance of this class as a static
        /// method.
        /// </para>
        /// <para>
        /// This method's default implementation is suitable only for the <see cref="IdItem"/> class
        /// itself. It should be overridden in subclasses to return the correct discriminator value.
        /// </para>
        /// </summary>
        /// <returns>The <see cref="IIdItem.IdItemTypeName"/> for any instance of this
        /// class.</returns>
        /// <remarks>
        /// <para>
        /// The value returned by this method is expected to start and end with the ':' character.
        /// </para>
        /// <para>
        /// Inheritance and polymorphism should be modeled by chaining subtypes with the ':'
        /// character as a separator.
        /// </para>
        /// <para>
        /// For example: ":BaseType:ChildType:".
        /// </para>
        /// </remarks>
        public string GetIdItemTypeName() => T.GetIdItemTypeName();
    }

    /// <summary>
    /// Returns a <see cref="PagedList{T}"/> wrapper for the current collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The current collection.</param>
    /// <param name="pageNumber">The current page number. The first page is 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
    /// <returns>An <see cref="PagedList{T}"/> containing the items in the current collection.</returns>
    public static PagedList<T> AsPagedList<T>(this IEnumerable<T> collection, long pageNumber, long pageSize, long totalCount)
        => new(collection, pageNumber, pageSize, totalCount);

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
    /// <returns>A combined expression.</returns>`
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
        private Dictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; }

        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = [];
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
