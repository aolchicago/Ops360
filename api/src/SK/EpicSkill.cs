using AzDoCopilotSK.Extensions;
using AzDoCopilotSK.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AzDoCopilotSK.SK
{
    public class EpicSkill
    {
        private readonly IPromptsFactory _promptsFactory;
        private readonly Kernel _kernel;

        public EpicSkill(Kernel kernel, IPromptsFactory promptsFactory)
        {
            _promptsFactory = promptsFactory;
            _kernel = kernel;
        }

        public async Task<Epic?> GetEpic(string portfolioContext, string businessOutcome, string stakeholder, string problemStatement)
        {
            var createEpic = _promptsFactory.GetEpicPrompt();

            var context = new KernelArguments
            {
                { "PortfolioContext", portfolioContext },
                { "BusinessOutcome", businessOutcome },
                { "Stakeholder", stakeholder },
                { "ProblemStatement", problemStatement }
            };

            var result = await createEpic.InvokeAsync(_kernel, context);

            if (!result.ToString().IsValidJson())
            {
                throw new Exception("Invalid prompt result, not valid json");
            }

            var epic = JsonSerializer.Deserialize<Epic>(result.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return epic;
        }
    }
}
