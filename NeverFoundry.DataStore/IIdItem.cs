using System;

namespace NeverFoundry.DataStorage
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
    public interface IIdItem : IEquatable<IIdItem>
    {
        /// <summary>
        /// The ID of this item.
        /// </summary>
        string Id { get; }
    }
}
