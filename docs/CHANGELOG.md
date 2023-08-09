# Changelog

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