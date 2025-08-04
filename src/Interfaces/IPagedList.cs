using System.Text.Json.Serialization;
using Tavenem.DataStorage.PagedLists;

namespace Tavenem.DataStorage;

/// <summary>
/// A list of items which is a subset of a larger collection, with information about the place
/// of this subset within the overall collection.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
[JsonConverter(typeof(IPagedListConverter))]
public interface IPagedList<out T> : IReadOnlyList<T>
{
    /// <summary>
    /// The zero-based index of the first item in the current page, within the whole collection.
    /// </summary>
    long FirstIndexOnPage { get; }

    /// <summary>
    /// Whether there is next page available.
    /// </summary>
    bool HasNextPage { get; }

    /// <summary>
    /// Whether there is a previous page available.
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// The zero-based index of the last item in the current page, within the whole collection.
    /// </summary>
    long LastIndexOnPage { get; }

    /// <summary>
    /// <para>
    /// The current page number.
    /// </para>
    /// <para>
    /// The first page is 1.
    /// </para>
    /// </summary>
    long PageNumber { get; }

    /// <summary>
    /// The page size.
    /// </summary>
    long PageSize { get; }

    /// <summary>
    /// <para>
    /// The total number of results, of which this page is a subset.
    /// </para>
    /// <para>
    /// Implementations are not required to provide this value. It may be <see
    /// langword="null"/>, which indicates that the total is unknown.
    /// </para>
    /// </summary>
    long? TotalCount { get; }

    /// <summary>
    /// <para>
    /// The total number of pages.
    /// </para>
    /// <para>
    /// Implementations are not required to provide this value. It may be <see
    /// langword="null"/>, which indicates that the total is unknown.
    /// </para>
    /// </summary>
    long? TotalPages { get; }
}
