using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.DTOs;

public record UserDTO(string Id, string UserName, string Email,
    string ProfileImagePath);
