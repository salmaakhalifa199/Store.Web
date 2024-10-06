

using Microsoft.AspNetCore.Identity;
using System.Net.Sockets;

namespace Store.Data.Entities.IdentityEntities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
    }
}
