using AutoMapper;
using Chat.Hubs;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        IMapper _mapper;
        IRepositoryManager _repositoryManager;

        public MessagesController(IMapper mapper, IRepositoryManager repositoryManager)
        {
            _mapper = mapper;
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public IActionResult GetMessages([FromQuery] List<int> tagIds)
        {
            var tagList = new List<string>();

            foreach (var tagId in tagIds)
            {
                var tag = _repositoryManager.Tag.FindTagById(tagId, false);

                tagList.Add(tag.Text);
            }

            var messagesWithTags = _repositoryManager.Message.GetAllMessages(tagList, false);

            return Ok(messagesWithTags);
        }
    }
}
