![build](https://img.shields.io/github/workflow/status/Tavenem/DataStore/publish/main) [![NuGet downloads](https://img.shields.io/nuget/dt/Tavenem.DataStore)](https://www.nuget.org/packages/Tavenem.DataStore/)

Tavenem.DataStore
==

Tavenem.DataStore is a persistence-agnostic repository library. Its intended purpose is to help author libraries which need to interact with a project's data layer, while remaining fully decoupled from persistence choices.

For example: you might want to author a library which can retrieve an object from the data store by ID, modify it, then update the item in the data store. You want your library to be useful to people who use [EntityFramework](https://docs.microsoft.com/en-us/ef/) to access a SQL database, people who use [Marten](https://martendb.io/) to access a PostgreSQL database, or people who work with [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/).

One possible solution might be to work with generic interfaces like [IQueryable](https://docs.microsoft.com/en-us/dotnet/api/system.linq.iqueryable), and provide event hooks so that implementers of your library are responsible for data retrieval and storage.

Tavenem.DataStore provides another possible way to handle this scenario. It provides a simple interface which encapsulates common data operations. As the author of a library, you can accept this interface and use it for all data operations. As a consumer of a library which uses Tavenem.DataStore, you can provide an implementation of this interface designed to work with the particular ORM or data storage SDK you are using in your project.

## Installation

Tavenem.DataStore is available as a [NuGet package](https://www.nuget.org/packages/Tavenem.DataStore/).

In addition there are a handful of complementary NuGet packages which provide implementations of `IDataStore` for various ORMs and data storage SDKs. Currently these include:
- [Tavenem.DataStore.Cosmos](https://www.nuget.org/packages/Tavenem.DataStore.Cosmos/) (for [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/))
- [Tavenem.DataStore.EntityFramework](https://www.nuget.org/packages/Tavenem.DataStore.EntityFramework/) (for [EntityFramework](https://docs.microsoft.com/en-us/ef/))
- [Tavenem.DataStore.Marten](https://www.nuget.org/packages/Tavenem.DataStore.Marten/) (for [Marten](https://martendb.io/))

## Roadmap

Tavenem.DataStore is currently in a **prerelease** state. Development is ongoing, and breaking changes are possible before the first production release.

No release date is currently set for v1.0 of Tavenem.DataStore. The project is currently in a "wait and see" phase while its utility, completeness, and correctness are evaluated. When the project is judged to be in a satisfactory state, and further breaking changes to the interface are determined to be unlikely, a production release will be made.

## Contributing

Contributions are always welcome. Please carefully read the [contributing](docs/CONTRIBUTING.md) document to learn more before submitting issues or pull requests.

## Code of conduct

Please read the [code of conduct](docs/CODE_OF_CONDUCT.md) before engaging with our community, including but not limited to submitting or replying to an issue or pull request.