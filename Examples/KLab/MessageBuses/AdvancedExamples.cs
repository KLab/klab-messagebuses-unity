// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace KLab.MessageBuses
{
    /// <summary>
    /// Advanced message bus usage exsamples
    /// </summary>
    internal sealed class AdvancedExamples
    {
        #region FindAllAnyConnectionProperties

        /// <summary>
        /// Signature of connection query properties
        /// </summary>
        /// <returns>True if any connection; false otherwise</returns>
        private delegate bool AnyConnectionDelegate();


        /// <summary>
        /// Finds all AnyConnection properties via relfection
        /// </summary>
        [Test]
        public void FindAllAnyConnectionProperties()
        {
            // Find waivable types
            var anyConnectionMethods = new List<AnyConnectionDelegate>();


            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .ToList()
                .ForEach(type =>
                {
                    var isBus = typeof(IMessageBus).IsAssignableFrom(type)
                        && type.IsClass
                        && !type.IsAbstract
                        && (type.BaseType != null);


                    if (isBus)
                    {
                        // Try get property
                        var propertyInfo = type.GetProperty("AnyConnection");


                        if (propertyInfo != null)
                        {
                            // Get bus
                            var bus = MessageBus.Unsafe.GetBus(type);


                            // Get method
                            var method = Delegate.CreateDelegate(typeof(AnyConnectionDelegate), bus, propertyInfo.GetGetMethod()) as AnyConnectionDelegate;


                            // Make sure delegate was created
                            Assert.IsNotNull(method, "Failed to create 'AnyProperty' wrapper delegate");


                            // Store delegate
                            anyConnectionMethods.Add(method);
                        }
                    }
                });


            // Log results
            Assert.GreaterOrEqual(anyConnectionMethods.Count, 2, "Expected to find at least 2 methods");
        }

        #endregion

        #region FindAllWaiveMethods

        /// <summary>
        /// Signature of waive methods
        /// </summary>
        private delegate void WaiveDelegate();


        /// <summary>
        /// Message bus with <see cref="DeferredMessageBus.WaiveDispatch"/> method
        /// </summary>
        private sealed class Waivable : DeferredMessageBus {};

        /// <summary>
        /// Message bus with <see cref="AddressableDeferredMessageBus{}.WaiveDispatchAll"/> method
        /// </summary>
        private sealed class WaivableAll : AddressableDeferredMessageBus<int> {};


        /// <summary>
        /// Finds all Waive() methods via relfection
        /// </summary>
        [Test]
        public void FindAllWaiveMethods()
        {
            // Find waivable types
            var waiveMethods = new List<WaiveDelegate>();


            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .ToList()
                .ForEach(type =>
                {
                    var isBus = typeof(IMessageBus).IsAssignableFrom(type)
                        && type.IsClass
                        && !type.IsAbstract
                        && (type.BaseType != null);


                    if (isBus)
                    {
                        // Try get waive all method first
                        var methodInfo = type.GetMethod("WaiveDispatchAll");


                        // Try get waive next
                        if (methodInfo == null)
                        {
                            methodInfo = type.GetMethod("WaiveDispatch");
                        }


                        if (methodInfo != null)
                        {
                            // Get bus
                            var bus = MessageBus.Unsafe.GetBus(type);


                            // Get method
                            var method = Delegate.CreateDelegate(typeof(WaiveDelegate), bus, methodInfo) as WaiveDelegate;


                            // Make sure delegate was created
                            Assert.IsNotNull(method, "Failed to create 'WaiveDispatch/WaiveDispatchAll' wrapper delegate");


                            // Store delegate
                            waiveMethods.Add(method);
                        }
                    }
                });


            // Log results
            Assert.GreaterOrEqual(waiveMethods.Count, 2, "Expected to find at least to methods");
        }

        #endregion
    }
}
