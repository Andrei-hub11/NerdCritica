﻿using NerdCritica.Domain.DTOs.MappingsDapper;

namespace NerdCritica.Application.Services.Token;

public interface ITokenService
{
    string GeneratePasswordResetToken(UserMapping user);
    bool ValidatePasswordResetToken(string token);
}
