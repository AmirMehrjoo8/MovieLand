using MovieLand.Data.Repository;
using MovieLand.Models;
using MovieLand.Models.Context;
using Microsoft.EntityFrameworkCore;
using MovieLand.Models.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MovieLand.Data.Service
{
    public class UserRepository : IUserRepository
    {
        private MovieLandDbContext _context;
        public UserRepository(MovieLandDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User GetByUsernamePassword(string username, string password)
        {
            return _context.Users.SingleOrDefault(u =>
                (u.Username == username.Trim() || u.Email == username.Trim() || u.Phone == username.Trim())
                && u.Password == password.Trim());
        }

        public bool CreateAccount(User user)
        {
            try
            {
                _context.Users.Add(user);
                return true;
            }
            catch { return false; }
        }

        public bool EditAccount(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                return true;
            }
            catch { return false; }
        }

        public bool DeleteAccount(User user)
        {
            try
            {
                foreach(var i in _context.Comments.Where(c => c.UserId == user.UserId))
                {
                    _context.Entry(i).State = EntityState.Deleted;
                }
                _context.Entry(user).State = EntityState.Deleted;
                return true;
            }
            catch { return false; }
        }

        public bool DeleteAccount(int id)
        {
            return DeleteAccount(GetById(id));
        }

        public bool AccountExists(string username, string password)
        {
            try
            {
                return _context.Users.Any(u =>
                (u.Username == username.Trim() || u.Email == username.Trim() || u.Phone == username.Trim())
                && u.Password == password.Trim());
            }
            catch { return false; }
        }

        public bool UsernameEmailPhoneExists(string username, string email, string phone, int userId)
        {
            return _context.Users.Any(u =>
                (u.Username == username.Trim() ||
                 u.Email == email.Trim() ||
                 u.Phone == phone.Trim()) &&
                u.UserId != userId // کاربر فعلی را نادیده بگیر
            );
        }


        public string UsernameEmailPhoneWhichExists(string username, string email, string phone, string lan, int userId)
        {
            switch (lan)
            {
                case "en-US":
                    {
                        if (_context.Users.Any(u => u.Username.ToLower() == username.Trim().ToLower() && u.UserId != userId))
                            return "The username is already taken.";
                        else if (_context.Users.Any(u => u.Email.ToLower() == email.Trim().ToLower() && u.UserId != userId))
                            return "The email address is already registered.";
                        else if (_context.Users.Any(u => u.Phone.ToLower() == phone.Trim().ToLower() && u.UserId != userId))
                            return "The phone number is already registered.";
                        else
                            return null;
                    }
                case "fa-IR":
                    {
                        if (_context.Users.Any(u => u.Username.ToLower() == username.Trim().ToLower() && u.UserId != userId))
                            return "نام کاربری وارد شده قبلاً ثبت شده است.";
                        else if (_context.Users.Any(u => u.Email.ToLower() == email.Trim().ToLower() && u.UserId != userId))
                            return "ایمیل وارد شده قبلاً ثبت شده است.";
                        else if (_context.Users.Any(u => u.Phone.ToLower() == phone.Trim().ToLower() && u.UserId != userId))
                            return "شماره موبایل وارد شده قبلاً ثبت شده است.";
                        else
                            return null;
                    }
            }
            return null;
        }


        public void Save()
        {
            _context.SaveChanges();
        }

        public IEnumerable<User> Search(string q)
        {
            try
            {
                if (q.Length == 0 || q == null)
                    return GetAll();

                var res = _context.Users.Where(u => u.Username.Contains(q) || u.Name.Contains(q) || u.Phone.Contains(q) || u.Email.Contains(q));
                return res;
            }
            catch
            {
                return GetAll();
            }
        }
    }
}