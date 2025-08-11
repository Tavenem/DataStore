# Changelog

## 3.1-preview.10
Version 3 is is a is a major release that includes significant breaking changes to the API, as well as a number of new features and improvements.

The primary objectives of this version were to make `IDataStore` more flexible regarding item and key types, and to enable more comprehensive support for LINQ operations on `IDataStoreQueryable<TSource>` which better match the expected range of features found on a standard `IQueryable<TSource>`.

### Added
- Many new interfaces which indicate native support for LINQ operations on an `IDataStoreQueryable<TSource>` were added. A specific implementation may opt into implementing these interfaces to indicate that it can perform the operation natively in the underlying data source. Those which do not implement an interface signal to the user that the given operations can be performed only client-side (using the built-in `IAsyncEnumerable<TSource>` extension methods). The new interfaces are:
  - `IDataStoreDistinctByQueryable<TSource>`
  - `IDataStoreDistinctQueryable<TSource>`
  - `IDataStoreFirstQueryable<TSource>`
  - `IDataStoreGroupByQueryable<TSource>`
  - `IDataStoreGroupJoinQueryable<TSource>`
  - `IDataStoreIntersectByQueryable<TSource>`
  - `IDataStoreIntersectQueryable<TSource>`
  - `IDataStoreJoinQueryable<TSource>`
  - `IDataStoreLastQueryable<TSource>`
  - `IDataStoreOfTypeQueryable<TSource>`
  - `IDataStoreOrderableQueryable<TSource>` (provides `Order`, `OrderBy`, `OrderByDescending`, and `OrderDescending`)
  - `IDataStoreReverseQueryable<TSource>`
  - `IDataStoreSelectManyQueryable<TSource>`
  - `IDataStoreSelectQueryable<TSource>`
  - `IDataStoreSkipLastQueryable<TSource>`
  - `IDataStoreSkipQueryable<TSource>`
  - `IDataStoreSkipWhileQueryable<TSource>`
  - `IDataStoreTakeLastQueryable<TSource>`
  - `IDataStoreTakeQueryable<TSource>`
  - `IDataStoreTakeWhileQueryable<TSource>`
  - `IDataStoreUnionByQueryable<TSource>`
  - `IDataStoreUnionQueryable<TSource>`
  - `IDataStoreWhereQueryable<TSource>`
  - `IDataStoreZipQueryable<TSource>`
  - It should now be possible for a data store to fully expose its query capabilities via LINQ through these interfaces, while still allowing for client-side LINQ operations on data stores which do not support the full range of LINQ operations natively. Libraries which depend on this one and are unaware of which interfaces might be supported by the data source ultimately selected by an application are expected to follow a pattern of first checking for the presence of an interface to use its method by preference, and only if the interface is not present, falling back on the built-in `IAsyncEnumerable<TSource>` extension methods to perform the operation client-side.
- A `JsonConverter` for `IPagedList<T>` (previously only the non-generic `PagedList<T>` was supported)
- `IOrderedDataStoreQueryable<TSource>` now provides a `ThenByDescending` method

### Changed
- Updated to .NET 10
- The original `IDataStore` interface has been split into four separate interfaces:
  - `IDataStore` which has members common to all data stores
  - `IDataStore<TItem>` for data stores that store items of a specific type
  - `IDataStore<TKey, TItem>` for data stores that store items of a specific type and have a key of a specific type
  - `IIdItemDataStore` which replicates the original by extending `IDataStore<string, IIdItem>`
- `IDataStoreQueryable<TSource>` now implements `IAsyncEnumerable<TSource>` and provides default implementations of a variety of LINQ methods which perform client-side logic on the asynchronously enumerated results. Implementations can override these methods to provide more efficient implementations which execute the logic in the underlying data source.
- All asynchronous methods of `IDataStore` and `IDataStoreQueryable<TSource>` now take a `CancellationToken` parameter
- The original `IIdItem` has been divided into two separate implementations:
  - `IIdItem<TSelf>` which provides a default implementation of `IdItemTypeName` and default equality operators
  - `IIdItem` which now provides the default implementation of `IEquatable<IIdItem>` previously provided by `IdItem`, as well as defining the JSON serialization behavior of its properties
  - This should make it easier to use the opinionated defaults for an `IIdItem` without deriving from the `IdItem` base class
