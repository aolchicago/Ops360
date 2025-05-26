using AzDoCopilotSK.Extensions;
using AzDoCopilotSK.Models;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace AzDoCopilotSK.SK
{
    public class TaskSkill
    {
        private readonly IPromptsFactory _promptsFactory;
        private readonly Kernel _kernel;

        public TaskSkill(Kernel kernel, IPromptsFactory promptsFactory)
        {
            _promptsFactory = promptsFactory;
            _kernel = kernel;
        }

        public async Task<TaskItem?> GetTask(string userStoryContext, string taskGoal)
        {
            var createTask = _promptsFactory.GetTaskPrompt();

            var context = new KernelArguments
            {
                { "UserStoryContext", userStoryContext },
                { "TaskGoal", taskGoal }
            };

            var result = await createTask.InvokeAsync(_kernel, context);

            if (!result.ToString().IsValidJson())
            {
                throw new Exception("Invalid prompt result, not valid json");
            }

            var task = JsonSerializer.Deserialize<TaskItem>(result.ToString(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return task;
        }
    }
}
