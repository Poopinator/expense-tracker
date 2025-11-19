namespace backend.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Limit { get; set; }
    }

    public class BudgetUpdateDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
}

}
