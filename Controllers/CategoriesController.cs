using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Project.Models;

namespace Web_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CategoriesController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] string? type, CancellationToken cancellationToken)
        {
            var query = _dbContext.Categories.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(x => x.Type == type);
            }

            var categories = await query
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    categoryId = x.CategoryId,
                    x.Name,
                    x.Type,
                    x.Icon,
                    x.Color,
                    x.IsSystem
                })
                .ToListAsync(cancellationToken);

            return Ok(categories);
        }
    }
}
