using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.Users.Dal.Entities
{
    internal class AccessToken
    {
        public Guid TokenId { get; set; }
        public Guid AccessId { get; set; }
        public DateTime TokenIat { get; set; }
        public DateTime? TokenExp { get; set; }
    }
}
