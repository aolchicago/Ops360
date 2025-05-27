using Xunit;
using Moq;
using AzDoCopilotSK.Controllers;
using AzDoCopilotSK.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using AzDoCopilotSK.SK;

namespace AzDoCopilotSK.Tests
{
    public class FeatureControllerTests
    {
        private static Kernel CreateKernel() => new Kernel();
        private static FunctionResult CreateFunctionResult(string json)
        {
            var mock = new Mock<FunctionResult>("test", "test");
            mock.Setup(f => f.ToString()).Returns(json);
            return mock.Object;
        }

        [Fact]
        public async Task Create_ReturnsFeature_WhenValid()
        {
            var kernel = CreateKernel();
            var mockFactory = new Mock<IPromptsFactory>();
            var mockLogger = new Mock<ILogger<FeatureController>>();
            var mockFunction = new Mock<KernelFunction>();
            mockFactory.Setup(f => f.GetFeaturePrompt()).Returns(mockFunction.Object);
            mockFunction.Setup(f => f.InvokeAsync(It.IsAny<Kernel>(), It.IsAny<KernelArguments>(), default))
                .ReturnsAsync(CreateFunctionResult("{\"id\":\"1\",\"title\":\"Test\",\"description\":\"desc\",\"acceptanceCriteria\":\"criteria\"}"));
            var controller = new FeatureController(kernel, mockFactory.Object, mockLogger.Object);
            var dto = new FeatureCreateDto { EpicContext = "epic", Stakeholder = "stake", FeatureGoal = "goal" };
            var result = await controller.Create(dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenMissing()
        {
            var kernel = CreateKernel();
            var controller = new FeatureController(kernel, new Mock<IPromptsFactory>().Object, new Mock<ILogger<FeatureController>>().Object);
            var result = controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
