using System.Threading.Tasks;

namespace NeverFoundry.DataStore
{
    /// <summary>
    /// An item with an ID.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the basic unit of persistence for <see cref="DataStore"/>.
    /// </para>
    /// <para>
    /// It uses a <see cref="string"/> key which may or may not be unique, depending on your
    /// persistence requirements.
    /// </para>
    /// <para>
    /// It provides default, <see langword="virtual"/> implementations for asynchronous Save and
    /// Delete methods which invoke the static DataStore object's own methods to do so.
    /// </para>
    /// </remarks>
    public interface IIdItem
    {
        /// <summary>
        /// The ID of this item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Removes this item from the data store.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the item was successfully deleted; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public virtual Task<bool> DeleteAsync() => DataStore.RemoveItemAsync(Id);

        /// <summary>
        /// Saves this item to the data store.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public virtual Task<bool> SaveAsync() => DataStore.StoreItemAsync(this);
    }
}
