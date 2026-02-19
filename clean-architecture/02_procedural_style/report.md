# Procedural style reflection

Honestly, I do not like this direct way of coding. The idea that data sharing, side effects
is not something "bad" but just the way we write programs does not play well with what we study say in strong ideas :) The modern notation of "Services" in OOP where we in the worst case operate with DB models everywhere directly, pass them from one "Service" to another,
mutate their state with setters just drives me mad. In essence modern OOP without ADT, pre and post conditions, with public getters is just another art of procedural programming :)

Maybe the lack of love to this style is because I do not know how to cook it correctly and the real C hard core programmers will tell me that this is the best way to write programs, but I would honsetly use functional programming + adanced typing which I will of course learn :) I think the latter approach would give me more power than Muaddib had... He, who controls the spice, controls the universe :)

My first solution I did with a mixture or a small subset of functional programming, commands, events(mediatr) and using generics. I like it better. But it also took more time for the design, WAY more time. This procedural thing was just 30 minutes :) Just define these global contexts, data flow from top to bottom and here we go. I did not like it :)