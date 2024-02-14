using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.Entities
{
    public class MoviePost
    {
      public Guid MoviePostId { get; private set; }
      public string MovieTitle { get; private set; }
      public string MovieDescription { get; private set; }
    }
}
