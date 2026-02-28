using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.Users.Dal.Entities
{
    internal class UserData
    {
        public Guid UserId { get; set; }
        public String UserName { get; set; } = null!;
        public String UserEmail { get; set; } = null!;
        public String? UserEmailCode { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
