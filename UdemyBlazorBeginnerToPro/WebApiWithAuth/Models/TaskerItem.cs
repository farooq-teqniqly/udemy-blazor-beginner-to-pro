using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiWithAuth.Models
{
    public static class TaskerItemExtensions
    {
        public static TaskerItem ToModel(this TaskerItemDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new TaskerItem
            {
                Completed = dto.Completed,
                Id = dto.Id,
                Name = dto.Name,
            };
        }

        public static TaskerItemDto ToDto(this TaskerItem taskerItem)
        {
            ArgumentNullException.ThrowIfNull(taskerItem);

            return new TaskerItemDto
            {
                Completed = taskerItem.Completed,
                Id = taskerItem.Id,
                Name = taskerItem.Name,
            };
        }
    }

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
