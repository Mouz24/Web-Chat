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

        public IEnumerable<Message> GetAllMessages(List<string> tags, bool trackChanges)
        {
            var query = FindAll(trackChanges).AsEnumerable();

            if (tags != null && tags.Count > 0)
            {
                query = query.Where(m => tags.Any(tag => m.Text.Contains($"#{tag}") || !m.Text.Contains("#")));
            }
            else
            {
                query = query.Where(m => !m.Text.Contains("#"));
            }

            return query.OrderBy(m => m.Timestamp).ToList();
        }
    }
}
