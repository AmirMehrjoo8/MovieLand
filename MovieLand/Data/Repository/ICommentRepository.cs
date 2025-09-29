using MovieLand.Models;

namespace MovieLand.Data.Repository
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetCommentsByPostId(int postId, string postFormat);
        IEnumerable<Comment> GetCommentsReplies(int postId, string postFormat);
        bool AddComment(Comment comment);
        void Save();
    }
}
