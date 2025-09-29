using Microsoft.EntityFrameworkCore;
using MovieLand.Data.Repository;
using MovieLand.Models;
using MovieLand.Models.Context;

namespace MovieLand.Data.Service
{
    public class CommentReposetory : ICommentRepository
    {
        private MovieLandDbContext _context;
        public CommentReposetory(MovieLandDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Comment> GetCommentsByPostId(int postId, string postFormat)
        {
            return _context.Comments.Where(c => c.PostId == postId && c.PostFormat == postFormat && c.RepliedCommentId == 0).OrderByDescending(c => c.AddedDateTime).Include(c => c.User);
        }

        public IEnumerable<Comment> GetCommentsReplies(int postId, string postFormat)
        {
            return _context.Comments.Where(c => c.PostId == postId && c.PostFormat == postFormat && c.RepliedCommentId != 0).OrderByDescending(c => c.AddedDateTime).Include(c => c.User);
        }

        public bool AddComment(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                return true;
            }
            catch {  return false; }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
