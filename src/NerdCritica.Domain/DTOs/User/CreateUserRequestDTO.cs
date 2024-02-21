﻿
namespace NerdCritica.Domain.DTOs.User;

public record CreateUserRequestDTO(string UserName, string Email, string Password, 
    List<string> Roles, byte[]? ProfileImage = null);