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
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<EpicController> _logger;

        public EpicController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<EpicController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
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
            return Ok(epic);
        }
    }

    public class EpicCreateDto
    {
        public string? PortfolioContext { get; set; }
        public string? BusinessOutcome { get; set; }
        public string? Stakeholder { get; set; }
        public string? ProblemStatement { get; set; }
    }
}
