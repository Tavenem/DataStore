﻿using System;

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
    /// It also has a built-in, read-only type discriminator property.
    /// </para>
    /// </remarks>
    public interface IIdItem : IEquatable<IIdItem>
    {
        /// <summary>
        /// The ID of this item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// <para>
        /// A built-in, read-only type discriminator.
        /// </para>
        /// <para>
        /// This property has a default implementation in the <see cref="IIdItem"/> interface which
        /// uses reflection to generate a string with the format "IdItemType_{GetType().Name}".
        /// </para>
        /// <para>
        /// The property can (and should) be overridden in implementations to hard-code the
        /// discriminator value, both in order to avoid reflection and also to guard against
        /// potential breaking changes if a type is renamed.
        /// </para>
        /// </summary>
        public string IdItemTypeName => $"IdItemType_{GetType().Name}";
    }
}
