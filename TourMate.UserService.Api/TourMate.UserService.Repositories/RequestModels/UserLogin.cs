﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.RequestModels
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
