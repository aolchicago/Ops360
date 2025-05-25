using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using AzDoCopilotSK.Models;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<TaskController> _logger;

        public TaskController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<TaskController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem?>> Create([FromBody] TaskCreateDto taskCreateDto)
        {
            _logger.LogInformation("Creating task.");
            var taskSkill = new TaskSkill(_kernel, _promptsFactory);
            var task = await taskSkill.GetTask(
                taskCreateDto.UserStoryContext!,
                taskCreateDto.TaskGoal!
            );
            return Ok(task);
        }
    }

    public class TaskCreateDto
    {
        public string? UserStoryContext { get; set; }
        public string? TaskGoal { get; set; }
        public int SprintPoints { get; set; }
    }
}
