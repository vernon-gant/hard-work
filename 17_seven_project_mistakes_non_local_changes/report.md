#  Non local changes

Again my favourite grandnode2 - I think I will be able literally rewrite everything myself after 2-3 next hard work sessions, because I am exploring the code base like a real sniffing dog!

## Authorization

Now the code base does not really care a lot about how to implement authorization
and do it this way

```c#
protected async Task<(bool allow, string message)> CheckAccessToProduct(Product product)
{
    if (product == null)
        return (false, "Product not exists");

    if (await _groupService.IsStaff(_contextAccessor.WorkContext.CurrentCustomer) &&
         !(!product.LimitedToStores || (product.Stores.Contains(_contextAccessor.WorkContext.CurrentCustomer.StaffStoreId) && product.LimitedToStores)))
            return (false, "This is not your product");

    return (true, null);
}
```

And then such methods are used in concrete methods of a controller to limit access to something. And this pattern repeats almost everywhere. What if we could make it more declarative and domain friendly? ASP.NET has solution for authorizatoin but it can not be really claim to be declarative, just authorization policy interfaces. And I found [a cool implementation of an authorization engine implemented in Go using of course - graphs](https://openfga.dev/). We build a relations graph instead of imperative checks. To build these realtions we use rules. Like this one

```
{
    "type": "store",
    "relations": {
      "staff": { "this": {} }
    }
}
```

We have a store and we build a relation staff. Like in databases, relations are everywhere :) Then we can create concrete relations for concrete users and concrete stores from C# code(this engine can be deployed in docker container and then just make calls through SDK to it)

```c#
 await client.Write(new ClientWriteRequest
  {
      Writes = new List<ClientTupleKey>
      {
          new()
          {
              User = "user:john",
              Relation = "staff",
              Object = "store:store1"
          }
      }
  });
```

The rule above is nothing but : access to the product has either its editor(creator) or staff of the store which the product belongs to. The rule is not that intuitive but AI helps everywhere :)

```
{
    "type": "product",
    "relations": {
      "parent_store": { "this": {} },

      "editor": {
        "union": {
          "child": [
            {"this": {}},
            {"tupleToUserset": {
              "tupleset": "parent_store"
              "computedUserset": "staff"
            }}
          ]
        }
      }
    }
  }
```

We say here that the product belongs to a store and editor is either a direct editor of this product(if such relation exists) or it s a staff of the store the product belongs to. And we get :

```
await client.Check(new ClientCheckRequest
  {
      User = "user:john",
      Relation = "editor",
      Object = "product:prod123"
  });
```

And much more, because we can also list all objects with some relation(say all products accessible to the user). Very cool!

## Elsa

What looks also as a strong choice for such domain is a workflow library. I personally did not know about this interesting library, but we tend to think in workflows. Of course, sometimes it may be a simplification and always thinking in a workflow manner is a bad idea. Although function composition is also a workflow from some perspective, or maybe not :) But for processes such as order creation(yes we modelled the order states with .NET stateless, but the workflow is still needed) and now we can model the order creation itself. These guys even did the GUI for that - designed. I would stick to normal .NET classes. So now we have [such a workflow](https://github.com/grandnode/grandnode2/blob/main/src/Business/Grand.Business.Checkout/Commands/Handlers/Orders/PlaceOrderCommandHandler.cs). Looks really good.

Main abstractions in the workflow domain are of course : activity and building of the workflow itself. For building we have a cool DSL which allows us to define steps, which we can later adjust on changing requirements.

```c#
public class OrderPlacementWorkflow : IWorkflow
{
      public void Build(IWorkflowBuilder builder)
      {
        builder
            .StartWith<ValidateShoppingCart>()
            .WithName("Validate Cart")
            .Then<PrepareOrderDetails>();

        builder.When(activity => activity.Output["RequiresPayment"] == false)
                  .Then<CreateOrder>()
                  .Then<AdjustInventory>()
                  .Then<MarkOrderAsPaid>()
                  .Then<SendOrderConfirmationEmail>()
                  .Then<CompleteWorkflow>();
       }
}
```

And single Activities are separate classes with one method `OnExecuteAsync`. We also have the context object we can write to so that next components can read data. Yes, this creates a couping between components, because we are not explicit and setting some values in the dictionary with strings. Although from the other hand we can operate here with preconditions that "some acitivy requires this and this to be set before it is called" and just add guard clauses. So it is the responisiblity of the person who configures the workflow to make sure that preceeding component sets everything. There is even a possiblity to integrate this with webhooks and continue the flow when event comes back. But we can definitely rewrite this and other workflows by splitting them to activities(maybe even reusable activities!) and share them across workflows.