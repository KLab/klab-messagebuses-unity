# *KLab Message Buses for Unity*

While not silver bullets, message buses (or [event buses as *Lumberyard* calls them](https://docs.aws.amazon.com/lumberyard/latest/userguide/ebus-in-depth.html))
can be an effective mechanism for loosen up coupling between systems.
*KLab Message Buses for Unity* provides a simple API 
for using buses in *Unity* with low runtime overhead.


## Usage

Using a message bus basically consists of 3 steps.

1. You declare a message bus
1. You connect to it (and later disconnect from it)
1. You send messages through the bus

The below is a minimal example.
(The example uses *Unity* components for handling connecting and sending,
but you can also use message buses in pure *C#* classes).

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

See [here](./Runtime/KLab/MessageBuses/MessageBus.cs#L90) for bus types available.


### Advanced Usage

#### Setting Initial Container Capacities

You can set initial capacities of the underlying containers used by a bus
by decorating the bus with [options](./Runtime/KLab/MessageBuses/MessageBus.cs#L25).

```cs
using KLab.MessageBuses;


[MessageBusOptions(connectionsCapacity : 1024)]
public sealed class MyMessageBus : MessageBus<string> {}
```

#### Finding All Connection Propertiess For Later Query

See [here](./Examples/KLab/MessageBuses/AdvancedExamples.cs#L32).


#### Finding All Waive Methods For Later Invokation

See [here](./Examples/KLab/MessageBuses/AdvancedExamples.cs#L106).


#### Creating A Non-global Bus

If you want to create a non-global message bus,
simply instantiate the message bus class directly
instead of getting the global singleton through ```MessageBus.GetBus<T>()```.


## Unity Import

The library can be imported easily as a Unity package.
It doesn't have any dependencies on other packages.


## Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

### [1.1.0] - 2019-05-09

#### Added

- Add options for controlling message bus container capacities

#### Changed

- Rename package from `com.klab.messagebuses` to `com.klab.message-buses`
- Rename runtime *asmdef* from `KLab.MessageBuses` to `KLab.MessageBuses.Runtime`


## Feedback

- Request a new feature on GitHub
- File a bug in GitHub Issues


## License

Copyright (c) KLab Inc.. All rights reserved.

Licensed under the [MIT](LICENSE) License.
