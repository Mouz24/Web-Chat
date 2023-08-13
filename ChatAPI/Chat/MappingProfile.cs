using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace Chat
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TagDTO, Tag>();
            CreateMap<MessageToAddDTO, Message>();
            CreateMap<MessageToAddDTO, MessageWithTags>();
        }
    }
}
