// BEFORE

public record ShoppingCartCommonWarningsValidatorRecord(
    Customer Customer,
    Store Store,
    IList<ShoppingCartItem> ShoppingCarts,
    Product Product,
    ShoppingCartType ShoppingCartType,
    DateTime? RentalStartDate,
    DateTime? RentalEndDate,
    int Quantity,
    string ReservationId);

public class ShoppingCartCommonWarningsValidator : AbstractValidator<ShoppingCartCommonWarningsValidatorRecord>
{
    public ShoppingCartCommonWarningsValidator(ITranslationService translationService, IPermissionService permissionService, ShoppingCartSettings shoppingCartSettings)
    {
        RuleFor(x => x).CustomAsync(async (value, context, _) =>
        {
            //maximum items validation
            switch (value.ShoppingCartType)
            {
                case ShoppingCartType.ShoppingCart:
                    {
                        if (value.ShoppingCarts.Count >= shoppingCartSettings.MaximumShoppingCartItems)
                            context.AddFailure(string.Format(
                                translationService.GetResource("ShoppingCart.MaximumShoppingCartItems"),
                                shoppingCartSettings.MaximumShoppingCartItems));
                    }
                    break;
                case ShoppingCartType.Wishlist:
                    {
                        if (value.ShoppingCarts.Count >= shoppingCartSettings.MaximumWishlistItems)
                            context.AddFailure(string.Format(
                                translationService.GetResource("ShoppingCart.MaximumWishlistItems"),
                                shoppingCartSettings.MaximumWishlistItems));
                    }
                    break;
            }

            switch (value.ShoppingCartType)
            {
                case ShoppingCartType.ShoppingCart
                    when !await permissionService.Authorize(StandardPermission.EnableShoppingCart, value.Customer):
                    context.AddFailure("Shopping cart is disabled");
                    return;
                case ShoppingCartType.Wishlist
                    when !await permissionService.Authorize(StandardPermission.EnableWishlist, value.Customer):
                    context.AddFailure("Wishlist is disabled");
                    return;
            }

            if (value.Quantity <= 0)
                context.AddFailure(translationService.GetResource("ShoppingCart.QuantityShouldPositive"));
        });
    }
}

// AFTER

// In the context of validating common warnings for shopping cart and wishlist, the system must ensure that:
// 1. The number of items in the shopping cart must be less than the maximum allowed items for shopping cart in global settings when the shopping cart type is ShoppingCart.
// 2. The number of items in the shopping cart must be less than the maximum allowed items for wishlist in global settings when the shopping cart type is Wishlist.
// 3. The customer must be authorized by permission service to enable the shopping cart when the shopping cart type is ShoppingCart.
// 4. The customer must be authorized by permission service to enable the wishlist when the shopping cart type is Wishlist.
// 5. The requested quantity must be greater than 0.
public record ShoppingCartCommonWarningsValidationContext(Customer Customer, IReadOnlyList<ShoppingCartItem> ShoppingCarts, ShoppingCartType ShoppingCartType, int RequestedQuantity, ShoppingCartSettings Settings, IPermissionService PermissionService);

public class ShoppingCartCommonWarningsValidator : AbstractValidator<ShoppingCartCommonWarningsValidationContext>
{
    private readonly ITranslationService _translationService;

    public ShoppingCartCommonWarningsValidator(ITranslationService translationService)
    {
        _translationService = translationService;

        RuleFor(ItemCount)
            .LessThan(MaxCartItems).When(ShoppingCartTypeIsShoppingCart, ApplyConditionTo.CurrentValidator).WithMessage(MaxCartItemsMessage)
            .LessThan(MaxWishlistItems).When(ShoppingCartTypeIsWishlist, ApplyConditionTo.CurrentValidator).WithMessage(MaxWishlistItemsMessage);

        RuleFor(WholeContext).Cascade(CascadeMode.Stop)
            .MustAsync(HaveCustomerAuthorizedForCartEnabling).When(ShoppingCartTypeIsShoppingCart, ApplyConditionTo.CurrentValidator).WithMessage(CartDisabledMessage)
            .MustAsync(HaveCustomerAuthorizedForWishlistEnabling).When(ShoppingCartTypeIsWishlist, ApplyConditionTo.CurrentValidator).WithMessage(WishlistDisabledMessage);

        RuleFor(Quantity).GreaterThan(0).WithMessage(QuantityPositiveMessage);
    }

    private static readonly Expression<Func<ShoppingCartCommonWarningsValidationContext, int>> ItemCount = context => context.ShoppingCarts.Count;

    private static readonly Expression<Func<ShoppingCartCommonWarningsValidationContext, ShoppingCartCommonWarningsValidationContext>> WholeContext = context => context;

    private static readonly Expression<Func<ShoppingCartCommonWarningsValidationContext, int>> Quantity = context => context.RequestedQuantity;

    private static readonly Expression<Func<ShoppingCartCommonWarningsValidationContext, int>> MaxCartItems = context => context.Settings.MaximumShoppingCartItems;

    private static readonly Expression<Func<ShoppingCartCommonWarningsValidationContext, int>> MaxWishlistItems = context => context.Settings.MaximumWishlistItems;

    private static bool ShoppingCartTypeIsShoppingCart(ShoppingCartCommonWarningsValidationContext validationContext) => validationContext.ShoppingCartType == ShoppingCartType.ShoppingCart;

    private static bool ShoppingCartTypeIsWishlist(ShoppingCartCommonWarningsValidationContext validationContext) => validationContext.ShoppingCartType == ShoppingCartType.Wishlist;

    private async Task<bool> HaveCustomerAuthorizedForCartEnabling(ShoppingCartCommonWarningsValidationContext validationContext, CancellationToken cancellationToken) => await validationContext.PermissionService.Authorize(StandardPermission.EnableShoppingCart, validationContext.Customer);

    private async Task<bool> HaveCustomerAuthorizedForWishlistEnabling(ShoppingCartCommonWarningsValidationContext validationContext, CancellationToken cancellationToken) => await validationContext.PermissionService.Authorize(StandardPermission.EnableWishlist, validationContext.Customer);

    private string MaxCartItemsMessage(ShoppingCartCommonWarningsValidationContext validationContext) => string.Format(_translationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), validationContext.Settings.MaximumShoppingCartItems);

    private string MaxWishlistItemsMessage(ShoppingCartCommonWarningsValidationContext validationContext) => string.Format(_translationService.GetResource("ShoppingCart.MaximumWishlistItems"), validationContext.Settings.MaximumWishlistItems);

    private string CartDisabledMessage(ShoppingCartCommonWarningsValidationContext validationContext) => _translationService.GetResource("ShoppingCart.CartDisabled");

    private string WishlistDisabledMessage(ShoppingCartCommonWarningsValidationContext validationContext) => _translationService.GetResource("ShoppingCart.WishlistDisabled");

    private string QuantityPositiveMessage(ShoppingCartCommonWarningsValidationContext validationContext) => _translationService.GetResource("ShoppingCart.QuantityShouldPositive");
}