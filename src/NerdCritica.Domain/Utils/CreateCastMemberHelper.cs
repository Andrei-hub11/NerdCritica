


using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Domain.Utils;

public class CastMemberHelper
{

    public static List<CastMember> GetCast(List<CastMemberRequestDTO> cast, 
        Dictionary<string, CastImages> castImagePaths)
    {
        if (cast == null || cast.Count == 0)
            throw new ArgumentException("A lista de elenco (cast) não pode ser nula ou vazia.", nameof(cast));

        if (castImagePaths == null || castImagePaths.Count == 0)
            throw new ArgumentException("O dicionário de caminhos de imagens do elenco (castImagePaths) não pode ser nulo ou vazio.", nameof(castImagePaths));

        List<CastMember> castMembers = new();

        foreach (var item in cast)
        {
            if (castImagePaths.ContainsKey(item.MemberName))
            {
                var newCast = CastMember.Create(item.MemberName, item.CharacterName,
                    castImagePaths[item.MemberName].CastMemberImageBytes,
                    castImagePaths[item.MemberName].CastMemberImagePath, item.RoleType);

                if (newCast.IsFailure)
                {
                    var errorMessages = newCast.Errors.Select(error => error.Description).ToList();
                    throw new ValidationException("Os campos de atualização de elenco não foram preenchidos corretamente.",
                        errorMessages);
                }

                castMembers.Add(newCast.Value);
            }
        }

        return castMembers;
    }

    public static List<CastMember> GetCastUpdated(List<UpdateCastMemberRequestDTO> cast,
       Dictionary<string, CastImages> castImagePaths)
    {
        if(cast == null || cast.Count == 0)
            throw new ArgumentException("A lista de elenco (cast) não pode ser nula ou vazia.", nameof(cast));

        if (castImagePaths == null || castImagePaths.Count == 0)
            throw new ArgumentException("O dicionário de caminhos de imagens do elenco (castImagePaths) não pode ser nulo ou vazio.", nameof(castImagePaths));

        List<CastMember> castMembers = new();

        foreach (var item in cast)
        {
            if (castImagePaths.ContainsKey(item.MemberName))
            {
                var updatedCast = CastMember.From(item.MemberName, item.CharacterName,
                    castImagePaths[item.MemberName].CastMemberImageBytes,
                    castImagePaths[item.MemberName].CastMemberImagePath, item.RoleType, item.CastMemberId);

                if (updatedCast.IsFailure)
                {
                    var errorMessages = updatedCast.Errors.Select(error => error.Description).ToList();
                    throw new ValidationException("Os campos de atualização de elenco não foram preenchidos corretamente.",
                        errorMessages);
                }

                castMembers.Add(updatedCast.Value);
            }
        }

        return castMembers;
    }

  }
