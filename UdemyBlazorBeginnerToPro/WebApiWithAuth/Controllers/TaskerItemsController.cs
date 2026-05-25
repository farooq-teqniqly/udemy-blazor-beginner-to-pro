using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiWithAuth.Data;
using WebApiWithAuth.Models;

namespace WebApiWithAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskerItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskerItemsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(userManager);

            _context = context;
            _userManager = userManager;
        }

        private string? _userId => _userManager.GetUserId(User);

        // DELETE: api/TaskerItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskerItem(int id)
        {
            var taskerItem = await _context.TaskerItems.FirstOrDefaultAsync(ti => ti.Id == id);

            if (taskerItem == null)
            {
                return NotFound();
            }

            _context.TaskerItems.Remove(taskerItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/TaskerItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskerItemDto>> GetTaskerItem(int id)
        {
            var taskerItem = await _context.TaskerItems.FirstOrDefaultAsync(ti => ti.Id == id);

            if (taskerItem == null)
            {
                return NotFound();
            }

            return taskerItem.ToDto();
        }

        // GET: api/TaskerItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskerItemDto>>> GetTaskerItems()
        {
            return await _context.TaskerItems.Select(ti => ti.ToDto()).ToListAsync();
        }

        // POST: api/TaskerItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskerItem>> PostTaskerItem(TaskerItemDto dto)
        {
            if (string.IsNullOrEmpty(_userId))
            {
                return Unauthorized();
            }

            var taskerItem = dto.ToModel();
            taskerItem.UserId = _userId;

            _context.TaskerItems.Add(taskerItem);
            await _context.SaveChangesAsync();

            dto = taskerItem.ToDto();

            return CreatedAtAction("GetTaskerItem", new { id = dto.Id }, dto);
        }

        // PUT: api/TaskerItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskerItem(int id, TaskerItemDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var taskerItem = await _context.TaskerItems.FirstOrDefaultAsync(ti => ti.Id == id);

            if (taskerItem == null)
            {
                return NotFound();
            }

            taskerItem.Name = dto.Name;
            taskerItem.Completed = dto.Completed;
            _context.Entry(taskerItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskerItemExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        private bool TaskerItemExists(int id)
        {
            return _context.TaskerItems.Any(e => e.Id == id);
        }
    }
}
