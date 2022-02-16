using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace EnhancedEvents
{
    public class LayeredEventTests
    {
        public class TestArgs : EventArgs
        {
            public TestArgs(int arg1=0, string arg2="")
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }
            public int Arg1 { get; }
            public string Arg2 { get; }
        }

        [Test]
        public void TestSubscribeToEvent()
        {
            LayeredEvent<TestArgs> e = new LayeredEvent<TestArgs>();

            void Callback1(object src, TestArgs args) { }
            void Callback2(object src, TestArgs args) { }

            Assert.AreEqual(false, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(0, e.SubscriberCount);

            e.Subscribe(Callback1);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(1, e.SubscriberCount);

            e.Subscribe(Callback2);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(2, e.SubscriberCount);

            e.Subscribe(Callback2, 2);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(3, e.SubscriberCount);

            e.Subscribe(Callback2, -22);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(4, e.SubscriberCount);
        }

        [Test]
        public void TestUnsubscribeFromEvent()
        {
            LayeredEvent<TestArgs> e = new LayeredEvent<TestArgs>();

            void Callback1(object src, TestArgs args) { }
            void Callback2(object src, TestArgs args) { }

            Assert.AreEqual(false, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(0, e.SubscriberCount);

            e.Subscribe(Callback1);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(1, e.SubscriberCount);

            e.Unsubscribe(Callback2);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(1, e.SubscriberCount);

            e.Unsubscribe(Callback1);

            Assert.AreEqual(false, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(0, e.SubscriberCount);

            e.Subscribe(Callback1);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(1, e.SubscriberCount);

            e.Subscribe(Callback2);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(2, e.SubscriberCount);

            e.Subscribe(Callback1);

            Assert.AreEqual(true, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(3, e.SubscriberCount);

            e.Unsubscribe(Callback1);

            Assert.AreEqual(false, e.HasSubscriber(Callback1));
            Assert.AreEqual(true, e.HasSubscriber(Callback2));
            Assert.AreEqual(1, e.SubscriberCount);

            e.Unsubscribe(Callback2);

            Assert.AreEqual(false, e.HasSubscriber(Callback1));
            Assert.AreEqual(false, e.HasSubscriber(Callback2));
            Assert.AreEqual(0, e.SubscriberCount);
        }

        [Test]
        public void TestPublish()
        {
            LayeredEvent<EventArgs> e = new LayeredEvent<EventArgs>();

            int testCounter = 0;

            void Increment(object src, EventArgs args)
            {
                testCounter++;
            }

            e.Subscribe(Increment);
            e.Publish(this, EventArgs.Empty);

            Assert.AreEqual(1, testCounter);

            e.Publish(this, EventArgs.Empty);

            Assert.AreEqual(2, testCounter);
        }

        [Test]
        public void TestPublishWithArgs()
        {
            LayeredEvent<TestArgs> e = new LayeredEvent<TestArgs>();

            int testCounter = 0;

            void Increment(object src, TestArgs args) {
                testCounter += args.Arg1;
            }

            void Decrement(object src, TestArgs args)
            {
                testCounter -= args.Arg1;
            }

            e.Subscribe(Increment);
            e.Publish(this, new TestArgs(2));

            Assert.AreEqual(2, testCounter);

            e.Subscribe(Increment);
            e.Publish(this, new TestArgs(1));

            Assert.AreEqual(4, testCounter);

            e.Subscribe(Decrement);
            e.Subscribe(Decrement);
            e.Subscribe(Decrement);
            e.Publish(this, new TestArgs(3));

            Assert.AreEqual(1, testCounter);
        }

        [Test]
        public void TestPublishWithLayers()
        {
            LayeredEvent<TestArgs> e = new LayeredEvent<TestArgs>();
            List<int> eventLog = new List<int>();

            void Callback1(object src, TestArgs args)
            {
                eventLog.Add(10 + args.Arg1);
            }

            void Callback2(object src, TestArgs args)
            {
                eventLog.Add(20 + args.Arg1);
            }

            void Callback3(object src, TestArgs args)
            {
                eventLog.Add(30 + args.Arg1);
            }

            e.Subscribe(Callback1);
            e.Subscribe(Callback2, -1);
            e.Subscribe(Callback3, 1);
            e.Publish(this, new TestArgs(0));
            int[] expectedLog = new int[] { 20, 10, 30 };
            for (int i = 0; i < 3; i++) 
            {
                Assert.AreEqual(expectedLog[i], eventLog[i]);
            }

            e = new LayeredEvent<TestArgs>();
            eventLog = new List<int>();

            e.Subscribe(Callback1, -999);
            e.Subscribe(Callback2, 100);
            e.Subscribe(Callback3, 1);
            e.Publish(this, new TestArgs(1));
            expectedLog = new int[] { 11, 31, 21 };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(expectedLog[i], eventLog[i]);
            }
        }
    }
}

