using Xunit;
using Moq;
using AzDoCopilotSK.Controllers;
using AzDoCopilotSK.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzDoCopilotSK.SK;

namespace AzDoCopilotSK.Tests
{
    public class UserStoryChainControllerTests
    {
        private static Kernel CreateKernel() => new Kernel();
        private static FunctionResult CreateFunctionResult(string json)
        {
            var mock = new Mock<FunctionResult>("test", "test");
            mock.Setup(f => f.ToString()).Returns(json);
            return mock.Object;
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenValid()
        {
            var kernel = CreateKernel();
            var mockFactory = new Mock<IPromptsFactory>();
            var mockLogger = new Mock<ILogger<UserStoryChainController>>();
            var mockFunction = new Mock<KernelFunction>();
            mockFactory.Setup(f => f.GetUserStoryPrompt(It.IsAny<string>())).Returns(mockFunction.Object);
            mockFactory.Setup(f => f.GetTaskPrompt()).Returns(mockFunction.Object);
            mockFunction.Setup(f => f.InvokeAsync(It.IsAny<Kernel>(), It.IsAny<KernelArguments>(), default))
                .ReturnsAsync(CreateFunctionResult("{\"id\":\"1\",\"title\":\"Test\",\"description\":\"desc\",\"acceptanceCriteria\":\"criteria\"}"));
            var controller = new UserStoryChainController(kernel, mockFactory.Object, mockLogger.Object);
            var dto = new UserStoryChainCreateDto { UserStoryDescription = "desc", UserStoryStyle = "classic", TaskGoals = new List<string> { "goal1" }, SprintPoints = 5 };
            var result = await controller.Create(dto);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
