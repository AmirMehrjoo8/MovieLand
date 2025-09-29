using MovieLand.Models;

namespace MovieLand.Data.Repository
{
    public interface IUserRepository
    {
        public IEnumerable<User> GetAll();
        public User GetById(int id);
        public User GetByUsernamePassword(string username, string password);
        public bool CreateAccount(User user);
        public bool EditAccount(User user);
        public bool DeleteAccount(User user);
        public bool DeleteAccount(int id);
        public bool AccountExists(string username, string password);
        public bool UsernameEmailPhoneExists(string username, string email, string phone, int userId);
        public string UsernameEmailPhoneWhichExists(string username, string email, string phone, string lan, int userId);
        public IEnumerable<User> Search(string q);
        public void Save();
    }
}
