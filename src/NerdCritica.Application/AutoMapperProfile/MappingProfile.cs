using AutoMapper;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;

namespace NerdCritica.Api.AutoMapperProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserMapping, ProfileUserResponseDTO>();
        //CreateMap<ApplicationUser, UserDTO>();
        //CreateMap<UserUpdateModel, UserDTO>();
        //CreateMap<TicketModel, TicketDTO>();
        //CreateMap<TicketDTO, TicketModel>();
    }
}
