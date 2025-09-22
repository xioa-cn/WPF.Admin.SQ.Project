﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Admin.Models.Models
{
    public class LoginAuthManager
    {
        public LoginAuth Login { get; set; }

        public static LoginAuthManager Create(LoginAuth login)
        {
            return new LoginAuthManager
            {
                Login = login
            };
        }
    }
}
