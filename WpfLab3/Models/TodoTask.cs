using System;

namespace WpfLab3.Models
{
    public class TodoTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public TodoTask()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Description = null;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }
    }
}
