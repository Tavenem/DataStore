using Microsoft.Azure.Cosmos;
using System;

namespace NeverFoundry.DataStorage.Cosmos
{
    /// <summary>
    /// A specialized <see cref="IIdItem"/> item for use with Azure Cosmos DB.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Adds the <see cref="Timestamp"/> property automatically added by Cosmos.
    /// </para>
    /// </remarks>
    public interface ICosmosItem : IIdItem
    {
        /// <summary>
        /// Gets the last modified timestamp associated with the resource from the Azure Cosmos DB service.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}
