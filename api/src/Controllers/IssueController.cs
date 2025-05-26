using AzDoCopilotSK.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private static readonly List<Issue> _issues = new();

        [HttpGet]
        public ActionResult<IEnumerable<Issue>> GetAll() => Ok(_issues);

        [HttpGet("{id}")]
        public ActionResult<Issue> GetById(Guid id)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == id);
            return issue == null ? NotFound() : Ok(issue);
        }

        [HttpPost]
        public ActionResult<Issue> Create([FromBody] Issue issue)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _issues.Add(issue);
            return Ok(issue);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] Issue update)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == id);
            if (issue == null) return NotFound();
            issue.Title = update.Title;
            issue.Description = update.Description;
            issue.Status = update.Status;
            issue.Priority = update.Priority;
            issue.StoryPoints = update.StoryPoints;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == id);
            if (issue == null) return NotFound();
            _issues.Remove(issue);
            return NoContent();
        }
    }
}
