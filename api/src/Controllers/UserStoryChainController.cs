using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using AzDoCopilotSK.Models;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStoryChainController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<UserStoryChainController> _logger;

        public UserStoryChainController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<UserStoryChainController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UserStoryChainCreateDto dto)
        {
            _logger.LogInformation("Creating user story chain.");
            // 1. Create User Story
            var userStorySkill = new UserStorySkill(_kernel, _promptsFactory);
            var userStory = await userStorySkill.GetUserStory(dto.UserStoryStyle!, dto.UserStoryDescription!, dto.ProjectContext, dto.PersonaName);
            if (userStory == null)
                return BadRequest("Failed to create user story");

            // 2. Create Task(s) for the User Story
            var taskSkill = new TaskSkill(_kernel, _promptsFactory);
            var tasks = new List<TaskItem>();
            foreach (var taskGoal in dto.TaskGoals ?? new List<string>())
            {
                var task = await taskSkill.GetTask(userStory.Description!, taskGoal);
                if (task != null && dto.SprintPoints <= 11)
                {
                    tasks.Add(task);
                }
                else if (task != null && dto.SprintPoints > 11)
                {
                    // If sprint points > 11, create a Feature instead
                    var featureSkill = new FeatureSkill(_kernel, _promptsFactory);
                    var feature = await featureSkill.GetFeature(userStory.Description!, dto.PersonaName!, taskGoal);
                    return Ok(new { userStory, feature });
                }
            }
            return Ok(new { userStory, tasks });
        }
    }
}
