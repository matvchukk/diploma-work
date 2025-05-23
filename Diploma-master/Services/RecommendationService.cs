using Neo4j.Driver;
using Microsoft.EntityFrameworkCore;
using Diploma.Contexts;
using Diploma.Contracts;

namespace Diploma.Services;

public class RecommendationService
{
    private readonly IDriver _driver;
    private readonly ShopDbContext _context;

    public RecommendationService(IDriver driver, ShopDbContext context)
    {
        _driver = driver;
        _context = context;
    }

    public async Task<List<ProductDto>> GetRecommendationsAsync(Guid userId, int limit = 10)
    {
        var session = _driver.AsyncSession();

        try
        {
            var result = await session.RunAsync(@"
                MATCH (u:User {id: $userId})
                MATCH (p:Product)
                WHERE p.node2vecEmbedding IS NOT NULL AND u.node2vecEmbedding IS NOT NULL AND p.pageRank IS NOT NULL
                WITH p,
                     gds.similarity.cosine(u.node2vecEmbedding, p.node2vecEmbedding) AS similarity,
                     p.pageRank AS pagerank
                WITH p, (0.8 * similarity + 0.2 * pagerank) AS score
                ORDER BY score DESC
                LIMIT $limit
                RETURN p.id AS id
            ", new { userId = userId.ToString(), limit });

            var productIds = await result.ToListAsync(r => Guid.Parse(r["id"].As<string>()));

            var allProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new();

            var filtered = allProducts
                .Where(p => !p.Restrictions.Intersect(user.Restrictions).Any())
                .Where(p => !user.IsVegan || p.IsVegan)
                .ToList();

            return filtered.Select(p => new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                Calories = p.Calories,
                Protein = p.Protein,
                Fat = p.Fat,
                Carbs = p.Carbs,
                IsVegan = p.IsVegan,
                Restrictions = p.Restrictions,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName ?? "",
                SellerId = p.SellerId,
                SellerName = p.Seller?.StoreName ?? "",
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0.0
            }).ToList();
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    public async Task<DailyMealPlanDto> GenerateStructuredDailyMenuAsync(Guid userId)
    {
        var recommended = await GetRecommendationsAsync(userId, 100);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return new();

        double bmr = user.Sex == "male"
            ? 10 * user.WeightKg + 6.25 * user.HeightCm - 5 * user.Age + 5
            : 10 * user.WeightKg + 6.25 * user.HeightCm - 5 * user.Age - 161;

        double activity = 1.375;
        double goalMultiplier = user.Goal switch
        {
            "lose weight" => 0.85,
            "gain weight" => 1.15,
            _ => 1.0
        };

        double dailyCalories = bmr * activity * goalMultiplier;
        double dailyProtein = dailyCalories * 0.3 / 4;
        double dailyFat = dailyCalories * 0.3 / 9;
        double dailyCarbs = dailyCalories * 0.4 / 4;
        double dailyBudget = (double)user.BudgetPerWeek / 7;

        var categories = new Dictionary<string, string[]>
        {
            ["protein"] = new[] { "Meat and sausages", "Fish and seafood" },
            ["protein_extra"] = new[] { "Dairy products", "Eggs" },
            ["carbs"] = new[] { "Bread and pastries", "Cereals and pasta" },
            ["carbs_extra"] = new[] { "Vegetables" },
            ["fats"] = new[] { "Oils", "Nuts" }
        };

        var plan = new DailyMealPlanDto();
        var usedProductIds = new HashSet<Guid>();

        async Task<List<ProductWithGramsDto>> PickMealAsync()
        {
            var meal = new List<ProductWithGramsDto>();
            double budget = dailyBudget / 3;
            double kcal = dailyCalories / 3;
            double protein = dailyProtein / 3;
            double fat = dailyFat / 3;
            double carbs = dailyCarbs / 3;

            async Task<bool> PickProducts(string[] mainCat, string[] extraCat, double target, Func<ProductDto, double> selector, double mainRatio)
            {
                double mainTarget = target * mainRatio;
                double extraTarget = target * (1 - mainRatio);

                foreach (var (cats, portion) in new[] { (mainCat, mainTarget), (extraCat, extraTarget) })
                {
                    var candidates = recommended
                        .Where(p => cats.Contains(p.CategoryName) && selector(p) > 0.1 && !usedProductIds.Contains(p.Id))
                        .OrderByDescending(selector)
                        .ToList();

                    foreach (var product in candidates)
                    {
                        double grams = portion / (selector(product) / 100.0);
                        grams = Math.Ceiling(grams / 10.0) * 10.0;
                        if (grams < 10) continue;
                        if (grams > 500) continue;

                        double cost = (double)product.Price * (grams / 100.0);
                        double cal = product.Calories * (grams / 100.0);
                        if (cost > budget * 1.5 || cal > kcal * 1.2) continue; // +20% допустимого

                        usedProductIds.Add(product.Id);
                        meal.Add(new ProductWithGramsDto { Product = product, Grams = grams });

                        budget -= cost;
                        kcal -= cal;
                        protein -= product.Protein * (grams / 100.0);
                        fat -= product.Fat * (grams / 100.0);
                        carbs -= product.Carbs * (grams / 100.0);

                        break;
                    }
                }

                return true;
            }

            await PickProducts(categories["protein"], categories["protein_extra"], protein, p => p.Protein, 0.8);
            await PickProducts(categories["carbs"], categories["carbs_extra"], carbs, p => p.Carbs, 0.8);
            await PickProducts(categories["fats"], Array.Empty<string>(), fat, p => p.Fat, 1.0);

            return meal;
        }

        plan.Breakfast = await PickMealAsync();
        plan.Lunch = await PickMealAsync();
        plan.Dinner = await PickMealAsync();

        var allMeals = plan.Breakfast.Concat(plan.Lunch).Concat(plan.Dinner);
        plan.TotalPrice = allMeals.Sum(p => (double)p.Product.Price * (p.Grams / 100.0));
        plan.TotalCalories = allMeals.Sum(p => p.Product.Calories * (p.Grams / 100.0));
        plan.TotalProtein = allMeals.Sum(p => p.Product.Protein * (p.Grams / 100.0));
        plan.TotalFat = allMeals.Sum(p => p.Product.Fat * (p.Grams / 100.0));
        plan.TotalCarbs = allMeals.Sum(p => p.Product.Carbs * (p.Grams / 100.0));

        return plan;
    }




}
