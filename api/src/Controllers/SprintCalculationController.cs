using AzDoCopilotSK.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintCalculationController : ControllerBase
    {
        [HttpPost]
        public ActionResult<SprintCalculationResultDto> Calculate([FromBody] SprintCalculationRequestDto dto)
        {
            var sprints = new List<Sprint>();
            var unassigned = new List<Issue>(dto.Issues);
            int sprintNum = 1;
            DateTime sprintStart = dto.StartDate;

            while (unassigned.Any())
            {
                var sprint = new Sprint
                {
                    Name = $"Sprint {sprintNum}",
                    StartDate = sprintStart,
                    EndDate = sprintStart.AddDays(dto.SprintLengthDays - 1),
                    Velocity = dto.Velocity
                };
                int usedPoints = 0;
                foreach (var issue in unassigned.ToList())
                {
                    if (usedPoints + issue.StoryPoints <= dto.Velocity)
                    {
                        sprint.Issues.Add(issue);
                        usedPoints += issue.StoryPoints;
                        unassigned.Remove(issue);
                    }
                }
                sprints.Add(sprint);
                sprintNum++;
                sprintStart = sprintStart.AddDays(dto.SprintLengthDays);
                if (usedPoints == 0) break; // Prevent infinite loop if no issues fit
            }
            return Ok(new SprintCalculationResultDto { Sprints = sprints, UnassignedIssues = unassigned });
        }
    }
}
