namespace Diploma.Contracts
{
    public class ProductWithGramsDto
    {
        public ProductDto Product { get; set; } = null!;
        public double Grams { get; set; }
    }

    public class DailyMealPlanDto
    {
        public List<ProductWithGramsDto> Breakfast { get; set; } = new();
        public List<ProductWithGramsDto> Lunch { get; set; } = new();
        public List<ProductWithGramsDto> Dinner { get; set; } = new();

        public double TotalPrice { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProtein { get; set; }
        public double TotalFat { get; set; }
        public double TotalCarbs { get; set; }
    }
}
