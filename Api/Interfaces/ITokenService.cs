using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Model;

namespace Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