- `IdItemTypeName` (on both `IIdItem` and `IdItem`) now uses the `JsonPropertyName` "_id_t" (both for brevity to minimize object size, and to better indicate its use as a type discriminator rather than an object property).
  - ***N.B. This is a breaking change that may affect the ability to deserialize existing data.*** A thoughtful migration strategy to move from v0-2 to v3 is essential.
- The original `InMemoryDataStore` has been divided into two separate implementations:
  - `InMemoryDataStore<TKey, TItem>` which allows specifying the key type and item type for stored items
  - `InMemoryDataStore` which replicates the original by extending `InMemoryDataStore<string, IIdItem>`
- All properties and methods of the new `InMemoryDataStore` and `InMemoryDataStoreQueryable<TSource>` match the new `IDataStore` and `IDataStoreQueryable<TSource>` interfaces, which result in the same changes as described above

### Removed
- All synchronous methods of `IDataStore` and `IDataStoreQueryable<TSource>`
  - Implementations are free to continue providing synchronous methods if the underlying data source permits synchronous operations, but the interfaces no longer specify them
  - Multiple data sources did not support synchronous operations, and the interface should ideally represent only the common capabilities of most data sources
- The `descending` parameter has been removed from the `IOrderedDataStoreQueryable<TSource>.ThenBy` method, which now always sorts in ascending order to better match the API of `IOrderedQueryable<TSource>` (it does not implement that interface, but the API is deliberately similar).

## 2.0
### Added
- Overloads of certain methods with an additional `JsonTypeInfo<T>` parameter, to support source generated (de)serialization for relevant data sources

## 1.0
### Added
- Initial production release

## 0.39.0-preview
### Changed
- Made `InMemoryDataStore` (de)serializable

## 0.38.1-preview
### Changed
- Remove `JsonConstructor` from protected constructor on abstract `IdItem`

## 0.38.0-preview
### Changed
- `AsPagedList` extension returns `PagedList<T>` instead of `IPagedList<T>`

## 0.37.0-preview
### Changed
- Update to .NET 8 preview
- No longer serializes `IdItemTypeName` as "$type" to avoid conflicts with `System.Text.Json` built-in polymorphic (de)serialization
- Serializes `IdItemTypeName` with its own property name, *after* `id`
- Expose `Items` property in `PagedList`.
### Removed
- `PagedListDTO<T>`

## 0.36.2-preview
### Added
- Add `PagedListDTO<T>` for better (de)serialization support.

## 0.36.1-preview
### Changed
- Clarify 1-based indexing of page numbers in `IPagedList`.

## 0.36.0-preview
### Changed
- Gave `IdItemTypeName` a set accessor (no-op in the default implementation).

## 0.35.0-preview
### Changed
- Made `Id` settable to allow source generated serializers to function.

## 0.34.0-preview
### Changed
- Set `IdItemTypeName` JSON serialization order first, as required by `System.Text.Json`.

## 0.33.0-preview
### Changed
- Add `IdItemTypeName` to `IdItem` base class.

## 0.32.0-preview
### Changed
- Restore `IdItemTypeName` for use with NoSQL providers. Recommended pattern is to use the same value for `JsonDerivedType` and `IdItemTypeName`.

## 0.31.0-preview
### Changed
- Update to .NET 7 preview
- Remove `IdItemTypeName` in favor of built-in polymorphic (de)serialization added to `System.Text.Json`

## 0.30.0-preview
### Changed
- Update to .NET 6 preview
- Update to C# 10 preview

## 0.29.0-preview
### Added
- Initial preview release