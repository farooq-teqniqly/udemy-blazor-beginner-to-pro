using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiWithAuth.Models
{
    public class TaskerItem
    {
        public bool Completed { get; set; } = false;
        public int Id { get; set; }

        [Required(ErrorMessage = "Every task must have a name.")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }

    public class TaskerItemDto
    {
        public bool Completed { get; set; } = false;
        public int Id { get; set; }

        [Required(ErrorMessage = "Every task must have a name.")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
