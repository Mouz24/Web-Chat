using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAllMessages(List<string> tags, bool trackChanges);
        void AddMessage(Message message);
    }
}
