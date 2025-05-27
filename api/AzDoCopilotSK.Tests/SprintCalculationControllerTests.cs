using Xunit;
using AzDoCopilotSK.Controllers;
using AzDoCopilotSK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AzDoCopilotSK.Tests
{
    public class SprintCalculationControllerTests
    {
        [Fact]
        public void Calculate_AssignsIssuesToSprints()
        {
            var controller = new SprintCalculationController();
            var issues = new List<Issue> {
                new Issue { Title = "A", StoryPoints = 3 },
                new Issue { Title = "B", StoryPoints = 5 },
                new Issue { Title = "C", StoryPoints = 8 }
            };
            var dto = new SprintCalculationRequestDto { Issues = issues, Velocity = 8, StartDate = DateTime.Today, SprintLengthDays = 14 };
            var result = controller.Calculate(dto);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<SprintCalculationResultDto>(ok.Value);
            Assert.True(data.Sprints.Count > 0);
        }
    }
}
