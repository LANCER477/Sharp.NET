using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.Users.Dal.Entities
{
    internal class UserAccess   
    {
        public Guid AccessId { get; set; }
        public Guid UserId { get; set; } 
        public Guid AccessLogin { get; set; }
        public Guid RoleId { get; set; }
        public String AccessSalt { get; set; } = null!;
        public String AccessDk { get; set; } = null!;
    }
}
