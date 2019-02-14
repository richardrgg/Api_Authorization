using Api_Authorization.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Authorization.Services
{
    public interface IUserService
    {
        void Create(User user);
        User ValidateUsername(string username);
    }

    public class UserService : IUserService
    {
        private readonly ApiContext _context;

        public UserService(ApiContext context) => _context = context;

        public void Create(User user)
        {
            _context.Add(user);
        }

        public User ValidateUsername(string username)
        {
            return _context.Users.SingleOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
        }
    }
}
