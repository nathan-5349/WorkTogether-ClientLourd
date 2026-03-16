using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTogether_ClientLourd.EF.Entities;

namespace WorkTogether_ClientLourd.WPF
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }
        public static string? Role { get; set; }

        public static bool IsAdmin => Role == "ROLE_ADMIN";
        public static bool IsAccountant => Role == "ROLE_ACCOUNTANT";
        public static bool IsSupport => Role == "ROLE_SUPPORT";
        public static bool IsTechnician => Role == "ROLE_TECHNICIAN";

        public static void Clear()
        {
            CurrentUser = null;
            Role = null;
        }
    }
}
