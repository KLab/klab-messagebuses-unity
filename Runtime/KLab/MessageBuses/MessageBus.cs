// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using KLab.MessageBuses.Broadcasters;
using System;
using System.Collections.Generic;


namespace KLab.MessageBuses
{
    #region Signal Buses

    #region Basic

    /// <summary>
    /// Signal bus
    /// </summary>
    public abstract class MessageBus : IMessageBus
    {
        #region Registry

        /// <summary>
        /// Message bus instances
        /// </summary>
        private static List<IMessageBus> Buses { get; set; }

        /// <summary>
        /// Gets message bus
        /// </summary>
        /// <typeparam name="TMessageBus">Message bus type</typeparam>
        /// <returns>Message bus</returns>
        public static TMessageBus GetBus<TMessageBus>() where TMessageBus : class, IMessageBus, new()
        {
            return (Unsafe.GetBus(typeof(TMessageBus)) as TMessageBus);
        }

        /// <summary>
        /// Gets message bus
        /// </summary>
        /// <typeparam name="TMessageBus">Message bus type</typeparam>
        /// <param name="result">Buffer to hold result</param>
        /// <returns>Message bus</returns>
        public static TMessageBus GetBus<TMessageBus>(ref TMessageBus result) where TMessageBus : class, IMessageBus, new()
        {
            result = GetBus<TMessageBus>();


            return result;
        }


        /// <summary>
        /// Non-typesafe methods
        /// </summary>
        public static class Unsafe
        {
            /// <summary>
            /// Gets bus by type
            /// </summary>
            /// <param name="type">Type of bus</param>
            /// <returns>Instance on success; <see langword="null"/> otherwise</returns>
            public static IMessageBus GetBus(Type type)
            {
                // Validate input
                if (!typeof(IMessageBus).IsAssignableFrom(type))
                {
                    return null;
                }

                if (!type.IsClass || type.IsAbstract)
                {
                    return null;
                }


                // Lazily initialize container
                if (Buses == null)
                {
                    Buses = new List<IMessageBus>();
                }


                // Return instance if found
                foreach (var bus in Buses)
                {
                    if (bus.GetType() == type)
                    {
                        return bus;
                    }
                }


                // Create new instance
                {
                    var bus = Activator.CreateInstance(type) as IMessageBus;


                    if (bus != null)
                    {
                        Buses.Add(bus);


                        return bus;
                    }
                }


                return null;
            }
        }

        #endregion

