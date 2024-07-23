using AutoMapper;
using NerdCritica.Contracts.DTOs.MappingsDapper;
using NerdCritica.Contracts.DTOs.Movie;
using NerdCritica.Contracts.DTOs.User;

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
