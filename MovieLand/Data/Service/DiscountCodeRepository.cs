using MovieLand.Data.Repository;
using MovieLand.Models;
using MovieLand.Models.Context;

namespace MovieLand.Data.Service
{
    public class DiscountCodeRepository : IDiscountCodeRepository
    {
        private MovieLandDbContext _context;
        public DiscountCodeRepository(MovieLandDbContext context)
        {
            _context = context;
        }
        public bool CreateDiscountCode(DiscountCode code)
        {
            try
            {
                _context.DiscountCodes.Add(code);
                return true;
            }
            catch {  return false; }
        }

        public bool DeleteDiscountCode(DiscountCode code)
        {
            try
            {
                _context.Entry(code).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                return true;
            }
            catch { return false; }
        }

        public bool DeleteDiscountCode(string id)
        {
            try
            {
                DeleteDiscountCode(GetById(id));
                return true;
            }
            catch { return false; }
        }

        public bool DiscountCodeExists(string code)
        {
            return _context.DiscountCodes.Any(c => c.TheDiscountCode == code);
        }

        public bool EditDiscountCode(DiscountCode code)
        {
            try
            {
                _context.Entry(code).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            catch { return false; }
        }

        public IEnumerable<DiscountCode> GetAll()
        {
            return _context.DiscountCodes.OrderBy(d => d.ExpireDateTime);
        }

        public DiscountCode GetById(string id)
        {
            return _context.DiscountCodes.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
