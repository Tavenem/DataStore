using System.Collections;
using System.Text.Json.Serialization;

namespace Tavenem.DataStorage;

/// <summary>
/// A list of items which is a subset of a larger collection, with information about the place
/// of this subset within the overall collection.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
[JsonConverter(typeof(PagedListConverter))]
[method: JsonConstructor]
public class PagedList<T>(
    IEnumerable<T>? items,
    long pageNumber,
    long pageSize,
    long? totalCount) : IPagedList<T>
{
    /// <summary>
    /// Return the paged query result.
    /// </summary>
    /// <param name="index">Index to fetch item from paged query result.</param>
    /// <returns>Item from paged query result.</returns>
    public T this[int index] => Items is null
        ? throw new IndexOutOfRangeException()
        : Items[index];

    /// <summary>
    /// The number of records in the paged query result.
    /// </summary>
    [JsonIgnore]
    public int Count => Items?.Count ?? 0;

    /// <summary>
    /// The zero-based index of the first item in the current page, within the whole collection.
    /// </summary>
    [JsonIgnore]
    public long FirstIndexOnPage => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Whether there is next page available.
    /// </summary>
    [JsonIgnore]
    public bool HasNextPage => LastIndexOnPage < TotalCount - 1;

    /// <summary>
    /// Whether there is a previous page available.
    /// </summary>
    [JsonIgnore]
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// The current page of items.
    /// </summary>
    public IReadOnlyList<T>? Items { get; } = items?.ToList()?.AsReadOnly();

    /// <summary>
    /// The zero-based index of the last item in the current page, within the whole collection.
    /// </summary>
    [JsonIgnore]
    public long LastIndexOnPage => FirstIndexOnPage + (Count - 1);

    /// <summary>
    /// <para>
    /// The current page number.
    /// </para>
    /// <para>
    /// The first page is 1.
    /// </para>
    /// </summary>
    public long PageNumber { get; } = Math.Max(1, pageNumber);

    /// <summary>
    /// The maximum number of items per page.
    /// </summary>
    public long PageSize { get; } = Math.Max(0, pageSize);

    /// <summary>
    /// The total number of results, of which this page is a subset.
    /// </summary>
    public long? TotalCount { get; } = totalCount;

    /// <summary>
    /// The total number of pages.
    /// </summary>
    [JsonIgnore]
    public long? TotalPages
    {
        get
        {
            if (TotalCount.HasValue)
            {
                return TotalCount.Value % PageSize == 0
                    ? TotalCount.Value / PageSize
                    : (TotalCount.Value / PageSize) + 1;
            }
            return null;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="PagedList{T}"/>.</returns>
    public IEnumerator<T> GetEnumerator() => Items?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
    /// </summary>
    /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="PagedList{T}"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
