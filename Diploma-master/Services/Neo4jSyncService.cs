using Neo4j.Driver;
using Microsoft.EntityFrameworkCore;
using Diploma.Contexts;


namespace Diploma.Services;

public class Neo4jSyncService
{
    private readonly ShopDbContext _context;
    private readonly IDriver _driver;

    public Neo4jSyncService(ShopDbContext context, IConfiguration config)
    {
        _context = context;
        _driver = GraphDatabase.Driver(
            config["Neo4j:Uri"],
            AuthTokens.Basic(config["Neo4j:User"], config["Neo4j:Password"])
        );
    }

    public async Task SyncAllAsync()
    {
        var session = _driver.AsyncSession();
        await session.RunAsync("MATCH (n) DETACH DELETE n");
        var users = await _context.Users.ToListAsync();
        var products = await _context.Products.Include(p => p.Category).Include(p => p.Seller).ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var sellers = await _context.Sellers.ToListAsync();
        var reviews = await _context.Reviews.ToListAsync();
        var wishLists = await _context.WishLists.ToListAsync();
        var wishedItems = await _context.WishedProducts.ToListAsync();
        var orderedItems = await _context.OrderedProducts.Include(o => o.Order).ToListAsync();

        // CATEGORIES
        foreach (var category in categories)
        {
            await session.RunAsync("MERGE (:Category {id: $id, name: $name})",
                new { id = category.Id.ToString(), name = category.CategoryName });
        }

        // SELLERS
        foreach (var seller in sellers)
        {
            await session.RunAsync("MERGE (:Seller {id: $id, name: $name})",
                new { id = seller.Id.ToString(), name = seller.StoreName });
        }

        // USERS
        foreach (var user in users)
        {
            await session.RunAsync(@"
                MERGE (u:User {id: $id})
                SET u.name = $name,
                    u.sex = $sex,
                    u.age = $age,
                    u.weight = $weight,
                    u.height = $height,
                    u.goal = $goal,
                    u.budget = $budget,
                    u.isVegan = $isVegan
            ",
            new
            {
                id = user.Id.ToString(),
                name = user.Name,
                sex = user.Sex,
                age = user.Age,
                weight = user.WeightKg,
                height = user.HeightCm,
                goal = user.Goal,
                budget = user.BudgetPerWeek,
                isVegan = user.IsVegan
            });
        }

        // PRODUCTS
        foreach (var product in products)
        {
            await session.RunAsync(@"
                MERGE (p:Product {id: $id})
                SET p.name = $name,
                    p.price = $price,
                    p.calories = $calories,
                    p.carbs = $carbs,
                    p.fat = $fat,
                    p.protein = $protein,
                    p.isVegan = $isVegan
            ",
            new
            {
                id = product.Id.ToString(),
                name = product.ProductName,
                price = product.Price,
                calories = product.Calories,
                carbs = product.Carbs,
                fat = product.Fat,
                protein = product.Protein,
                isVegan = product.IsVegan
            });

            await session.RunAsync(@"
                MATCH (p:Product {id: $pid}), (c:Category {id: $cid})
                MERGE (p)-[:IN_CATEGORY]->(c)",
                new { pid = product.Id.ToString(), cid = product.CategoryId.ToString() });

            await session.RunAsync(@"
                MATCH (p:Product {id: $pid}), (s:Seller {id: $sid})
                MERGE (p)-[:SOLD_BY]->(s)",
                new { pid = product.Id.ToString(), sid = product.SellerId.ToString() });
        }

        // BOUGHT
        foreach (var order in orderedItems)
        {
            await session.RunAsync(@"
                MATCH (u:User {id: $uid}), (p:Product {id: $pid})
                MERGE (u)-[r:BOUGHT]->(p)
                SET r.weight = 3.0",
                new
                {
                    uid = order.Order.UserId.ToString(),
                    pid = order.ProductId.ToString()
                });
        }

        // REVIEWED
        foreach (var review in reviews)
        {
            double weight = Math.Max(0.2, review.Rating / 5.0);

            await session.RunAsync(@"
                MATCH (u:User {id: $uid}), (p:Product {id: $pid})
                MERGE (u)-[r:REVIEWED]->(p)
                SET r.rating = $rating, r.comment = $comment, r.weight = $weight",
                new
                {
                    uid = review.UserId.ToString(),
                    pid = review.ProductId.ToString(),
                    rating = review.Rating,
                    comment = review.Comment,
                    weight = weight
                });
        }

        // WISHLISTS
        foreach (var list in wishLists)
        {
            await session.RunAsync("MERGE (:WishList {id: $id, name: $name})",
                new { id = list.Id.ToString(), name = list.Name });

            await session.RunAsync(@"
                MATCH (u:User {id: $uid}), (w:WishList {id: $wid})
                MERGE (u)-[r:HAS_WISHLIST]->(w)
                SET r.weight = 0.3",
                new { uid = list.UserId.ToString(), wid = list.Id.ToString() });
        }

        foreach (var item in wishedItems)
        {
            await session.RunAsync(@"
                MATCH (w:WishList {id: $wid}), (p:Product {id: $pid})
                MERGE (w)-[r:CONTAINS]->(p)
                SET r.weight = 0.5",
                new { wid = item.WishListId.ToString(), pid = item.ProductId.ToString() });
        }

        // IN_CATEGORY
        await session.RunAsync(@"
                MATCH (:Product)-[r:IN_CATEGORY]->(:Category)
                SET r.weight = 0.3");

        // SOLD_BY
        await session.RunAsync(@"
                MATCH (:Product)-[r:SOLD_BY]->(:Seller)
                SET r.weight = 0.2");

        await session.CloseAsync();
    }
}
