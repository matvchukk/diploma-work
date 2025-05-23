using Diploma.Services.Authorization;
using Diploma.Contracts;
using Diploma.Contexts;
using Microsoft.AspNetCore.Identity;
using Diploma.Entities;
using Diploma.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Services;

public class UsersService
{
    private readonly MyJwtProvider _jwtProvider;
    private readonly MyPasswordHasher _passwordHasher;
    private readonly ShopDbContext _context;

    public UsersService(MyPasswordHasher passwordHasher, MyJwtProvider jwtProvider, ShopDbContext context)
    {
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _context = context;
    }

    public async Task<JwtTokenResult> Register(RegisterUserRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
            throw new Exception("Користувач з таким email вже існує.");

        var hashedPassword = _passwordHasher.Generate(request.Password);

        var newUser = new User(
            name: request.Name,
            email: request.Email,
            hashedPassword: hashedPassword,
            age: request.Age,
            sex: request.Sex,
            weightKg: request.WeightKg,
            heightCm: request.HeightCm,
            goal: request.Goal,
            budgetPerWeek: request.BudgetPerWeek,
            isVegan: request.IsVegan
        );

        if (request.Restrictions is not null)
        {
            newUser.Restrictions = request.Restrictions;
        }

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return _jwtProvider.GenerateToken(newUser.Id.ToString(), "user");
    }

    public async Task<JwtTokenResult> Login(LoginUserRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new Exception("Користувача не знайдено.");

        if (!_passwordHasher.Verify(request.Password, user.HashedPassword))
            throw new Exception("Невірний пароль.");

        return _jwtProvider.GenerateToken(user.Id.ToString(), "user");
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.WishLists)
                .ThenInclude(wl => wl.WishedProducts)
                    .ThenInclude(wi => wi.Product)
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .Include(u => u.Reviews)
                .ThenInclude(r => r.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
            Role = "user",
            Sex = user.Sex,
            WeightKg = user.WeightKg,
            HeightCm = user.HeightCm,
            Goal = user.Goal,
            BudgetPerWeek = user.BudgetPerWeek,
            IsVegan = user.IsVegan,
            Restrictions = user.Restrictions.ToList(),
            RegisteredAt = user.RegisteredAt,

            // WishLists (твоя структура)
            WishLists = user.WishLists.Select(wl => new WishListDto
            {
                Id = wl.Id,
                Name = wl.Name,
                Products = wl.WishedProducts.Select(wi => new WishListProductDto
                {
                    ProductId = wi.ProductId,
                    ProductName = wi.Product?.ProductName ?? string.Empty
                }).ToList()
            }).ToList(),

            // Orders (твоя структура)
            Orders = user.Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                ShippingAddress = o.ShippingAddress,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.ProductName ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product != null ? oi.Product.Price : 0,
                    TotalPrice = oi.Product != null ? oi.Product.Price * oi.Quantity / 100 : 0
                }).ToList()
            }).ToList(),

            // Reviews (твоя структура)
            Reviews = user.Reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedDate = r.CreatedDate,
                UserId = r.UserId,
                UserName = user.Name,
                ProductId = r.ProductId,
                ProductName = r.Product?.ProductName
            }).ToList()
        };
    }


    public async Task<JwtTokenResult> Switch(Guid sellerId)
    {
        // Знайди продавця з включенням повʼязаного користувача
        var seller = await _context.Sellers
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == sellerId);

        if (seller == null || seller.User == null)
            throw new Exception("Користувача, пов'язаного з продавцем, не знайдено.");

        return _jwtProvider.GenerateToken(seller.User.Id.ToString(), "user");
    }

    public async Task UpdateUser(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Користувача не знайдено.");

        user.Name = request.Name;
        user.Age = request.Age;
        user.Sex = request.Sex;
        user.WeightKg = request.WeightKg;
        user.HeightCm = request.HeightCm;
        user.Goal = request.Goal;
        user.BudgetPerWeek = request.BudgetPerWeek;
        user.IsVegan = request.IsVegan;
        user.Restrictions = request.Restrictions ?? new List<string>();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("Користувача не знайдено.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}