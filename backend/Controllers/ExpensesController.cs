using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ requires valid JWT token
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: Get current user's ID from JWT
        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }

        // ✅ 1. Get all expenses for this user
        [HttpGet]
        public async Task<IActionResult> GetExpenses()
        {
            int userId = GetUserId();
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
            return Ok(expenses);
        }

        // ✅ Filter and sort expenses by category, date range, or amount
        [HttpGet("filter")]
        public async Task<IActionResult> FilterExpenses(
            string? category,
            DateTime? startDate,
            DateTime? endDate,
            string? sort)
        {
            int userId = GetUserId();

            var query = _context.Expenses
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(e => e.Category == category);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort == "amount_asc")
                    query = query.OrderBy(e => e.Amount);
                else if (sort == "amount_desc")
                    query = query.OrderByDescending(e => e.Amount);
                else if (sort == "date_asc")
                    query = query.OrderBy(e => e.Date);
                else if (sort == "date_desc")
                    query = query.OrderByDescending(e => e.Date);
            }

            var results = await query.ToListAsync();
            return Ok(results);
        }

                // ✅ Total spending grouped by category
        [HttpGet("summary/category")]
        public async Task<IActionResult> SummaryByCategory()
        {
            int userId = GetUserId();

            var summary = await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(summary);
        }

        // ✅ Total spending grouped by month (YYYY-MM)
        [HttpGet("summary/monthly")]
        public async Task<IActionResult> SummaryByMonth()
        {
            int userId = GetUserId();

            var summary = await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Ok(summary);
        }

        // ✅ 2. Add a new expense
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            int userId = GetUserId();
            expense.UserId = userId;
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return Ok(expense);
        }

        // ✅ 3. Update an existing expense
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense updated)
        {
            int userId = GetUserId();
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense == null)
                return NotFound("Expense not found or not owned by you.");

            expense.Title = updated.Title;
            expense.Category = updated.Category;
            expense.Amount = updated.Amount;
            expense.Date = updated.Date;

            await _context.SaveChangesAsync();
            return Ok(expense);
        }

        // ✅ 4. Delete an expense
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            int userId = GetUserId();
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense == null)
                return NotFound("Expense not found or not owned by you.");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