        /// <summary>
        /// Connections
        /// </summary>
        private readonly Broadcaster Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public MessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new Broadcaster(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Connect(Action handler)
        {
            Broadcaster.Connect(handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        public void Broadcast()
        {
            Broadcaster.Broadcast();
        }
    }


    /// <summary>
    /// Ordered signal bus
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    public abstract class OrderedMessageBus<TOrder> : IMessageBus where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly OrderedBroadcaster<TOrder> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public OrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new OrderedBroadcaster<TOrder>(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TOrder order, Action handler)
        {
            Broadcaster.Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        public void Broadcast()
        {
            Broadcaster.Broadcast();
        }
    }

    #endregion


    #region Deferred

    /// <summary>
    /// Deferred signal bus
    /// </summary>
    public abstract class DeferredMessageBus : IMessageBus
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly DeferredBroadcaster Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public DeferredMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new DeferredBroadcaster(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Connect(Action handler)
        {
            Broadcaster.Connect(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        public void Broadcast()
        {
            Broadcaster.Broadcast();
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch()
        {
            Broadcaster.Dispatch();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Broadcaster.WaiveDispatch();
        }
    }


    /// <summary>
    /// Ordered deferred signal broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    public abstract class DeferredOrderedMessageBus<TOrder> : IMessageBus where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly DeferredOrderedBroadcaster<TOrder> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public DeferredOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new DeferredOrderedBroadcaster<TOrder>(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TOrder order, Action handler)
        {
            Broadcaster.Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        public void Broadcast()
        {
            Broadcaster.Broadcast();
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch()
        {
            Broadcaster.Dispatch();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Broadcaster.WaiveDispatch();
        }
    }

    #endregion


    #region Addressable

    /// <summary>
    /// Addressable signal bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    public abstract class AddressableMessageBus<TAddress> : IMessageBus where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableBroadcaster<TAddress> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableBroadcaster<TAddress>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, Action handler)
        {
            Broadcaster.Connect(address, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            Broadcaster.Broadcast(address);
        }
    }


    /// <summary>
    /// Ordered addressable signal bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    public abstract class AddressableOrderedMessageBus<TAddress, TOrder> : IMessageBus
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableOrderedBroadcaster<TAddress, TOrder> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableOrderedBroadcaster<TAddress, TOrder>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, TOrder order, Action handler)
        {
            Broadcaster.Connect(address, order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            Broadcaster.Broadcast(address);
        }
    }


    /// <summary>
    /// Deferred addressable signal bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    public abstract class AddressableDeferredMessageBus<TAddress> : IMessageBus where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly AddressableDeferredBroadcaster<TAddress> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableDeferredMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableDeferredBroadcaster<TAddress>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, Action handler)
        {
            Broadcaster.Connect(address, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Order</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        public void Broadcast(TAddress address)
        {
            Broadcaster.Broadcast(address);
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch(TAddress address)
        {
            Broadcaster.Dispatch(address);
        }

        /// <summary>
        /// Signals all
        /// </summary>
        public void DispatchAll()
        {
            Broadcaster.DispatchAll();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        /// <param name="address"></param>
        public void WaiveDispatch(TAddress address)
        {
            Broadcaster.WaiveDispatch(address);
        }

        /// <summary>
        /// Cancels all dispatches
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcaster.WaiveDispatchAll();
        }
    }


    /// <summary>
    /// Ordered deferred addressable signal bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    public abstract class AddressableDeferredOrderedMessageBus<TAddress, TOrder> : IMessageBus
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableDeferredOrderedBroadcaster<TAddress, TOrder> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableDeferredOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableDeferredOrderedBroadcaster<TAddress, TOrder>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, TOrder order, Action handler)
        {
            Broadcaster.Connect(address, order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            Broadcaster.Broadcast(address);
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch(TAddress address)
        {
            Broadcaster.Dispatch(address);
        }

        /// <summary>
        /// Signals all
        /// </summary>
        public void DispatchAll()
        {
            Broadcaster.DispatchAll();
        }

    
        /// <summary>
        /// Cancels dispatch
        /// </summary>
        /// <param name="address"></param>
        public void WaiveDispatch(TAddress address)
        {
            Broadcaster.WaiveDispatch(address);
        }

        /// <summary>
        /// Cancels all dispatches
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcaster.WaiveDispatchAll();
        }
    }

    #endregion

    #endregion


    #region Message Buses

    #region Basic

    /// <summary>
    /// Message bus
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class MessageBus<TMessage> : IMessageBus
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly Broadcaster<TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public MessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new Broadcaster<TMessage>(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Connect(Action<TMessage> handler)
        {
            Broadcaster.Connect(handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action<TMessage> handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="message">Message</param>
        public void Broadcast(TMessage message)
        {
            Broadcaster.Broadcast(message);
        }
    }


    /// <summary>
    /// Ordered message bus
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class OrderedMessageBus<TOrder, TMessage> : IMessageBus where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly OrderedBroadcaster<TOrder, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public OrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new OrderedBroadcaster<TOrder, TMessage>(options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TOrder order, Action<TMessage> handler)
        {
            Broadcaster.Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action<TMessage> handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="message">Message</param>
        public void Broadcast(TMessage message)
        {
            Broadcaster.Broadcast(message);
        }
    }

    #endregion


    #region Deferred

    /// <summary>
    /// Deferred message bus
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class DeferredMessageBus<TMessage> : IMessageBus
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly DeferredBroadcaster<TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public DeferredMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new DeferredBroadcaster<TMessage>(options.ConnectionsCapacity, options.MessagesCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Connect(Action<TMessage> handler)
        {
            Broadcaster.Connect(handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action<TMessage> handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Enqueues message
        /// </summary>
        public void Broadcast(TMessage message)
        {
            Broadcaster.Broadcast(message);
        }

        /// <summary>
        /// Messages
        /// </summary>
        public void Dispatch()
        {
            Broadcaster.Dispatch();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Broadcaster.WaiveDispatch();
        }
    }


    /// <summary>
    /// Ordered deferred message broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class DeferredOrderedMessageBus<TOrder, TMessage> : IMessageBus where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly DeferredOrderedBroadcaster<TOrder, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public DeferredOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new DeferredOrderedBroadcaster<TOrder, TMessage>(options.ConnectionsCapacity, options.MessagesCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TOrder order, Action<TMessage> handler)
        {
            Broadcaster.Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="handler">Handler</param>
        public void Disconnect(Action<TMessage> handler)
        {
            Broadcaster.Disconnect(handler);
        }


        /// <summary>
        /// Enqueues message
        /// </summary>
        /// <param name="message">Message</param>
        public void Broadcast(TMessage message)
        {
            Broadcaster.Broadcast(message);
        }

        /// <summary>
        /// Messages
        /// </summary>
        public void Dispatch()
        {
            Broadcaster.Dispatch();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Broadcaster.WaiveDispatch();
        }
    }

    #endregion


    #region Addressable

    /// <summary>
    /// Addressable message bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class AddressableMessageBus<TAddress, TMessage> : IMessageBus where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableBroadcaster<TAddress, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableBroadcaster<TAddress, TMessage>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Connect(address, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            Broadcaster.Broadcast(address, message);
        }
    }


    /// <summary>
    /// Ordered addressable message bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class AddressableOrderedMessageBus<TAddress, TOrder, TMessage> : IMessageBus
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableOrderedBroadcaster<TAddress, TOrder, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableOrderedBroadcaster<TAddress, TOrder, TMessage>(options.AddressesCapacity, options.ConnectionsCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, TOrder order, Action<TMessage> handler)
        {
            Broadcaster.Connect(address, order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            Broadcaster.Broadcast(address, message);
        }
    }


    /// <summary>
    /// Deferred addressable message bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class AddressableDeferredMessageBus<TAddress, TMessage> : IMessageBus where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly AddressableDeferredBroadcaster<TAddress, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableDeferredMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableDeferredBroadcaster<TAddress, TMessage>(options.AddressesCapacity, options.ConnectionsCapacity, options.MessagesCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Connect(address, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Order</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Enqueues message
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            Broadcaster.Broadcast(address, message);
        }

        /// <summary>
        /// Messages
        /// </summary>
        public void Dispatch(TAddress address)
        {
            Broadcaster.Dispatch(address);
        }

        /// <summary>
        /// Messages all
        /// </summary>
        public void DispatchAll()
        {
            Broadcaster.DispatchAll();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        /// <param name="address"></param>
        public void WaiveDispatch(TAddress address)
        {
            Broadcaster.WaiveDispatch(address);
        }

        /// <summary>
        /// Cancels all dispatches
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcaster.WaiveDispatchAll();
        }
    }


    /// <summary>
    /// Ordered deferred addressable message bus
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    public abstract class AddressableDeferredOrderedMessageBus<TAddress, TOrder, TMessage> : IMessageBus
        where TAddress : IEquatable<TAddress>
        where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcaster
        /// </summary>
        private readonly AddressableDeferredOrderedBroadcaster<TAddress, TOrder, TMessage> Broadcaster;

        /// <summary>
        /// Flag whether bus has any connections
        /// </summary>
        public bool AnyConnection
        {
            get { return Broadcaster.AnyConnection; }
        }


        #region Ctors

        /// <summary>
        /// Initializes instance
        /// </summary>
        public AddressableDeferredOrderedMessageBus()
        {
            var options = MessageBusOptions.GetForType(GetType());
            Broadcaster = new AddressableDeferredOrderedBroadcaster<TAddress, TOrder, TMessage>(options.AddressesCapacity, options.ConnectionsCapacity, options.MessagesCapacity);
        }

        #endregion


        /// <summary>
        /// Connects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler</param>
        public void Connect(TAddress address, TOrder order, Action<TMessage> handler)
        {
            Broadcaster.Connect(address, order, handler);
        }

        /// <summary>
        /// Disconnects handler
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            Broadcaster.Disconnect(address, handler);
        }


        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            Broadcaster.Broadcast(address, message);
        }

        /// <summary>
        /// Messages
        /// </summary>
        public void Dispatch(TAddress address)
        {
            Broadcaster.Dispatch(address);
        }

        /// <summary>
        /// Messages all
        /// </summary>
        public void DispatchAll()
        {
            Broadcaster.DispatchAll();
        }


        /// <summary>
        /// Cancels dispatch
        /// </summary>
        /// <param name="address"></param>
        public void WaiveDispatch(TAddress address)
        {
            Broadcaster.WaiveDispatch(address);
        }

        /// <summary>
        /// Cancels all dispatches
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcaster.WaiveDispatchAll();
        }
    }

    #endregion

    #endregion
}
