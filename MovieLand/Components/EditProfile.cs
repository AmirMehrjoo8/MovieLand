using Microsoft.AspNetCore.Mvc;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;

namespace MovieLand.Components
{
    public class EditProfile : ViewComponent
    {
        private MovieLandDbContext _context;
        private IUserRepository _userRepository;
        public EditProfile(MovieLandDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(_context);
        }
        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            ViewBag.UserId = userId;
            var user = _userRepository.GetById(userId);
            var editProfileVM = new EditProfileVM()
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password,
                Username = user.Username
            };
            return View("/Views/Account/_EditProfile.cshtml", editProfileVM);
        }
    }
}
