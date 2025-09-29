using Microsoft.AspNetCore.Mvc;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;

namespace MovieLand.Components
{
    public class Comment : ViewComponent
    {
        private MovieLandDbContext _context;
        private ICommentRepository _commentRepository;
        public Comment(MovieLandDbContext context)
        {
            _context = context;
            _commentRepository = new CommentReposetory(_context);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var commentsVM = new CommentVM() 
            {
                Comments = _commentRepository.GetCommentsByPostId(Convert.ToInt32(TempData["postId"]), TempData["postFormat"].ToString()).ToList(),
                Replies = _commentRepository.GetCommentsReplies(Convert.ToInt32(TempData["postId"]), TempData["postFormat"].ToString()).ToList()
            };
            return View("/Views/Home/_Comments.cshtml", commentsVM);
        }
    }
}
