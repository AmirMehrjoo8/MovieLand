using MovieLand.Models;

namespace MovieLand.Data.Repository
{
    public interface IContactMsgRepository
    {
        public IEnumerable<ContactMessage> GetAll();
        public bool AddMsg(ContactMessage msg);
        public bool DeleteMsg(ContactMessage msg);
        public bool DeleteMsg(int msgId);
        public void Save();
    }
}
