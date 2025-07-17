using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.RequestModels
{
    public class RoleCreate
    {
        public string RoleName { get; set; }
        public Role Convert() => new Role { RoleName = RoleName };

    }
}
