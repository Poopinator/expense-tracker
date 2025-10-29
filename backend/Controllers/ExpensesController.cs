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
