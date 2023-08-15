using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Repository
{
    public class MessageWithTagsRepository : RepositoryBase<MessageWithTags>, IMessageWithTagsRepository
    {
        public MessageWithTagsRepository(ChatContext chatContext)
            : base(chatContext) { }

        public void AttachTagsToMessage(Guid messageId, Tag tag)
        {
            var message = new MessageWithTags
            {
                Id = Guid.NewGuid()
            };

            message.MessageId = messageId;    
            message.TagId = tag.Id;
        
            Create(message);
        }

        public void CreateMessageWithoutTags(Guid messageId)
        {
            var message = new MessageWithTags
            {
                Id = Guid.NewGuid()
            };

            message.MessageId = messageId;
            message.TagId = null;

            Create(message);
        }

        public bool HasTaggedMessages(int tagId)
        {
            var taggedMessages = FindByCondition(m => m.TagId.Equals(tagId), false).FirstOrDefault();
            if (taggedMessages == null)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<MessageWithTagsDTO> GetMessages(List<int> tagIds, bool trackChanges)
        {
            if (tagIds.Count == 0)
            {
                var messagesWithNoTags = FindByCondition(m => m.TagId == null, trackChanges)
                    .Include(m => m.message)
                    .ToList();

                var groupedMessages = messagesWithNoTags
                    .GroupBy(mt => mt.MessageId)
                    .Select(group => new MessageWithTagsDTO
                    {
                        Id = group.First().Id,
                        MessageId = group.Key,
                        message = group.First().message,
                        tag = group.Select(g => g.tag).ToList()
                    })
                    .GroupBy(dto => dto.MessageId)
                    .Select(group => group.First())
                    .OrderBy(m => m.message.Timestamp);

                return groupedMessages;
            }
            else
            {
                var messagesWithTags = FindByCondition(m => tagIds.Contains((int)m.TagId), trackChanges)
                    .Include(m => m.message)
                    .Include(m => m.tag)
                    .ToList();

                var groupedMessages = messagesWithTags
                    .GroupBy(mt => mt.MessageId)
                    .Select(group => new MessageWithTagsDTO
                    {
                        Id = group.First().Id,
                        MessageId = group.Key,
                        message = group.First().message,
                        tag = group.Select(g => g.tag).ToList()
                    })
                    .GroupBy(dto => dto.MessageId)
                    .Select(group => group.First())
                    .OrderBy(m => m.message.Timestamp);

                return groupedMessages;
            }
        }

    }
}