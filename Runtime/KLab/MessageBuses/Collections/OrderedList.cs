// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using System;
using System.Collections.Generic;


namespace KLab.MessageBuses.Collections
{
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
}
