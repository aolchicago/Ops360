using AzDoCopilotSK.Extensions;
using AzDoCopilotSK.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AzDoCopilotSK.SK
{
    public class FeatureSkill
    {
        private readonly IPromptsFactory _promptsFactory;
        private readonly Kernel _kernel;

        public FeatureSkill(Kernel kernel, IPromptsFactory promptsFactory)
        {
            _promptsFactory = promptsFactory;
            _kernel = kernel;
        }

        public async Task<Feature?> GetFeature(string epicContext, string stakeholder, string featureGoal)
        {
            var createFeature = _promptsFactory.GetFeaturePrompt();

            var context = new KernelArguments
            {
                { "EpicContext", epicContext },
                { "Stakeholder", stakeholder },
                { "FeatureGoal", featureGoal }
            };

            var result = await createFeature.InvokeAsync(_kernel, context);

            if (!result.ToString().IsValidJson())
            {
                throw new Exception("Invalid prompt result, not valid json");
            }

            var feature = JsonSerializer.Deserialize<Feature>(result.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return feature;
        }
    }
}
