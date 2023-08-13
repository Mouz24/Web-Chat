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
        public IActionResult GetMessages([FromQuery] List<int> ids)
        {
            var messagesWithTags = _repositoryManager.MessageWithTags.GetMessages(ids, false);

            return Ok(messagesWithTags);
        }

        [HttpPost(Name = "AddTagsToMessage")]
        public async Task<IActionResult> AddMessage(MessageToAddDTO messageToAdd)
        {
            var message = _mapper.Map<Message>(messageToAdd);

            _repositoryManager.Message.AddMessage(message);

            if (messageToAdd.Ids.Count == 0)
            {
                _repositoryManager.MessageWithTags.CreateMessageWithoutTags(message.Id);
            }

            foreach (var tagId in messageToAdd.Ids)
            {
                var tag = _repositoryManager.Tag.FindTagById(tagId, false);

                _repositoryManager.MessageWithTags.AttachTagsToMessage(message.Id, tag);
            }

            await _repositoryManager.SaveAsync();

            return CreatedAtRoute("AddTagsToMessage", message);
        }
    }
}
