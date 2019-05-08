// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using KLab.MessageBuses.Collections;
using System;
using System.Collections.Generic;


namespace KLab.MessageBuses.Broadcasters
{
    #region Signal Broadcasters

    /// <summary>
    /// Signal broadcaster
    /// </summary>
    internal sealed class Broadcaster
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly List<Action> Connections = new List<Action>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }


            Connections.Add(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }


            Connections.Remove(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        public void Broadcast()
        {
            var connections = Connections;
            var count = Connections.Count;


            // Signal
            for (var c = 0; c < count; ++c)
            {
                connections[c]();
            }
        }
    }


    /// <summary>
    /// Ordered signal broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    internal sealed class OrderedBroadcaster<TOrder> where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly OrderedList<TOrder, Action> Connections = new OrderedList<TOrder, Action>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TOrder order, Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }


            Connections.Add(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }


            Connections.Remove(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        public void Broadcast()
        {
            var connections = Connections;
            var count = Connections.Count;


            // Signal
            for (var c = 0; c < count; ++c)
            {
                connections[c]();
            }
        }
    }

    #region Deferred

    /// <summary>
    /// Deferred signal broadcaster
    /// </summary>
    internal sealed class DeferredBroadcaster
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly List<Action> Connections = new List<Action>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Control whether signal should be dispatched
        /// </summary>
        private bool ShouldSignal { get; set; }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }

            Connections.Add(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            Connections.Remove(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        public void Broadcast()
        {
            ShouldSignal = true;
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch()
        {
            // Early out unless should signal
            if (!ShouldSignal)
            {
                return;
            }


            var connections = Connections;
            var connectionsCount = Connections.Count;


            // Signal
            for (var c = 0; c < connectionsCount; ++c)
            {
                connections[c]();
            }


            // Reset flag
            ShouldSignal = false;
        }
    

        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            ShouldSignal = false;
        }
    }


    /// <summary>
    /// Ordered deferred signal broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    internal sealed class DeferredOrderedBroadcaster<TOrder> where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly OrderedList<TOrder, Action> Connections = new OrderedList<TOrder, Action>();

        /// <summary>
        /// Control whether signal should be dispatched
        /// </summary>
        private bool ShouldSignal { get; set; }

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TOrder order, Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }


            Connections.Add(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }


            Connections.Remove(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        public void Broadcast()
        {
            ShouldSignal = true;
        }

        /// <summary>
        /// Signals
        /// </summary>
        public void Dispatch()
        {
            // Early out unless should signal
            if (!ShouldSignal)
            {
                return;
            }


            var connections = Connections;
            var connectionsCount = Connections.Count;


            // Signal
            for (var c = 0; c < connectionsCount; ++c)
            {
                connections[c]();
            }


            // Reset flag
            ShouldSignal = false;
        }


        /// <summary>
        /// Prevents disptach
        /// </summary>
        public void WaiveDispatch()
        {
            ShouldSignal = false;
        }
    }

    #endregion

    #region Addressable

    /// <summary>
    /// Addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    internal sealed class AddressableBroadcaster<TAddress> where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, Broadcaster> Broadcasters = new AddressableList<TAddress, Broadcaster>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public Broadcaster this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new Broadcaster();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, Action handler)
        {
            this[address].Connect(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            this[address].Broadcast();
        }
    }


    /// <summary>
    /// Ordered, immediate signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    internal sealed class AddressableOrderedBroadcaster<TAddress, TOrder>
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, OrderedBroadcaster<TOrder>> Broadcasters = new AddressableList<TAddress, OrderedBroadcaster<TOrder>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public OrderedBroadcaster<TOrder> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new OrderedBroadcaster<TOrder>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, TOrder order, Action handler)
        {
            this[address].Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            this[address].Broadcast();
        }
    }


    /// <summary>
    /// Deferred addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    internal sealed class AddressableDeferredBroadcaster<TAddress> where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, DeferredBroadcaster> Broadcasters = new AddressableList<TAddress, DeferredBroadcaster>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public DeferredBroadcaster this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new DeferredBroadcaster();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, Action handler)
        {
            this[address].Connect(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            this[address].Broadcast();
        }

        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Dispatch(TAddress address)
        {
            this[address].Dispatch();
        }

        /// <summary>
        /// Signals all
        /// </summary>
        public void DispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.Dispatch();
            });
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch(TAddress address)
        {
            this[address].WaiveDispatch();
        }

        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.WaiveDispatch();
            });
        }
    }


    /// <summary>
    /// Ordered deferred addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    internal sealed class AddressableDeferredOrderedBroadcaster<TAddress, TOrder>
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, DeferredOrderedBroadcaster<TOrder>> Broadcasters = new AddressableList<TAddress, DeferredOrderedBroadcaster<TOrder>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public DeferredOrderedBroadcaster<TOrder> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new DeferredOrderedBroadcaster<TOrder>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, TOrder order, Action handler)
        {
            this[address].Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Enqueues signal
        /// </summary>
        /// <param name="address">Address</param>
        public void Broadcast(TAddress address)
        {
            this[address].Broadcast();
        }

        /// <summary>
        /// Signals
        /// </summary>
        /// <param name="address">Address</param>
        public void Dispatch(TAddress address)
        {
            this[address].Dispatch();
        }

        /// <summary>
        /// Signals all
        /// </summary>
        public void DispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.Dispatch();
            });
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch(TAddress address)
        {
            this[address].WaiveDispatch();
        }

        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.WaiveDispatch();
            });
        }
    }

    #endregion

    #endregion

    #region Message Broadcasters

    /// <summary>
    /// Immediate, unordered message broadcaster
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class Broadcaster<TMessage>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly List<Action<TMessage>> Connections = new List<Action<TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }


            Connections.Add(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }


            Connections.Remove(handler);
        }


        /// <summary>
        /// Broadcasts message
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TMessage message)
        {
            var connections = Connections;
            var count = Connections.Count;

            // Invoke all handlers
            for (var c = 0; c < count; ++c) { connections[c](message); }
        }
    }

    /// <summary>
    /// Ordered, immediate message broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class OrderedBroadcaster<TOrder, TMessage> where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly OrderedList<TOrder, Action<TMessage>> Connections = new OrderedList<TOrder, Action<TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TOrder order, Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }


            Connections.Add(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            Connections.Remove(handler);
        }


        /// <summary>
        /// Broadcasts message
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TMessage message)
        {
            var connections = Connections;
            var count = Connections.Count;

            // Invoke all handlers
            for (var c = 0; c < count; ++c)
            {
                connections[c](message);
            }
        }
    }

    #region Deferred

    /// <summary>
    /// Unordered, deferred message broadcaster
    /// </summary>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class DeferredBroadcaster<TMessage>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly List<Action<TMessage>> Connections = new List<Action<TMessage>>();

        /// <summary>
        /// Message queue
        /// </summary>
        private readonly List<TMessage> Messages = new List<TMessage>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="handler">Handler to connect</param>
        public void Connect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }

            Connections.Add(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            Connections.Remove(handler);
        }


        /// <summary>
        /// Enqueues message for broadcast
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TMessage message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Broadcasts messages
        /// </summary>
        public void Dispatch()
        {
            var messages      = Messages;
            var messagesCount = messages.Count;

            var connections      = Connections;
            var connectionsCount = Connections.Count;


            // Send all messages on all handlers
            for (var m = 0; m < messagesCount; ++m)
            {
                for (var c = 0; c < connectionsCount; ++c)
                {
                    connections[c](messages[m]);
                }
            }


            // Empty queue
            messages.Clear();
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Messages.Clear();
        }
    }


    /// <summary>
    /// Deferred ordered message broadcaster
    /// </summary>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class DeferredOrderedBroadcaster<TOrder, TMessage> where TOrder : IComparable<TOrder>
    {
        /// <summary>
        /// Connections
        /// </summary>
        private readonly OrderedList<TOrder, Action<TMessage>> Connections = new OrderedList<TOrder, Action<TMessage>>();

        /// <summary>
        /// Message queue
        /// </summary>
        private readonly List<TMessage> Messages = new List<TMessage>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get { return (Connections.Count > 0); }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="order">Order of handler</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TOrder order, Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            // Early out if handler already connected
            if (Connections.Contains(handler)) { return; }

            Connections.Add(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(Action<TMessage> handler)
        {
            // Early out if handler invailed
            if (handler == null) { return; }

            Connections.Remove(handler);
        }


        /// <summary>
        /// Enqueues message for broadcast
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TMessage message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Broadcasts
        /// </summary>
        public void Dispatch()
        {
            var messages = Messages;
            var messagesCount = messages.Count;

            var connections = Connections;
            var connectionsCount = Connections.Count;


            // Send all messages on all handlers
            for (var m = 0; m < messagesCount; ++m)
            {
                for (var c = 0; c < connectionsCount; ++c)
                {
                    connections[c](messages[m]);
                }
            }


            // Empty queue
            messages.Clear();
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch()
        {
            Messages.Clear();
        }
    }

    #endregion

    #region Addressable

    /// <summary>
    /// Addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class AddressableBroadcaster<TAddress, TMessage> where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, Broadcaster<TMessage>> Broadcasters = new AddressableList<TAddress, Broadcaster<TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public Broadcaster<TMessage> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new Broadcaster<TMessage>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, Action<TMessage> handler)
        {
            this[address].Connect(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Broadcasts
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            this[address].Broadcast(message);
        }
    }


    /// <summary>
    /// Ordered, immediate signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class AddressableOrderedBroadcaster<TAddress, TOrder, TMessage>
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, OrderedBroadcaster<TOrder, TMessage>> Broadcasters = new AddressableList<TAddress, OrderedBroadcaster<TOrder, TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public OrderedBroadcaster<TOrder, TMessage> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new OrderedBroadcaster<TOrder, TMessage>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, TOrder order, Action<TMessage> handler)
        {
            this[address].Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Broadcasts
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            this[address].Broadcast(message);
        }
    }


    /// <summary>
    /// Deferred addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class AddressableDeferredBroadcaster<TAddress, TMessage> where TAddress : IEquatable<TAddress>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, DeferredBroadcaster<TMessage>> Broadcasters = new AddressableList<TAddress, DeferredBroadcaster<TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public DeferredBroadcaster<TMessage> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new DeferredBroadcaster<TMessage>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, Action<TMessage> handler)
        {
            this[address].Connect(handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Enqueues message
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            this[address].Broadcast(message);
        }

        /// <summary>
        /// Broadcasts
        /// </summary>
        /// <param name="address">Address</param>
        public void Dispatch(TAddress address)
        {
            this[address].Dispatch();
        }

        /// <summary>
        /// Broaddcasts all
        /// </summary>
        public void DispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.Dispatch();
            });
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch(TAddress address)
        {
            this[address].WaiveDispatch();
        }

        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.WaiveDispatch();
            });
        }
    }


    /// <summary>
    /// Ordered deferred addressable signal broadcaster
    /// </summary>
    /// <typeparam name="TAddress">Address type</typeparam>
    /// <typeparam name="TOrder">Order type</typeparam>
    /// <typeparam name="TMessage">Message type</typeparam>
    internal sealed class AddressableDeferredOrderedBroadcaster<TAddress, TOrder, TMessage>
        where TAddress : IEquatable<TAddress>
        where TOrder   : IComparable<TOrder>
    {
        /// <summary>
        /// Broadcasters
        /// </summary>
        private readonly AddressableList<TAddress, DeferredOrderedBroadcaster<TOrder, TMessage>> Broadcasters = new AddressableList<TAddress, DeferredOrderedBroadcaster<TOrder, TMessage>>();

        /// <summary>
        /// Flag whether one or more connections exist
        /// </summary>
        public bool AnyConnection
        {
            get
            {
                var anyConnection = false;


                Broadcasters.ForEach((broadcaster) =>
                {
                    if (broadcaster.AnyConnection)
                    {
                        anyConnection = true;
                    }
                });


                return anyConnection;
            }
        }


        /// <summary>
        /// Gets broadcaster by address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Broadcaster</returns>
        public DeferredOrderedBroadcaster<TOrder, TMessage> this[TAddress address]
        {
            get
            {
                var broadcaster = Broadcasters[address];


                if (broadcaster == null)
                {
                    broadcaster = new DeferredOrderedBroadcaster<TOrder, TMessage>();


                    Broadcasters.Add(address, broadcaster);
                }


                return broadcaster;
            }
        }


        /// <summary>
        /// Connects handler if unique (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="order">Order</param>
        /// <param name="handler">Handler to connect</param>
        public void Connect(TAddress address, TOrder order, Action<TMessage> handler)
        {
            this[address].Connect(order, handler);
        }

        /// <summary>
        /// Disconnects handler (failing silently)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="handler">Handler to disconnect</param>
        public void Disconnect(TAddress address, Action<TMessage> handler)
        {
            this[address].Disconnect(handler);
        }


        /// <summary>
        /// Enqueues message
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(TAddress address, TMessage message)
        {
            this[address].Broadcast(message);
        }

        /// <summary>
        /// Broadcasts
        /// </summary>
        /// <param name="address">Address</param>
        public void Dispatch(TAddress address)
        {
            this[address].Dispatch();
        }

        /// <summary>
        /// Broaddcasts all
        /// </summary>
        public void DispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.Dispatch();
            });
        }


        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatch(TAddress address)
        {
            this[address].WaiveDispatch();
        }

        /// <summary>
        /// Prevents dispatch
        /// </summary>
        public void WaiveDispatchAll()
        {
            Broadcasters.ForEach(b =>
            {
                b.WaiveDispatch();
            });
        }
    }

    #endregion

    #endregion
}
