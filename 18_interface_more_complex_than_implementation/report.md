## Ghost State

Research about this topic was an interesting one. I did not understand what was meant from the lesson and how to connect the definition from the formal verification with the baby level software engineering :) And decided to chat with Claude as I always do. Before I copied the definition from some paper about ghost state it was insisting on the following interepretation:

```
Ghost state is any property that, if violated, would indicate a bug, but whose violation cannot be detected by the type system or expressed in a contract.
```

and guiding everything torwards pre and postconditions. No word about acutal meaning: auxiliary variables which are used in the proof but do not exist in the actual program. And I found a really interesting code example for the ghost variables in action:

```
function add([int] v1, int c) => ([int] v2)
// Return must have same length as parameters
ensures |v2| == |v1|:
   //
   int i = 0
   int tmp = |v1| // ghost variable
   //
   while i < |v1| where i >= 0 && |v1| == tmp:
      v1[i] = v1[i] + c
      i = i + 1
   //
   return v1
```

Here we use the tmp solely for proof purposes, because without having it in the loop invariant compiler can not infer that v1.size has not changed. And because it does not mutate any state we can exclude it from the non debug build, how cool :) This is a very powerful tool when used correctly for theorem proving, but very often we just pollute code with some state.

```
public virtual async Task<bool?> IsConditionMet(ContactAttribute attribute, IList<CustomAttribute> customAttributes)
{
    ArgumentNullException.ThrowIfNull(attribute);
    customAttributes ??= new List<CustomAttribute>();

    var conditionAttribute = attribute.ConditionAttribute;
    if (!conditionAttribute.Any())
        return null;

    var dependOnAttribute = (await ParseContactAttributes(conditionAttribute)).FirstOrDefault();
    if (dependOnAttribute == null)
        return true;

    ...

    //compare values
    var allFound = true;
    foreach (var t1 in valuesThatShouldBeSelected)
    {
        var found = false;
        foreach (var t2 in selectedValues.Where(t2 => t1 == t2))
            found = true;
        if (!found)
            allFound = false;
    }

    return allFound;
}
```

No need for this `allFound` and confusing manipulations. Just sets!

```
var selectedSet = selectedValues.ToHashSet();
var requiredSet = valuesThatShouldBeSelected.ToHashSet();
return selectedSet.SetEquals(requiredSet);
```

Another code snippet also with a for loop. If we use LINQ, then we should use it to 100%!

```
foreach (var a2 in attributes2)
{
    var conditionMet = await _contactAttributeParser.IsConditionMet(a2, customAttributes);
    if (!a2.IsRequired || ((!conditionMet.HasValue || !conditionMet.Value) && conditionMet.HasValue)) continue;

    var found = false;
    foreach (var a1 in attributes1)
    {
        if (a1.Id != a2.Id) continue;
        var attributeValuesStr = customAttributes.Where(x => x.Key == a1.Id).Select(x => x.Value).ToList();
        foreach (var str1 in attributeValuesStr)
            if (!string.IsNullOrEmpty(str1.Trim()))
            {
                found = true;  // Изменяется
                break;
            }
    }

    if (!found)
        warnings.Add(...);
}
```

we could rewrite this flab and unclear loop to

```
var hasValidValue = attributes1
               .Where(a1 => a1.Id == a2.Id)
               .SelectMany(a1 => customAttributes.Where(x => x.Key == a1.Id).Select(x => x.Value))
               .Any(value => !string.IsNullOrEmpty(value.Trim()));

```

## Imprecision

Looking at order notification section I found this sample:

```
orderPlacedAttachmentFileName =
    _orderSettings.AttachPdfInvoiceToOrderPlacedEmail && !_orderSettings.AttachPdfInvoiceToBinary
        ? "order.pdf"
        : null;
```

We actually have here one big problem - customer places 5 orders, downloads 5 emails —> all attachments named "order.pdf". No way to distinguish which invoice is which. I would better save some pattern in the settings which we could use as template
and insert concrete order number later

```
// SPEC: ResolveAttachmentFileName(order) -> string | null
//   when attachments disabled: null
//   when enabled: pattern from settings with order context
//   pattern: "{prefix}_{OrderNumber}.pdf" where prefix in [order, invoice, ...]
//   default pattern: "order_{OrderNumber}.pdf"
private string ResolveAttachmentFileName(Order order)
{
    if (!_orderSettings.AttachPdfInvoiceToOrderPlacedEmail || _orderSettings.AttachPdfInvoiceToBinary)
        return null;

    var pattern = _orderSettings.InvoiceFileNamePattern ?? "order_{OrderNumber}.pdf";
    return pattern.Replace("{OrderNumber}", order.OrderNumber);
}
```

Next code snippet refers to image processings.

```
string picture1 = null;
string picture2 = null;
string picture3 = null;
var i = 0;
foreach (var picture in product.ProductPictures.Take(3))
{
    var pic = await _pictureService.GetPictureById(picture.PictureId);
    var pictureLocalPath = await _pictureService.GetThumbPhysicalPath(pic);
    switch (i)
    {
        case 0:
            picture1 = pictureLocalPath;
            break;
        case 1:
            picture2 = pictureLocalPath;
            break;
        case 2:
            picture3 = pictureLocalPath;
            break;
    }
    i++;
}
```

I would better parametrize the method with `maxPictures` and make it non nullable, because code in this project makes evrything
nullable and then coalleses to fallback values.

```
// SPEC: (Product, int) -> Dictionary[string, string]
//   maxPictures: positive int, upper bound on pictures to export
//   result keys: "Picture1", "Picture2", ... , "PictureN"
//   where N = min(product.Pictures.Count, maxPictures)
private async Task<Dictionary<string, string>> GetProductPicturesForExport(Product product, int maxPictures)
{
    var pictures = new Dictionary<string, string>();
    var index = 1;

    foreach (var picture in product.ProductPictures.Take(maxPictures))
    {
        var pic = await _pictureService.GetPictureById(picture.PictureId);
        var path = await _pictureService.GetThumbPhysicalPath(pic);
        pictures[$"Picture{index}"] = path;
        index++;
    }

    return pictures;
}
```

## Interface must not be oversimplified

[This](https://github.com/NEventStore/NEventStore/blob/master/src/NEventStore/IAccessSnapshots.cs) is the first example. At first glance this looks like just 3 methods. But the real interface here is not the C# signatures — it’s the specification around snapshots, revisions, storage consistency, and multi-thread safety. That spec is unavoidably more complex due to the specific domain and we can not really do anything about it. Bucket id, stream id, revision id? Yes, we need need this to get a snapshot, this is the specification. We could of course add wrappers around these types, but this will not remove the inherent complexity.

As I understand it now, main point here is that we just need to accept that some domains or areas are complex. Another area where we can not really cut off or simplify things - expression
tree programming in C#.

```c#
public static LambdaExpression Lambda(Expression body, string? name, bool tailCall, IEnumerable<ParameterExpression>? parameters);
```

We can not just reduce the expression creation to name and body. What about parameters, tail call optimization? Or maybe we know the type of the expression at compile time, then there is also an overload taking `Type delegateType` representing the delegate signature. We just need to undertsand the domain...