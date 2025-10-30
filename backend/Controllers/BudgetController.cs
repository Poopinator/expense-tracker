using backend.Data;           // ✅ REQUIRED
using backend.Models;         // ✅ REQUIRED
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }

        // ✅ Create or update a budget
        [HttpPost]
        public async Task<IActionResult> SetBudget([FromBody] Budget budget)
        {
            int userId = GetUserId();
            var existing = await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Category == budget.Category);

            if (existing != null)
            {
                existing.Limit = budget.Limit;
            }
            else
            {
                budget.UserId = userId;
                _context.Budgets.Add(budget);
            }

            await _context.SaveChangesAsync();
            return Ok(budget);
        }

        // ✅ Get all budgets for current user
        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            int userId = GetUserId();
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return Ok(budgets);
        }

        // ✅ Compare current expenses vs budget
        [HttpGet("compare")]
        public async Task<IActionResult> CompareBudgets()
        {
            int userId = GetUserId();

            var budgets = await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Spent = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var comparison = budgets.Select(b => new
            {
                Category = b.Category,
                Limit = b.Limit,
                Spent = expenses.FirstOrDefault(e => e.Category == b.Category)?.Spent ?? 0,
                Remaining = b.Limit - (expenses.FirstOrDefault(e => e.Category == b.Category)?.Spent ?? 0),
                Percentage = (expenses.FirstOrDefault(e => e.Category == b.Category)?.Spent ?? 0) / (b.Limit == 0 ? 1 : b.Limit) * 100
            });

            return Ok(comparison);
        }
    }
}
