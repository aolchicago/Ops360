using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using AzDoCopilotSK.Models;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<FeatureController> _logger;

        public FeatureController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<FeatureController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Feature?>> Create([FromBody] FeatureCreateDto featureCreateDto)
        {
            _logger.LogInformation("Creating feature.");
            var featureSkill = new FeatureSkill(_kernel, _promptsFactory);
            var feature = await featureSkill.GetFeature(
                featureCreateDto.EpicContext!,
                featureCreateDto.Stakeholder!,
                featureCreateDto.FeatureGoal!
            );
            return Ok(feature);
        }
    }

    public class FeatureCreateDto
    {
        public string? EpicContext { get; set; }
        public string? Stakeholder { get; set; }
        public string? FeatureGoal { get; set; }
    }
}
