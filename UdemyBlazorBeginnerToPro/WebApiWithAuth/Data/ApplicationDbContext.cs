using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiWithAuth.Models;

namespace WebApiWithAuth.Data
{
    /// <summary>
    /// Provides access to the application's database, including automatic per-user query filtering.
    /// </summary>
    public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes the context with database options and HTTP context access for user-scoped query filtering.
        /// </summary>
        /// <param name="options">The database context options.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context for resolving the authenticated user.</param>
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets or sets the collection of tasker items.
        /// </summary>
        public DbSet<TaskerItem> TaskerItems { get; set; }

        /// <summary>
        /// Gets the current authenticated user's identifier from the HTTP context, or <see langword="null"/> if unavailable.
        /// </summary>
        private string? CurrentUserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskerItem>().HasQueryFilter(ti => ti.UserId == CurrentUserId);
        }
    }
}
