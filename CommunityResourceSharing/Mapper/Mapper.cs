using AutoMapper;
using CommunityResourceSharing.DTOs;
using CommunityResourceSharing.Models;

namespace CommunityResourceSharing.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Resource , ResourceDto>().ReverseMap();
        }
    }
}
