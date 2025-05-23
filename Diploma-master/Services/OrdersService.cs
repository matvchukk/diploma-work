using Diploma.Contexts;
using Diploma.Entities;
using Diploma.Contracts;
using Microsoft.EntityFrameworkCore;
using Diploma.Extensions;

namespace Diploma.Services;

public class OrdersService
{
    private readonly ShopDbContext _context;

    public OrdersService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task MakeOrderAsync(Guid userId, CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            throw new Exception("Адреса доставки обов’язкова");

        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new Exception("Кошик порожній");

        var order = new Order(userId, request.ShippingAddress);

        foreach (var item in cartItems)
        {
            var unitPrice = item.Product.Price;
            var total = unitPrice * item.Quantity / 100;

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Product = item.Product,
                Order = order
            };

            order.OrderItems.Add(orderItem);
            order.TotalPrice += total;
        }

        _context.Orders.Add(order);

        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderDto>> GetOrdersAsync(Guid userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            ShippingAddress = o.ShippingAddress,
            Status = o.Status.ToString(),
            TotalPrice = o.TotalPrice,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product.ProductName,
                Quantity = oi.Quantity,
                UnitPrice = oi.Product.Price,
                TotalPrice = oi.Product.Price * oi.Quantity / 100
            }).ToList()
        }).ToList();
    }

    public async Task ClearOrderHistoryAsync(Guid userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ToListAsync();

        _context.Orders.RemoveRange(orders);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new Exception("Замовлення не знайдено");

        order.Status = newStatus;
        await _context.SaveChangesAsync();
    }
}
