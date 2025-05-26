namespace AzDoCopilotSK.Models
{
    public class TaskCreateDto
    {
        public string? UserStoryContext { get; set; }
        public string? TaskGoal { get; set; }
        public int SprintPoints { get; set; }
    }
}
