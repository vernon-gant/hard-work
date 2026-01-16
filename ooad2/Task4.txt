Let's say we have an even based system which persists incoming events into an Event Store, reads from it afterwards, merges all of them into snapshots and so on.
Somewhere here there must be a stream of events. And to implement basic behavior for a stream of abstract events we create a class AbstractEventStream. This class defines
main logic for working with events - we can add them in bulk, single, create a snapshot. We tested our functionality and created a nuget package where we can use this class
to create more concrete streams which will extend the behavior of this abstract stream.

Our AbstractEventStream class is closed in terms that it has been tested and deployed to nuget. We do not modify it, BUT we extend it with say ResourceCapabilityStream where we
override some methods for working with our given entity or even add new methods. We can add as many streams for our program entities as we want without touching our exiting code.
All these derived classes are our OPEN classes because they introduce new functionality/behavior in the system without changing our code in closed module.