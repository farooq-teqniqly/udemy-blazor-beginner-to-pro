namespace Tasker.Components.Models
{
    public sealed class TaskerItem
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public bool IsComplete { get; set; }

        [Required(ErrorMessage = "Every task must have a name.")]
        [MinLength(3, ErrorMessage = "Task name must be at least 3 characters long.")]
        [MaxLength(50, ErrorMessage = "Task name can't be longer than 50 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
