
using Rotativa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using xyzpharmacy.Data.Cart;
using xyzpharmacy.Data.Services;
using xyzpharmacy.Data.Static;
using xyzpharmacy.Data.ViewModels;
using xyzpharmacy.Models;
using System.Security.Claims;
using Rotativa.NetCore;
using Azure;

namespace xyzpharmacy.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IProductsService _service;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersService _orderservice;


        public OrdersController( IProductsService Service, ShoppingCart shoppingCart,IOrdersService ordersService)
        {
            _service = Service;
            _shoppingCart = shoppingCart;
            _orderservice = ordersService;
        }

        public async Task<IActionResult> ShoppingCart()
        {
            try
            {
                var items = _shoppingCart.GetShoppingCartItems();
                _shoppingCart.ShoppingCartItems = items;

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);
                List<Order> orderslist = await _orderservice.GetOrders();
                List<Order> Found = orderslist.FindAll(x => x.UserId == userId);
                if (Found.Count == 0)
                {
                    var response = new ShoppingCartVM()
                    {
                        ShoppingCart = _shoppingCart,
                        ShoppingCartTotal = (_shoppingCart.GetShoppingCartTotal() / 2)

                    };

                    return View(response);
                }
                else if (Found.Count >= 3)
                {
                    var response = new ShoppingCartVM()
                    {
                        ShoppingCart = _shoppingCart,
                        ShoppingCartTotal = (_shoppingCart.GetShoppingCartTotal() - 20)
                    };
                    
                    return View(response);

                }
                else
                {
                    var response = new ShoppingCartVM()
                    {
                        ShoppingCart = _shoppingCart,
                        ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
                    };
                    
                    return View(response);
                }
            }
            catch (Exception)
            {

                throw;
            }
            
           
        }
        public async Task<IActionResult> CompleteOrder()
        {
            try
            {
                var items = _shoppingCart.GetShoppingCartItems();
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);
                foreach (var item in items)
                {
                    var list = await _service.GetAllAsync();
                    Product found = list.FirstOrDefault(X => X.ProductId == item.product.ProductId)!;
                    found.Stock = (int)(found.Stock - item.Amount);
                    await _service.UpdateAsync(found.ProductId, found);

                }
                await _orderservice.StoreOrderAsync(items, userId, userEmailAddress);
                await _shoppingCart.ClearShoppingCartAsync();

                return View("OrderCompleted");

            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userRole = User.FindFirstValue(ClaimTypes.Role);

                var orders = await _orderservice.GetOrdersByUserIdAndRoleAsync(userId, userRole);
                return View(orders);

            }
            catch (Exception)
            {

                throw;
            }
        }
    
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);


                if (item != null)
                {
                    _shoppingCart.AddItemToCart(item);

                }
                return RedirectToAction(nameof(ShoppingCart));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);

                if (item != null)
                {
                    _shoppingCart.RemoveItemFromCart(item);
                }
                return RedirectToAction(nameof(ShoppingCart));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userRole = User.FindFirstValue(ClaimTypes.Role);

                var orders = await _orderservice.GetOrdersByUserIdAndRoleAsync(userId, userRole);
                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {

                        var list = await _service.GetAllAsync();
                        Product found1 = list.FirstOrDefault(X => X.ProductId == item.product.ProductId)!;
                        found1.Stock = (int)(found1.Stock + item.Amount);
                        await _service.UpdateAsync(found1.ProductId, found1);


                    }
                }
                //return View(orders);
                //List<Order> orders = await _orderservice.GetOrders();
                Order found = orders.FirstOrDefault(X => X.Id == Id)!;
                return View(found);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {try
            {
                List<Order> orderlist = await _orderservice.GetOrders();
                if (orderlist == null)
                {
                    return View("NotFound");
                }
                await _orderservice.DeleteAsync(id);


                return View("OrderCancellation");

            }
            catch (Exception) { throw; }
        }

    }
}
