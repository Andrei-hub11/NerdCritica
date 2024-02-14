using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Infrastructure.UserContext
{
    public class IUserContext
    {
        bool IsAuthenticated { get; }
        Guid UserId { get; }
    }
}
