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
            var unassignedEpics = dto.Epics != null ? new List<Epic>(dto.Epics) : new List<Epic>();
            var unassignedFeatures = dto.Features != null ? new List<Feature>(dto.Features) : new List<Feature>();
            var unassignedUserStories = dto.UserStories != null ? new List<UserStory>(dto.UserStories) : new List<UserStory>();
            var unassignedTasks = dto.Tasks != null ? new List<TaskItem>(dto.Tasks) : new List<TaskItem>();
            var unassignedIssues = new List<Issue>(dto.Issues);
            int sprintNum = 1;
            DateTime sprintStart = dto.StartDate;

            // Helper to assign items to a sprint by story points and velocity
            void AssignItemsByPoints<T>(List<T> items, int velocity, List<T> assigned, Func<T, int> getPoints)
            {
                int usedPoints = 0;
                for (int i = 0; i < items.Count;)
                {
                    var item = items[i];
                    int points = getPoints(item);
                    if (usedPoints + points <= velocity)
                    {
                        assigned.Add(item);
                        usedPoints += points;
                        items.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            while (unassignedEpics.Any() || unassignedFeatures.Any() || unassignedUserStories.Any() || unassignedTasks.Any() || unassignedIssues.Any())
            {
                var sprint = new Sprint
                {
                    Name = $"Sprint {sprintNum}",
                    StartDate = sprintStart,
                    EndDate = sprintStart.AddDays(dto.SprintLengthDays - 1),
                    Velocity = dto.Velocity,
                    Epics = new(),
                    Features = new(),
                    UserStories = new(),
                    Tasks = new(),
                    Issues = new()
                };
                int usedPoints = 0;
                // Assign Epics
                AssignItemsByPoints(unassignedEpics, dto.Velocity, sprint.Epics, e => e is Epic epic ? (epic.AcceptanceCriteria?.Length ?? 1) : 1);
                usedPoints += sprint.Epics.Sum(e => e.AcceptanceCriteria?.Length ?? 1);
                // Assign Features
                AssignItemsByPoints(unassignedFeatures, dto.Velocity - usedPoints, sprint.Features, f => f is Feature feature ? (feature.AcceptanceCriteria?.Length ?? 1) : 1);
                usedPoints += sprint.Features.Sum(f => f.AcceptanceCriteria?.Length ?? 1);
                // Assign User Stories
                AssignItemsByPoints(unassignedUserStories, dto.Velocity - usedPoints, sprint.UserStories, us => us is UserStory uss ? (uss.AcceptanceCriteria?.Length ?? 1) : 1);
                usedPoints += sprint.UserStories.Sum(us => us.AcceptanceCriteria?.Length ?? 1);
                // Assign Tasks
                AssignItemsByPoints(unassignedTasks, dto.Velocity - usedPoints, sprint.Tasks, t => t is TaskItem ti ? (ti.AcceptanceCriteria?.Length ?? 1) : 1);
                usedPoints += sprint.Tasks.Sum(t => t.AcceptanceCriteria?.Length ?? 1);
                // Assign Issues
                AssignItemsByPoints(unassignedIssues, dto.Velocity - usedPoints, sprint.Issues, i => i.StoryPoints);
                usedPoints += sprint.Issues.Sum(i => i.StoryPoints);
                sprints.Add(sprint);
                sprintNum++;
                sprintStart = sprintStart.AddDays(dto.SprintLengthDays);
                if (usedPoints == 0) break; // Prevent infinite loop if no items fit
            }
            return Ok(new SprintCalculationResultDto
            {
                Sprints = sprints,
                UnassignedEpics = unassignedEpics,
                UnassignedFeatures = unassignedFeatures,
                UnassignedUserStories = unassignedUserStories,
                UnassignedTasks = unassignedTasks,
                UnassignedIssues = unassignedIssues
            });
        }
    }
}
