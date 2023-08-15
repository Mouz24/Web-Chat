using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Hubs
{
    public class ChatHub : Hub
    {
        IMapper _mapper;
        IRepositoryManager _repositoryManager;

        public ChatHub(IMapper mapper, IRepositoryManager repositoryManager)
        {
            _mapper = mapper;
            _repositoryManager = repositoryManager;
        }

        public async Task SendMessage(MessageToAddDTO messageToAdd)
        {
            if (messageToAdd.Text.IsNullOrEmpty())
            {
                return;
            }

            var message = _mapper.Map<Message>(messageToAdd);

            _repositoryManager.Message.AddMessage(message);

            await _repositoryManager.SaveAsync();

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
