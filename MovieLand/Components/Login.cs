using Microsoft.AspNetCore.Mvc;
using MovieLand.Models.ViewModels;

namespace MovieLand
{
    public class Login:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Shared/_Login.cshtml", new LoginVM());
        }
    }
}
