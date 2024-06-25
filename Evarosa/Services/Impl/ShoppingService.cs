using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using System.Security.Claims;

namespace Evarosa.Services.Impl
{
    public class ShoppingService
    {
        private readonly UnitOfWork _unitOfWork;

        string ShoppingCartId { get; set; }

        public const string CartSessionKey = "VicoShoppingCart";

        public ShoppingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static ShoppingService GetCart(HttpContext context, UnitOfWork unitOfWork)
        {
            var cart = new ShoppingService(unitOfWork);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingService GetCart(Controller controller, UnitOfWork unitOfWork)
        {
            return GetCart(controller.HttpContext, unitOfWork);
        }

        public void AddToCart(Product product, Sku? sku, int quantity = 1)
        {
            // Get the matching cart and album instances
            var cartQr = _unitOfWork.CartItem.GetAll(
                    predicate: c => c.CartId == ShoppingCartId
                        && c.ProductId == product.Id,
                    disableTracking: false
                );

            if (sku != null)
            {
                cartQr = cartQr.Where(c => c.SkuId == sku.Id);
            }

            var cartItem = cartQr.FirstOrDefault();

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new CartItem
                {
                    RecordId = Guid.NewGuid().ToString(),
                    ProductId = product.Id,
                    CartId = ShoppingCartId,
                    Quantity = quantity,
                };

                if (sku != null)
                {
                    cartItem.Price = sku.FinalPrice;
                    cartItem.SkuId = sku.Id;
                } else
                {
                    cartItem.Price = product.FinalPrice;
                }

                _unitOfWork.CartItem.Insert(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Quantity += quantity;
            }
            // Save changes
            _unitOfWork.Commit();
        }

        public void UpdateFromCart(string recordId, int quantity)
        {
            // Get the matching cart and album instances
            var cartItem = _unitOfWork.CartItem.GetAll(
                    predicate: c => c.RecordId == recordId, disableTracking: false).FirstOrDefault();

            if (cartItem != null)
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Quantity = quantity;
            }
            // Save changes
            _unitOfWork.Commit();
        }

        public int RemoveFromCart(string id)
        {
            // Get the cart
            var cartItem = _unitOfWork.CartItem.GetAll(
                predicate: cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id).FirstOrDefault();

            int itemCount = 0;

            if (cartItem != null)
            {
                itemCount = cartItem.Quantity;
                _unitOfWork.CartItem.Delete(cartItem);
                // Save changes
                _unitOfWork.Commit();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _unitOfWork.CartItem.GetAll(
                 predicate: cart => cart.CartId == ShoppingCartId)
                .ToList();

            _unitOfWork.CartItem.Delete(cartItems);
            // Save changes
            _unitOfWork.Commit();
        }

        public decimal GetItemTotal(string recordId)
        {
            // Get the matching cart and album instances
            var total = _unitOfWork.CartItem.GetAll(
                predicate: c => c.RecordId == recordId,
                selector: m => m.Total).FirstOrDefault();

            return total;
        }

        public List<CartItem> GetCartItems()
        {
            return _unitOfWork.CartItem.GetAll(
                    predicate: cart => cart.CartId == ShoppingCartId,
                    include: l => l.Include(m => m.Product)
                        .Include(m => m.Sku)
                 ).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = _unitOfWork.CartItem.Count(predicate: m => m.CartId == ShoppingCartId);

            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = _unitOfWork.CartItem.GetAll(predicate: m => m.CartId == ShoppingCartId).Select(m => m.Quantity * m.Price).Sum();

            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    SkuId = item.SkuId,
                    OrderId = order.Id,
                    UnitPrice = item.Price,
                    Quantity = item.Quantity,
                };
                // Set the order total of the shopping cart
                orderTotal += item.Quantity * item.Price;

                _unitOfWork.OrderDetail.Insert(orderDetail);
            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            _unitOfWork.Commit();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.Id;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value))
                {
                    context.Session.SetString(CartSessionKey, context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }
            return context.Session.GetString(CartSessionKey) ?? "";
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = _unitOfWork.CartItem.GetAll(
                predicate: c => c.CartId == ShoppingCartId, disableTracking: false)
                .ToList();

            foreach (var item in shoppingCart)
            {
                item.CartId = userName;
            }
            _unitOfWork.Commit();
        }
    }
}
