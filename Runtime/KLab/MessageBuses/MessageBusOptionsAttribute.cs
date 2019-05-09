// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using System;


namespace KLab.MessageBuses
{
    /// <summary>
    /// Additional options for message bus declaration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MessageBusOptionsAttribute : Attribute
    {
        /// <summary>
        /// Connections container capacity
        /// </summary>
        public readonly int ConnectionsCapacity;

        /// <summary>
        /// Addresses container capacity
        /// </summary>
        public readonly int AddressesCapacity;

        /// <summary>
        /// Message container capacity
        /// </summary>
        public readonly int MessagesCapacity;


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        /// <param name="connectionsCapacity">Connections capacity</param>
        /// <param name="addressesCapacity">Addresses capacity</param>
        /// <param name="messagesCapacity">Messages capacity</param>
        public MessageBusOptionsAttribute(int connectionsCapacity = 4, int addressesCapacity = 8, int messagesCapacity = 4)
        {
            ConnectionsCapacity = connectionsCapacity;
            AddressesCapacity = addressesCapacity;
            MessagesCapacity = messagesCapacity;
        }

        #endregion
    }


    /// <summary>
    /// Helper methods for <see cref="MessageBusOptionsAttribute"/>
    /// </summary>
    internal static class MessageBusOptions
    {
        /// <summary>
        /// Gets options for type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>User options if available; default options otherwise</returns>
        public static MessageBusOptionsAttribute GetForType(Type type)
        {
            var options = type.GetCustomAttributes(typeof(MessageBusOptionsAttribute), false);


            return (options.Length > 0)
                ? (options[0] as MessageBusOptionsAttribute)
                : new MessageBusOptionsAttribute();
        }
    }
}
