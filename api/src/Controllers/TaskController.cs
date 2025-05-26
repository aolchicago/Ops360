using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using AzDoCopilotSK.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TaskController : ControllerBase
    {
        private static readonly List<TaskItem> _tasks = new();
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<TaskController> _logger;

        public TaskController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<TaskController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetAll() => Ok(_tasks);

        [HttpGet("{id}")]
        public ActionResult<TaskItem> GetById(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return task == null ? NotFound() : Ok(task);
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
            if (task != null) _tasks.Add(task);
            return Ok(task);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] TaskItem update)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            task.Title = update.Title;
            task.Description = update.Description;
            task.AcceptanceCriteria = update.AcceptanceCriteria;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            _tasks.Remove(task);
            return NoContent();
        }
    }
}

