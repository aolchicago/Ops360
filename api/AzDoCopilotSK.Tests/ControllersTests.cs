using Xunit;
using Moq;
using AzDoCopilotSK.Controllers;
using AzDoCopilotSK.Models;
using AzDoCopilotSK.SK;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AzDoCopilotSK.Tests
{
    public partial class EpicControllerTests
    {
        [Fact]
        public async Task Create_ReturnsEpic_WhenValid1()
        {
            var mockKernel = new Mock<Kernel>();
            var mockFactory = new Mock<IPromptsFactory>();
            var mockLogger = new Mock<ILogger<EpicController>>();
            var mockSkill = new Mock<EpicSkill>(mockKernel.Object, mockFactory.Object);
            var controller = new EpicController(mockKernel.Object, mockFactory.Object, mockLogger.Object);
            var dto = new EpicCreateDto { PortfolioContext = "ctx", BusinessOutcome = "outcome", Stakeholder = "stake", ProblemStatement = "problem" };
            // Simulate EpicSkill
            mockFactory.Setup(f => f.GetEpicPrompt()).Returns(() => null!);
            // Act
            var result = await controller.Create(dto);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenMissing1()
        {
            var controller = new EpicController(new Mock<Kernel>().Object, new Mock<IPromptsFactory>().Object, new Mock<ILogger<EpicController>>().Object);
            var result = controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
