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
        void CreateUserDemo();
    }

    public class UserService : IUserService
    {
        private readonly ApiContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(ApiContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void Create(User user)
        {
            _context.Add(user);
        }

        public void CreateUserDemo()
        {
            var user = new User
            {
                Name = "Richard",
                Password = _passwordHasher.GenerateIdentityV3Hash("123456"),
                Username = "richard123",
                Role = "Admin"
            };

            _context.Users.Add(user);
            _context.SaveChangesAsync();
        }

        public User ValidateUsername(string username)
        {
            return _context.Users.SingleOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
        }
    }
}
