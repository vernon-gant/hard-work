# Introduction

I decided to approach this task using a dedicated framework or I would better say a programming model - state maching.
A very powerful tool if used correctly which in many cases allows us to avoid clumsy if checks. How? I like the idea which
was implemented in the Stateless library in .NET - guards. Nothing new - same precondition according to Betrand Meyer, just
a smarter word for it. They can be easily implemented using my favourite FluentValidation DSL in case of complex rules or just
few simple ifs if there is nothing big. Then we have the `OnEntry` and `OnExit` for every state which are our actions when
we enter a new state or we leave the state. Leaving the state might be useful when we want to notify someone about the change,
however we can also do that in the entry. The rest is just to configure valid transitions for states. I will not write state
machines for whole features like order placement process here, because it would require to rewrite the whole project, but will
focus on eliminating these low level checks into guards or eliminating them completely due to illegal state transition. Rewriting
the action parts will be mostly ignored or simplified + infrastructural concerns, because for now it does not support DI
and callbacks configuration is also delegate based and not interface based, what makes the extension a bit harder.

## 1. Cancel order

```c#
public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
{
    ArgumentNullException.ThrowIfNull(request.Order);

    if (request.Order.OrderStatusId == (int)OrderStatusSystem.Cancelled)         
        throw new Exception("Cannot do cancel for order.");

    var shipments = await _shipmentService.GetShipmentsByOrder(request.Order.Id);
    if (shipments.Any())
        throw new Exception("Cannot do cancel for order with shipments");
    ...
    50 lines of code
}
```

As already said the first if check is eliminated by the state maching configuration and the second one should be encorporated
into the guard or the precondition.

```c#
private void Configure()
{
    // I do not really like that they use delegates as callbacks during configuration, but what can we do... Actually
    there could be a workaround, but we need to dig into the infrastructure.
    _sm.Configure(OrderStatusSystem.Pending)
        .PermitIfAsync(OrderTrigger.Cancel, OrderStatusSystem.Cancelled, GuardCanCancelAsync);
    
    _sm.Configure(OrderStatusSystem.Processing)
        .PermitIfAsync(OrderTrigger.Cancel, OrderStatusSystem.Cancelled, GuardCanCancelAsync);
    
    _sm.Configure(OrderStatusSystem.Cancelled); // Forbids any transitions
}

publid async Task<bool> GuardCanCancelAsync(...)
{
    ...
}
```

### 2. Delete order

```c#
public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
{
    ArgumentNullException.ThrowIfNull(request.Order);

    var shipments = await _shipmentService.GetShipmentsByOrder(request.Order.Id);
    
    if (shipments.Any())
          throw new Exception("Cannot do delete for order with shipments");

    //check whether the order wasn't cancelled before 
    if (request.Order.OrderStatusId == (int)OrderStatusSystem.Cancelled)
    {
        // Complex cancellation logic...
    }
    ...
  }
```

Again the first if can be pulled up into the guard because it is a precondition and the second branching is nothing but a transition to a new state and can be modelled
using the `OnExit`

```c#
private void Configure()
{
    _sm.Configure(OrderStatusSystem.Cancelled)
        .PermitIfAsync(OrderTrigger.Delete, OrderStatusSystem.Deleted, GuardCanDeleteAsync)
        .OnExitAsync(HandleOrderCancellationCleanupAsync);
  }
```

### 3. Gift voucher

```c#
foreach (var gc in giftVouchers)
    if (request.Activate)
    {
        //activate
        if (gc.GiftVoucherTypeId == GiftVoucherType.Virtual)
        {
            ...
        }

          gc.IsGiftVoucherActivated = true;
          await _giftVoucherService.UpdateGiftVoucher(gc);
      }
      else
      {
          //deactivate
          gc.IsGiftVoucherActivated = false;
          await _giftVoucherService.UpdateGiftVoucher(gc); 
      }
```

Here we have mixing of 2 properties - the state of the voucher and also activation rules are applied based on the voucher
type. We could represent activation and deactivation using state machine(there are also other states, although looks like overkill
here. Other states are created, deleted, redeemed). And then we could polymorphically call activate inside the `OnEnter`

```c#
private void Configure()
{
    _sm.Configure(GiftVoucherState.Created)
          .PermitIfAsync(GiftVoucherTrigger.Activate, GiftVoucherState.Active, GuardCanActivateAsync);

      _sm.Configure(GiftVoucherState.Active)
          .OnEntryAsync(HandleVoucherActivationAsync)
          .Permit(GiftVoucherTrigger.Deactivate, GiftVoucherState.Deactivated);
}
```

### 4. Deliver Shipment

```c#
public async Task<bool> Handle(DeliveryCommand request, CancellationToken cancellationToken)
{
    ArgumentNullException.ThrowIfNull(request.Shipment);

    var order = await _orderService.GetOrderById(request.Shipment.OrderId);

    if (!request.Shipment.ShippedDateUtc.HasValue)
        throw new Exception("This shipment is not shipped yet");

    if (request.Shipment.DeliveryDateUtc.HasValue)
        throw new Exception("This shipment is already delivered");
    ...
}
```

Here every simpler - we do not allow "Deliver" from non-Shipped states and of course we do not allow deliver from "Delivered state". States are everywhere...

```c#
private void Configure()
{
    _sm.Configure(ShipmentState.Created)
        .Permit(ShipmentTrigger.Ship, ShipmentState.Shipped);

    _sm.Configure(ShipmentState.Packed)
        .Permit(ShipmentTrigger.Ship, ShipmentState.Shipped);
    
    _sm.Configure(ShipmentState.Shipped)
        .PermitIfAsync(ShipmentTrigger.Deliver, ShipmentState.Delivered, GuardCanDeliverAsync);
    
    _sm.Configure(ShipmentState.Delivered);
    
    _sm.Configure(ShipmentState.Cancelled);
}
```

### 5. Payment

Not a single example but a whole payment module. If checks are scattered throughout the whole code [here](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Checkout/Services/Payments/PaymentService.cs).
But the solution is again our state machine. Because this

```c#
if (paymentTransaction.TransactionStatus is TransactionStatus.Canceled or not TransactionStatus.Pending)
    return false; //do not allow for cancelled orders
```

in every second method really distracts from the logic. Now we have

```c#
private void Configure()
{
    _sm.Configure(PaymentState.Pending)
       .Permit(PaymentTrigger.Process,     PaymentState.Paid)
       .Permit(PaymentTrigger.Void,        PaymentState.Voided)
       .Permit(PaymentTrigger.Cancel,      PaymentState.Canceled);
    
    _sm.Configure(PaymentState.Authorized)
       .Permit(PaymentTrigger.Capture, PaymentState.Paid)
       .Permit(PaymentTrigger.Void, PaymentState.Voided)
       .Permit(PaymentTrigger.Cancel, PaymentState.Canceled);
    
    _sm.Configure(PaymentState.Refunded);
    _sm.Configure(PaymentState.Voided);
    _sm.Configure(PaymentState.Canceled);
}
```

Now we can not leave the Cancelled state at all!