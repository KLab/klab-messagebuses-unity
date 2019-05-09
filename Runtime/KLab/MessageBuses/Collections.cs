// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using System;
using System.Collections.Generic;


namespace KLab.MessageBuses.Collections
{
    #region Ordered List

    /// <summary>
    /// Ordered <see cref="List"/>
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TItem">Item type</typeparam>
    internal sealed class OrderedList<TOrder, TItem> where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Elements
        /// </summary>
        private readonly List<ElementsEntry> Elements;


        /// <summary>
        /// Element count
        /// </summary>
        public int Count
        {
            get { return Elements.Count; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">Index to access</param>
        /// <returns>Item at index</returns>
        public TItem this[int index]
        {
            get { return Elements[index].Item; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        /// <param name="initialCapacity">Capacity</param>
        public OrderedList(int initialCapacity)
        {
            Elements = new List<ElementsEntry>(initialCapacity);
        }

        #endregion


        /// <summary>
        /// Adds item
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="item">Item</param>
        public void Add(TOrder order, TItem item)
        {
            Elements.Add(new ElementsEntry
            {
                Order = order,
                Item = item
            });


            Elements.Sort(ElementsEntry.Compare);
        }

        /// <summary>
        /// Removes item
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(TItem item)
        {
            var itemIndex = IndexOf(item);


            // Silently fail
            if (itemIndex == -1) { return; }


            // Remove element by index
            Elements.RemoveAt(itemIndex);
        }


        /// <summary>
        /// Checks whether item is contained in list
        /// </summary>
        /// <param name="item">Item to query for</param>
        /// <returns><see langword="true"/> if contained; <see langword="false"/> otherwise</returns>
        public bool Contains(TItem item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Gets index of item
        /// </summary>
        /// <param name="item">Item to query for</param>
        /// <returns>Valid index if found; -1 otherwise</returns>
        private int IndexOf(TItem item)
        {
            var elements = Elements;
            var elementsLength = Elements.Count;
            var itemIndex = -1;


            // Find item
            for (var i = 0; i < elementsLength; ++i)
            {
                if (!elements[i].Item.Equals(item)) { continue; }


                itemIndex = i;


                break;
            }


            return itemIndex;
        }

        #region ElementEntry

        /// <summary>
        /// Order-action pair
        /// </summary>
        private struct ElementsEntry
        {
            /// <summary>
            /// Compares items
            /// </summary>
            /// <param name="lhs">Left-hand value</param>
            /// <param name="rhs">Right-hand value</param>
            /// <returns>Comparison order</returns>
            public static int Compare(ElementsEntry lhs, ElementsEntry rhs)
            {
                return lhs.Order.CompareTo(rhs.Order);
            }


            /// <summary>
            /// Order
            /// </summary>
            public TOrder Order;

            /// <summary>
            /// Handler
            /// </summary>
            public TItem Item;
        }

        #endregion
    }

    #endregion


    #region Addressable List

    /// <summary>
    /// <see cref="List"/> with addressable items
    /// </summary>
    /// <typeparam name="TAddress">Order type</typeparam>
    /// <typeparam name="TItem">Item type</typeparam>
    internal sealed class AddressableList<TAddress, TItem> where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Items
        /// </summary>
        private readonly List<ElementsEntry> Elements;


        /// <summary>
        /// Item count
        /// </summary>
        public int Count
        {
            get { return Elements.Count; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">Address to access</param>
        /// <returns>Item at address</returns>
        public TItem this[TAddress address]
        {
            get
            {
                for (var i = 0; i < Elements.Count; ++i)
                {
                    if (Elements[i].Address.Equals(address))
                    {
                        return Elements[i].Item;
                    }
                }

                
                return default(TItem);
            }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        /// <param name="initialCapacity">Capacity</param>
        public AddressableList(int initialCapacity)
        {
            Elements = new List<ElementsEntry>(initialCapacity);
        }

        #endregion


        /// <summary>
        /// Adds item
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="item">Item</param>
        public void Add(TAddress address, TItem item)
        {
            var index = IndexOf(address);

            if (index != -1)
            {
                Elements[index] = new ElementsEntry
                {
                    Address = address,
                    Item = item
                };


                return;
            }


            Elements.Add(new ElementsEntry
            {
                Address = address,
                Item = item
            });
        }

        /// <summary>
        /// Removes item
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(TItem item)
        {
            var index = IndexOf(item);


            // Silently fail
            if (index == -1) { return; }


            // Remove element by index
            Elements.RemoveAt(index);
        }


        /// <summary>
        /// Checks whether address is contained in list
        /// </summary>
        /// <param name="address">Address to query for</param>
        /// <returns><see langword="true"/> if contained; <see langword="false"/> otherwise</returns>
        public bool Contains(TAddress address)
        {
            return IndexOf(address) != -1;
        }

        /// <summary>
        /// Checks whether item is contained in list
        /// </summary>
        /// <param name="item">Item to query for</param>
        /// <returns><see langword="true"/> if contained; <see langword="false"/> otherwise</returns>
        public bool Contains(TItem item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Gets index of address
        /// </summary>
        /// <param name="address">Address to query for</param>
        /// <returns>Valid index if found; -1 otherwise</returns>
        private int IndexOf(TAddress address)
        {
            var elements = Elements;
            var count = Elements.Count;
            var index = -1;


            // Find address
            for (var i = 0; i < count; ++i)
            {
                if (!elements[i].Address.Equals(address)) { continue; }


                index = i;


                break;
            }


            return index;
        }

        /// <summary>
        /// Gets index of item
        /// </summary>
        /// <param name="item">Item to query for</param>
        /// <returns>Valid index if found; -1 otherwise</returns>
        private int IndexOf(TItem item)
        {
            var elements = Elements;
            var count = Elements.Count;
            var index = -1;


            // Find item
            for (var i = 0; i < count; ++i)
            {
                if (!elements[i].Item.Equals(item)) { continue; }


                index = i;


                break;
            }


            return index;
        }


        /// <summary>
        /// Performs action on each item
        /// </summary>
        /// <param name="action">Action to perform</param>
        public void ForEach(Action<TItem> action)
        {
            // Fail silently
            if (action == null)
            {
                return;
            }


            var elements = Elements;
            var count = Elements.Count;


            for (var i = 0; i < count; ++i)
            {
                action(elements[i].Item);
            }
        }

        #region ElementsEntry

        /// <summary>
        /// Order-action pair
        /// </summary>
        private struct ElementsEntry
        {
            /// <summary>
            /// Address
            /// </summary>
            public TAddress Address;

            /// <summary>
            /// Item
            /// </summary>
            public TItem Item;
        }

        #endregion
    }

    #endregion
}
