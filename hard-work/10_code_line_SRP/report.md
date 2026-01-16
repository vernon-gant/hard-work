# SRP on single line level

## 1. No explicit parameters

I should confess I also like to do it - but will not do it anymore...

```C#
return await _cacheBase.GetAsync(string.Format(CacheKey.PRODUCTS_CUSTOMER_PERSONAL_KEY, request.CustomerId)
```

the correct approach is to use a separate variable for the key and then use it in the method call:

```C#
var key = string.Format(CacheKey.PRODUCTS_CUSTOMER_PERSONAL_KEY, request.CustomerId);
return await _cacheBase.GetAsync(key);
```

## 2. Nested ifs

```c#
if (ca.IsRequired && ((conditionMet.HasValue && conditionMet.Value) || !conditionMet.HasValue))
```

If one comes across this masterpiece, this is a good time to observer if we could refactor this using FluentValidation
because nested conditions expressing some constraints are a good candidate for it. 

```c#
RuleFor(attribute).Must(BeRequired).When(...) .WithMessage("Attribute is required when ...");
``` 

Although single line statements like the one above are not neccessarily that bad, they just always come in bundle
with other such statements, so FluentValidation is a good candidate to refactor them out.

## 3. Context changing method call chain

```c#
_s3Client.MakeObjectPublicAsync(_bucketName, thumbFileName, true).Wait();
```

In the example above we retrieve a task from the S3 client and then wait for it to complete. Althoug it is not
recommended at all to use the blocking call `Wait()`, however if we do need to do it, the better way is to separate
the task creation from the task execution and use a variable to hold the task:

```c#
var task = _s3Client.MakeObjectPublicAsync(_bucketName, thumbFileName, true);
task.Wait();
```

Normally this is achieved by using `await` instead of `Wait()`, but if we do need to use `Wait()`, then the above
approach is the correct one as we can say later inspect the task and see if it was successful or not.

## 4. Too many computations

```c#
fractionPart = fractionPart > 5 ? 10 - fractionPart : 5 - fractionPart;
```

Although concise, this code bundles multiple responsibilities:

1. It subtracts the integer part from fractionPart.
2. It multiplies the result by 10 to shift the decimal.

Just by extracting computations into separate variables, we can make the code more readable:

```c#
var integerPart = Math.Truncate(fractionPart);
var decimalPart = fractionPart - integerPart;
fractionPart = decimalPart * 10;
```

## 5. Complex LINQ method

```c#
 _orderRepository.Table.Where(x => x.CustomerId == customer.Id && !x.Deleted && x.PaymentStatusId == PaymentStatus.Paid && x.StoreId == storeId)
```

Method Where above is just an example. Sometimes Select, Where, GroupBy include very complex logic inside what makes all benefits of LINQ
disappear. In such cases it is better to extract the logic into a separate method with a meaningful name:

```c#
private static bool BelongsToCustomerAndStore(Order order, Customer customer, int storeId)
{
    return order.CustomerId == customer.Id && !order.Deleted && order.PaymentStatusId == PaymentStatus.Paid && order.StoreId == storeId;
}

_orderRepository.Table.Where(order => BelongsToCustomerAndStore(order, customer, storeId));
```

Way better now!