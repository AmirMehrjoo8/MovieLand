using Microsoft.EntityFrameworkCore;
using MovieLand.Data.Repository;
using MovieLand.Models;
using MovieLand.Models.Context;

namespace MovieLand.Data.Service
{
    public class ContactMsgRepository : IContactMsgRepository
    {
        private MovieLandDbContext _context;
        public ContactMsgRepository(MovieLandDbContext context)
        {
            _context = context;
        }
        public bool AddMsg(ContactMessage msg)
        {
            try
            {
                _context.ContactMessages.Add(msg);
                return true;
            }
            catch { return false; }
        }

        public bool DeleteMsg(ContactMessage msg)
        {
            try
            {
                _context.Entry(msg).State = EntityState.Deleted;
                return true;
            }
            catch { return false; }
        }

        public bool DeleteMsg(int msgId)
        {
            try
            {
                DeleteMsg(_context.ContactMessages.Find(msgId));
                return true;
            }
            catch { return false; }
        }

        public IEnumerable<ContactMessage> GetAll()
        {
            return _context.ContactMessages;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
