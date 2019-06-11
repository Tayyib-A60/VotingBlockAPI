using API.Controllers.DTOs;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile () {
            // Domain to API Resource
            CreateMap<User, UserDTO> ();
            CreateMap<Election, ElectionDTO> ();

            // API to Domain Resource
            CreateMap<UserDTO, User> ();
            CreateMap<ElectionDTO, Election> ();
        }
    }
}