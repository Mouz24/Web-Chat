using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private ChatContext _chatContext;

        private IMessageRepository _MessageRepository;

        private IMessageWithTagsRepository _MessageWithTagsRepository;

        private ITagRepository _TagRepository;

        public RepositoryManager(ChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        public IMessageRepository Message
        {
            get 
            {   if (_MessageRepository == null)
                {
                    _MessageRepository = new MessageRepository(_chatContext);
                }

                return _MessageRepository; 
            }
        }

        public IMessageWithTagsRepository MessageWithTags
        {
            get
            {
                if (_MessageWithTagsRepository == null)
                {
                    _MessageWithTagsRepository = new MessageWithTagsRepository(_chatContext);
                }

                return _MessageWithTagsRepository;
            }
        }

        public ITagRepository Tag
        {
            get
            {
                if (_TagRepository == null)
                {
                    _TagRepository = new TagRepository(_chatContext);
                }

                return _TagRepository;
            }
        }

        public async Task SaveAsync() => await _chatContext.SaveChangesAsync();
    }
}
