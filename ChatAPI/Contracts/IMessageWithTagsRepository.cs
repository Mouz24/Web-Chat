using Entities.DTOs;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMessageWithTagsRepository
    {
        IEnumerable<MessageWithTagsDTO> GetMessages(List<int> tagId, bool trackChanges);
        void AttachTagsToMessage(Guid messageId, Tag tag);
        void CreateMessageWithoutTags(Guid messageId);
        public bool HasTaggedMessages(int tagId);
    }
}
