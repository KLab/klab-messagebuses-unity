// -------------------------------------------------------------------------------------------- //
//  Copyright (c) KLab Inc.. All rights reserved.                                               //
//  Licensed under the MIT License. See 'LICENSE' in the project root for license information.  //
// -------------------------------------------------------------------------------------------- //

using NUnit.Framework;


namespace KLab.MessageBuses
{
    /// <summary>
    /// Message bus tests
    /// </summary>
    internal sealed class Tests
    {
        /// <summary>
        /// Test message
        /// </summary>
        private class TestMessage
        {
            /// <summary>
            /// Message contents
            /// </summary>
            public string Contents = "TestMessage";
        }

        /// <summary>
        /// Immediate test message bus
        /// </summary>
        private sealed class TestMessageBus : MessageBus<TestMessage> {}

        /// <summary>
        /// Deferred test message bus
        /// </summary>
        private sealed class DeferredTestMessageMBus : DeferredMessageBus<TestMessage> {}


        [Test]
        public void Connected_Received()
        {
            Reset();


            // Arrange
            MessageBus.GetBus<TestMessageBus>().Connect(OnTestMessage);


            // Act
            MessageBus.GetBus<TestMessageBus>().Broadcast(new TestMessage());


            // Assert
            Assert.IsTrue(OnTestMessageInvokeCount == 1);


            MessageBus.GetBus<TestMessageBus>().Disconnect(OnTestMessage);
        }


        [Test]
        public void Disconnected_NotReceived()
        {
            Reset();


            // Arrange
            MessageBus.GetBus<TestMessageBus>().Connect(OnTestMessage);
            MessageBus.GetBus<TestMessageBus>().Broadcast(new TestMessage());


            // Act
            MessageBus.GetBus<TestMessageBus>().Disconnect(OnTestMessage);
            MessageBus.GetBus<TestMessageBus>().Broadcast(new TestMessage());


            // Assert
            Assert.IsTrue(OnTestMessageInvokeCount == 1);
        }


        [Test]
        public void Deferred_Dispatches()
        {
            Reset();


            // Arrange
            MessageBus.GetBus<DeferredTestMessageMBus>().Connect(OnTestMessage);
            MessageBus.GetBus<DeferredTestMessageMBus>().Broadcast(new TestMessage());
            Assert.IsTrue(OnTestMessageInvokeCount == 0);


            // Act
            MessageBus.GetBus<DeferredTestMessageMBus>().Dispatch();


            // Assert
            Assert.IsTrue(OnTestMessageInvokeCount == 1);


            MessageBus.GetBus<DeferredTestMessageMBus>().Disconnect(OnTestMessage);
        }

        #region Helpers

        /// <summary>
        /// Times <see cref="OnTestMessage"/> was invoked
        /// </summary>
        private int OnTestMessageInvokeCount { get; set; }


        /// <summary>
        /// Resets state
        /// </summary>
        private void Reset()
        {
            OnTestMessageInvokeCount = 0;
        }

        /// <summary>
        /// Increments <see cref="OnTestMessageInvokeCount"/>
        /// </summary>
        /// <param name="unused">Unused message body</param>
        private void OnTestMessage(TestMessage unused)
        {
            ++OnTestMessageInvokeCount;
        }

        #endregion
    }
}
