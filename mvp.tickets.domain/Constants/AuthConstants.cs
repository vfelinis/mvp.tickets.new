using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvp.tickets.domain.Constants
{
    public static class AuthConstants
    {
        public const string AdminClaim = "AdminClaim";
        public const string EmployeeClaim = "EmployeeClaim";
        public const string UserClaim = "UserClaim";
        public const string FirebaseIdClaim = "FirebaseIdClaim";
        public const string AdminPolicy = "AdminPolicy";
        public const string EmployeePolicy = "EmployeePolicy";
        public const string UserPolicy = "UserPolicy";
    }
}
