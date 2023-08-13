using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(ChatContext chatContext)
            : base(chatContext) { }

        public void AddMessage(Message message)
        {
            message.Timestamp = DateTime.UtcNow.AddHours(3);

            Create(message);
        }

        public IEnumerable<Message> GetAllMessages(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(m => m.Timestamp)
            .ToList();
    }
}
