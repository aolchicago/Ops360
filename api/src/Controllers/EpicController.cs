using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using AzDoCopilotSK.Models;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpicController : ControllerBase
    {
        private static readonly List<Epic> _epics = new();
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<EpicController> _logger;

        public EpicController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<EpicController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Epic>> GetAll() => Ok(_epics);

        [HttpGet("{id}")]
        public ActionResult<Epic> GetById(Guid id)
        {
            var epic = _epics.FirstOrDefault(e => e.Id == id);
            return epic == null ? NotFound() : Ok(epic);
        }

        [HttpPost]
        public async Task<ActionResult<Epic?>> Create([FromBody] EpicCreateDto epicCreateDto)
        {
            _logger.LogInformation("Creating epic.");
            var epicSkill = new EpicSkill(_kernel, _promptsFactory);
            var epic = await epicSkill.GetEpic(
                epicCreateDto.PortfolioContext!,
                epicCreateDto.BusinessOutcome!,
                epicCreateDto.Stakeholder!,
                epicCreateDto.ProblemStatement!
            );
            if (epic != null) _epics.Add(epic);
            return Ok(epic);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] Epic update)
        {
            var epic = _epics.FirstOrDefault(e => e.Id == id);
            if (epic == null) return NotFound();
            epic.Title = update.Title;
            epic.Description = update.Description;
            epic.AcceptanceCriteria = update.AcceptanceCriteria;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var epic = _epics.FirstOrDefault(e => e.Id == id);
            if (epic == null) return NotFound();
            _epics.Remove(epic);
            return NoContent();
        }
    }
}
