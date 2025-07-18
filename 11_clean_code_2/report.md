# Clean code 2

## Class level

### Too big class - violated SRP

https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Catalog/Services/Categories/CategoryService.cs

This one is not the real maddness which I saw in the project but is also a good example. Typical problem of this "service oriented" approach in modern OOP. We do not form abstractions and just put everything into one service class based on some entity. In the case above, however, even if we do not consider abstracting away query operations and reorganize the approach, the class still does too much. 12/15 methods are querying something from the database and even these query methods can be further grouped for normal cateogory queryies and bread crumb queries. However we also see here state chaing operations like update, insert and delete. Let's believe them - they just had a hard day and if they would do it again it would be done differently...

### Class is too small

```
public class AffiliateValidator : BaseGrandValidator<AffiliateModel>
{
    public AffiliateValidator(IEnumerable<IValidatorConsumer<AffiliateModel>> validators,
        ITranslationService translationService)
        : base(validators)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(translationService.GetResource("Admin.Affiliates.Fields.Name.Required"));
    }
}
```

There is no need here for a separate validator class if we just check one condition. It is one simple if in the code, however in that case we introduce a new dependency for the class without obvious need for this. Validators make sense when we express more less complex validation logic or just have at least 3+ rules. Otherwise this is just an overkill even considering the fact that this class does only one thing and does it good.

### Class owns a method which suits better to another class

https://github.com/vernon-gant/grandnode2/blob/main/src/Core/Grand.Infrastructure/Plugins/PluginManager.cs#L216

Almost all utility methods in a class mean that we do some work related to other entities. Okay, if it is a small private methods which is used in a couple of other methods - all good. But here we obviously see that "Pluing Info" could be extracted in a separate class which could be constructed say using a FileInfo instance and having methods like IsPackagePlugin and Matches.

### Class is filled with data all over the codebase

https://github.com/vernon-gant/grandnode2/blob/main/src/Core/Grand.Domain/Catalog/Product.cs

Obvious drawback of incorrect usage of MVC pattern where model could be undersood as a direct projection of an entity to a class which is the manipulated all other the code in an uncontrolled manner with simple getters and setters. There is no logic in this class, we just pass it like a struct in C and mutate its data in procedures represented through services methods which produce side effects by changing the state of a product. 16 list properties...

### Class depends on the implementation details

https://github.com/ANcpLua/CreatePdf.NET/blob/main/CreatePdf.NET/Internal/OcrService.cs

Found a project from my colleague from the university. He uses just a simple static method in another static methods. This even can not be considered a method - just a function. However, even this function depepds on internal implementation details of OcrTools.GePdfConverter which is implemented using a tesseract OCR. We can not swap the implementation or properly test it because we directly call a static method.

### Type narrowing

https://github.com/vernon-gant/grandnode2/blob/main/src/Core/Grand.Infrastructure/TypeConverters/Converter/GenericDictionaryTypeConverter.cs#L66

All these types manipulations look already very suspicious. The TypeConverter which is implemented here is a conversion from a string into a dictionary. Into a generic dictionary. And what the colleague does in the loop he tries to parse the string and then type cast the extracted key using the method "ConvertFromInvariantString" which is an object to the type K. Okay, K can be of type object and then the conversion will not fail. But what if we try to narrow down to int and we have a string reference? We will get a runtime exception. And it is not handled anywhere. Same applies for the value conversion to type V.

### When creating a child of one class need to create children of other classes

https://github.com/vernon-gant/grandnode2/blob/main/src/Plugins/Authentication.Facebook/FacebookAuthenticationPlugin.cs#L10

After following the logic of creating a new plugin I see that when we create children of a BasePlugin class we also need to create a new child of ISettings class. In that case this is the FacebookExternalAuthSettings. Okay, if we would have a factory pattern, then it could be justified that every factory implied creating a new child of the entity it creates. More less understandable. However here, when we have just simple settings I do not see any value of creating new settings children. I am sure this could be done in a different way.

### Class overrides parent class method and violates LSP

https://github.com/vernon-gant/grandnode2/blob/main/src/Core/Grand.Infrastructure/Caching/Redis/RedisMessageCacheManager.cs#L25

https://github.com/vernon-gant/grandnode2/blob/main/src/Core/Grand.Infrastructure/Caching/MemoryCacheBase.cs#L113

Overriding parent methods without calling the base method or doing it in some ad hoc manner is always a smell. Because we clearly violate the LSP and our subclass does not fulfill the contract and is not replacable with the parent class. In that cas the RedisMessageCacheManager overrides 3 methods of the base class and only in one calls the base method. The remove methods are completely overridden with a different logic specific only for the redis cache. Setting implemented methods to virtual must be prohibited...

---

## Application level

### One modification needs modifications in multiple classes

https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Checkout/Commands/Handlers/Orders/PlaceOrderCommandHandler.cs#L144

This is one of the monsters for 1000 LOC. And if we want to make a simple change like being able to make test orders in the system, this would require changing a lot of stuff here. Firstly, we need to add a field to the main model, but okay this can be considered as neglictable. Next one are the query methods in the repositories - we would need to add the `Where(order => !order.IsTestOrder)` clause to all queries not to return test orders to the customer. Then we would need to change the product reservation, maybe we do not want to reserve products for test orders and just let them exist in the system without affecting the stock. Then we could also potentially touch the currency service - maybe we want the test orders have a specific currency? So this small change with existing approach causes multiple changes in other classes, what should be of course avoiede.

### Complex design patterns where a simple solution can be used

I did not find an example of complex pattern implementation in this project, however I do remember a case from my current job where we have custom fields in the system, and because everything is an even in the system, when creating a custom field you set the value as string and an enum of the type of the custom field. Then somewhere deep in code we use the ad hoc type conversion from a string to the destination type of the custom field - int, string, bool, guid and then I clearly remember a visitor on these primitive types like

```
void Visit(string value);

void Visit(int value);

void Visit(bool value);
```

which were mappin the value to the database representation. Together with a comment - "visitor pattern works not that good with primitives but still works". Something in this manner. Although we can easitly avoid it just by defining a simple "CustomFieldFactory" with 3 methods "WorksWith" which would define the enum value the factory works with. "CanConvert" to accept a string and check whether we can convert the initial string to the destinatin type and avoid any ad hoc conersion. And then "Project" or something similar where we serialize it to a database string. No visitors with primitives - just small abstaction.