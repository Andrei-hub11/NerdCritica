﻿namespace NerdCritica.Domain.DTOs.User;

public record ProfileUserResponseDTO(Guid Id, string IdentityUserId, string UserName, string Email,
    string ProfileImagePath);
