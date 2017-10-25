namespace GameStore.App.Services
{
    using Contracts;
    using Data;
    using Data.Models;
    using GameStore.App.Data.Contracts;
    using SimpleMvc.Framework.Attributes;
    using System.Linq;

    public class UserService : IUserService
    {
        [Inject]
        private readonly IRepository repository;

        public bool Create(string email, string password, string name)
        {
            if (this.repository.Retrieve<User>().Any(u => u.Email == email))
            {
                return false;
            }

            var isAdmin = !this.repository.Retrieve<User>().Any();

            var user = new User
            {
                Email = email,
                Name = name,
                Password = password,
                IsAdmin = isAdmin
            };

            this.repository.Add(user);

            return true;
        }

        public bool UserExists(string email, string password)
        {
            var users = repository
                .Retrieve<User>();

            bool userExists = users.Any(u => u.Email == email && u.Password == password);

            return userExists;
        }

        public bool IsAdmin(string email)
        {
            using (GameStoreDbContext db = new GameStoreDbContext())
            {
                return this.repository
                    .Retrieve<User>()
                    .Where(u => u.Email == email)
                    .Select(u => u.IsAdmin)
                    .FirstOrDefault();
            }
        }
    }
}
