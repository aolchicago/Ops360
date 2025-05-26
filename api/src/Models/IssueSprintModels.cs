using System.ComponentModel.DataAnnotations;

namespace AzDoCopilotSK.Models
{
    public class Issue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Medium";
        public int StoryPoints { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Sprint
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Velocity { get; set; }
        public List<Issue> Issues { get; set; } = new();
        public List<Epic>? Epics { get; set; }
        public List<Feature>? Features { get; set; }
        public List<UserStory>? UserStories { get; set; }
        public List<TaskItem>? Tasks { get; set; }
    }

    public class SprintCalculationRequestDto
    {
        public List<Issue> Issues { get; set; } = new();
        public List<Epic>? Epics { get; set; }
        public List<Feature>? Features { get; set; }
        public List<UserStory>? UserStories { get; set; }
        public List<TaskItem>? Tasks { get; set; }
        public int Velocity { get; set; }
        public DateTime StartDate { get; set; }
        public int SprintLengthDays { get; set; } = 14;
    }

    public class SprintCalculationResultDto
    {
        public List<Sprint> Sprints { get; set; } = new();
        public List<Issue> UnassignedIssues { get; set; } = new();
        public List<Epic>? UnassignedEpics { get; set; }
        public List<Feature>? UnassignedFeatures { get; set; }
        public List<UserStory>? UnassignedUserStories { get; set; }
        public List<TaskItem>? UnassignedTasks { get; set; }
    }
}
