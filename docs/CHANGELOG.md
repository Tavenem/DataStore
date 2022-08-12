# Changelog

## 0.35.0-preview
### Changed
- Made `Id` settable to allow source generated serializers to function.

## 0.34.0-preview
### Changed
- Set `IdItemTypeName` JSON serialization order first, as required by System.Text.Json.

## 0.33.0-preview
### Changed
- Add `IdItemTypeName` to `IdItem` base class.

## 0.32.0-preview
### Changed
- Restore `IdItemTypeName` for use with NoSQL providers. Recommended pattern is to use the same value for `JsonDerivedType` and `IdItemTypeName`.

## 0.31.0-preview
### Changed
- Update to .NET 7 preview
- Remove `IdItemTypeName` in favor of built-in polymorphic (de)serialization added to System.Text.Json

## 0.30.0-preview
### Changed
- Update to .NET 6 preview
- Update to C# 10 preview

## 0.29.0-preview
### Added
- Initial preview release