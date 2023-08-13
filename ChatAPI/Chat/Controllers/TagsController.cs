using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        IRepositoryManager _repositoryManager;
        IMapper _mapper;

        public TagsController(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetTags()
        {
            var tags = _repositoryManager.Tag.GetTags(false);

            return Ok(tags);
        }

        [HttpPost(Name = "AddTag")]
        public async Task<IActionResult> AddTag(TagDTO tagToAdd)
        {
            var tag = _mapper.Map<Tag>(tagToAdd);

            var duplicateTag = _repositoryManager.Tag.FindDuplicateTag(tag.Text, false);
            if (duplicateTag != null)
            {
                return BadRequest("The tag already exists");
            }
            
            tag = _repositoryManager.Tag.AddTag(tag);

            await _repositoryManager.SaveAsync();

            return CreatedAtRoute("AddTag", tag);
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> RemoveTag(int tagId)
        {
            if (_repositoryManager.MessageWithTags.HasTaggedMessages(tagId))
            {
                return BadRequest("The tag can't be deleted becuase it has connected messages.");
            }

            _repositoryManager.Tag.RemoveTag(tagId);

            await _repositoryManager.SaveAsync();
            
            return NoContent();
        }
    }
}
