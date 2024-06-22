using AutoMapper;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;

namespace NerdCritica.Api.AutoMapperProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FavoriteMovieMapping, FavoriteMovieResponseDTO>();
        CreateMap<UserMapping, ProfileUserResponseDTO>();
        CreateMap<CastMemberMapping, CastMemberResponseDTO>();
        CreateMap<CommentsMapping, CommentsResponseDTO>();
        CreateMap<CommentLikeMapping, CommentLikeResponseDTO>();
        CreateMap<MoviePostMapping, MoviePostResponseDTO>();

        //CreateMap<ApplicationUser, UserDTO>();
        //CreateMap<UserUpdateModel, UserDTO>();
        //CreateMap<TicketModel, TicketDTO>();
        //CreateMap<TicketDTO, TicketModel>();
    }
}
