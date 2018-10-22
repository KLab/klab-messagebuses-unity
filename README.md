# *KLab Message Buses for Unity*

While not silver bullets, message buses (or [event buses as *Lumberyard* calls them](https://docs.aws.amazon.com/lumberyard/latest/userguide/ebus-in-depth.html))
can be an effective mechanism for loosen up coupling between systems.
*KLab Message Buses for Unity* provides a simple API for using buses in your *Unity* projects with low runtime overhead.


## Message Buses

Using a message bus basically consists of just 3 steps.

1. You declare a message bus
1. You connect to it (and later disconnect from it)
1. You send messages through the bus

See below is a minimal example. (The example uses *Unity* components for handling connecting and sending,
but you can also use message buses in 'pure' *C#* classes).

```cs
using KLab.MessageBuses;
using UnityEngine;


public sealed class MyMessageBus : MessageBus<string> {}


public sealed class Sender : MonoBehaviour
{
    private const string Message = "Hello, World!";

    private MyMessageBus Bus { get; set; }



	private void Start ()
    {
        Bus = MessageBus.GetBus<MyMessageBus>();
	}

	private void Update ()
    {
        Bus.Broadcast(Message);
	}
}


public sealed class Receiver : MonoBehaviour
{
    private void OnMessage(string message)
    {
        Debug.Log(message);
    }


    private void OnEnable()
    {
        MessageBus
            .GetBus<MyMessageBus>()
            .Connect(OnMessage);
    }

    private void OnDisable()
    {
        MessageBus
            .GetBus<MyMessageBus>()
            .Disconnect(OnMessage);
    }
}
```

See [here](./KLab/MessageBuses/Runtime/MessageBus.cs) for all types of buses available;
and [here](./KLab/MessageBuses/Examples/AdvancedMessageBusExamples.cs) or [here](./KLab/MessageBuses/Tests/MessageBusTests.cs) for (advanced) examples.

If you want to create a non-global message bus, simply instantiate it on your own instead of getting the global singleton through ```MessageBus.GetBus<T>()```.


## Importing Into Unity

The library can be imported easily as a Unity package. It doesn't have any dependencies on other packages.


## Feedback

- Request a new feature on GitHub
- File a bug in GitHub Issues


## License

Copyright (c) KLab Inc.. All rights reserved.

Licensed under the [MIT](LICENSE) License.
