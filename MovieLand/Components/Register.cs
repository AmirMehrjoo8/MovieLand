using Microsoft.AspNetCore.Mvc;
using MovieLand.Models.ViewModels;

namespace MovieLand.Components
{
    public class Register:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Shared/_Register.cshtml", new RegisterVM());
        }
    }
}
