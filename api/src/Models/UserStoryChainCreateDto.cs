namespace AzDoCopilotSK.Models
{
    public class UserStoryChainCreateDto
    {
        public string? ProjectContext { get; set; }
        public string? UserStoryDescription { get; set; }
        public string? PersonaName { get; set; }
        public string UserStoryStyle { get; set; } = "classic";
        public List<string>? TaskGoals { get; set; }
        public int SprintPoints { get; set; }
    }
}
