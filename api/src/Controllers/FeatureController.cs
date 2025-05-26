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
        private static readonly List<Feature> _features = new();
        private readonly Kernel _kernel;
        private readonly IPromptsFactory _promptsFactory;
        private readonly ILogger<FeatureController> _logger;

        public FeatureController(Kernel kernel, IPromptsFactory promptsFactory, ILogger<FeatureController> logger)
        {
            _kernel = kernel;
            _promptsFactory = promptsFactory;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Feature>> GetAll() => Ok(_features);

        [HttpGet("{id}")]
        public ActionResult<Feature> GetById(Guid id)
        {
            var feature = _features.FirstOrDefault(f => f.Id == id);
            return feature == null ? NotFound() : Ok(feature);
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
            if (feature != null) _features.Add(feature);
            return Ok(feature);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] Feature update)
        {
            var feature = _features.FirstOrDefault(f => f.Id == id);
            if (feature == null) return NotFound();
            feature.Title = update.Title;
            feature.Description = update.Description;
            feature.AcceptanceCriteria = update.AcceptanceCriteria;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var feature = _features.FirstOrDefault(f => f.Id == id);
            if (feature == null) return NotFound();
            _features.Remove(feature);
            return NoContent();
        }
    }
}
