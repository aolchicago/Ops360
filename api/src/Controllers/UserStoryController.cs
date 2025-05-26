// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using System.Text.Json;
using AzDoCopilotSK.Extensions;
using AzDoCopilotSK.Models;

namespace AzDoCopilotSK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStoryController(Kernel kernel, IPromptsFactory skillsFactory, ILogger<UserStoryController> logger) : ControllerBase
    {
        private static readonly List<UserStory> _userStories = [];
        private readonly Kernel _kernel = kernel;
        private readonly IPromptsFactory _promptsFactory = skillsFactory;
        private readonly ILogger<UserStoryController> _logger = logger;

        [HttpGet]
        public ActionResult<IEnumerable<UserStory>> GetAll() => Ok(_userStories);

        [HttpGet("{id}")]
        public ActionResult<UserStory> GetById(Guid id)
        {
            var userStory = _userStories.FirstOrDefault(u => u.Id == id);
            return userStory == null ? NotFound() : Ok(userStory);
        }

        [HttpPost]
        public async Task<ActionResult<UserStory?>> Create(UserStoryCreateDto userStoryCreateDto)
        {
            _logger.LogInformation("Creating user story.");

            var userStorySkill = new UserStorySkill(_kernel, _promptsFactory);

            var userStory = await userStorySkill.GetUserStory(
                userStoryCreateDto.UserStoryStyle,
                userStoryCreateDto.UserStoryDescription!,
                userStoryCreateDto.ProjectContext,
                userStoryCreateDto.PersonaName
            );
            if (userStory != null) _userStories.Add(userStory);
            return Ok(userStory);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, [FromBody] UserStory update)
        {
            var userStory = _userStories.FirstOrDefault(u => u.Id == id);
            if (userStory == null) return NotFound();
            userStory.Title = update.Title;
            userStory.Description = update.Description;
            userStory.AcceptanceCriteria = update.AcceptanceCriteria;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var userStory = _userStories.FirstOrDefault(u => u.Id == id);
            if (userStory == null) return NotFound();
            _userStories.Remove(userStory);
            return NoContent();
        }
    }
}
