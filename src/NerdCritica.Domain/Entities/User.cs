using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string ProfileImagePath { get; private set; } = string.Empty;
    public byte[] ProfileImage { get; private set; } = new byte[0];
   
}
