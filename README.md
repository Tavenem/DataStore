![build](https://img.shields.io/github/actions/workflow/status/Tavenem/DataStore/publish.yml) [![NuGet downloads](https://img.shields.io/nuget/dt/Tavenem.DataStore)](https://www.nuget.org/packages/Tavenem.DataStore/)

Tavenem.DataStore
==

Tavenem.DataStore is a persistence-agnostic repository library. Its intended purpose is to help author libraries which need to interact with a project's data layer, while remaining fully decoupled from persistence choices.

For example: you might want to author a library which can retrieve an object from the data store by ID, modify it, then update the item in the data store. You want your library to be useful to people who use [EntityFramework](https://docs.microsoft.com/en-us/ef/) to access a SQL database, people who use [Marten](https://martendb.io/) to access a PostgreSQL database, or people who work with [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/).

Tavenem.DataStore provides one possible way to handle this scenario. It provides a simple interface which encapsulates common data operations. As the author of a library, you can accept this interface and use it for all data operations. As a consumer of a library which uses Tavenem.DataStore, you can provide an implementation of this interface designed to work with the particular ORM or data storage SDK you are using in your project.

## Installation

Tavenem.DataStore is available as a [NuGet package](https://www.nuget.org/packages/Tavenem.DataStore/).

In addition there are a handful of complementary NuGet packages which provide implementations of `IDataStore` for various ORMs and data storage SDKs. Currently these include:
- [Tavenem.Blazor.IndexedDB](https://www.nuget.org/packages/Tavenem.Blazor.IndexedDB/) (for [IndexedDB](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API))
- [Tavenem.DataStore.Cosmos](https://www.nuget.org/packages/Tavenem.DataStore.Cosmos/) (for [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/))
- [Tavenem.DataStore.EntityFramework](https://www.nuget.org/packages/Tavenem.DataStore.EntityFramework/) (for [EntityFramework](https://docs.microsoft.com/en-us/ef/))
- [Tavenem.DataStore.Marten](https://www.nuget.org/packages/Tavenem.DataStore.Marten/) (for [Marten](https://martendb.io/))

## Roadmap

Development on Tavenem.DataStore is ongoing. Major releases with breaking changes are possible on an unpredictable cadence.

## Contributing

Contributions are always welcome. Please carefully read the [contributing](docs/CONTRIBUTING.md) document to learn more before submitting issues or pull requests.

## Code of conduct

Please read the [code of conduct](docs/CODE_OF_CONDUCT.md) before engaging with our community, including but not limited to submitting or replying to an issue or pull request.