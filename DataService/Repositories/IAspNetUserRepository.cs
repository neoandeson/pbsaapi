using DataService.Infrastructure;
using DataService.Models;

namespace DataService.Repositories
{
    public interface IAspNetUserRepository : IRepository<AspNetUsers>
    {
        AspNetUsers RegisterUser(string username, string fullName, string phone);
    }

    public class AspNetUserRepository : Repository<AspNetUsers>, IAspNetUserRepository
    {
        public AspNetUserRepository(PBSAContext context) : base(context)
        {
        }

        public AspNetUsers RegisterUser(string username, string fullName, string phone)
        {
            if (Exist(aspNetUser => aspNetUser.UserName == username))
            {
                return null;
            }
            
            AspNetUsers user = new AspNetUsers
            {
                Id = username,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                FullName = fullName,
                PhoneNumber = phone,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsActive = true
            };
            return Add(user);
        }
    }
}