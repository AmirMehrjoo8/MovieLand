using MovieLand.Models;

namespace MovieLand.Data.Repository
{
    public interface IDiscountCodeRepository
    {
        public IEnumerable<DiscountCode> GetAll();
        public DiscountCode GetById(string id);
        public bool CreateDiscountCode(DiscountCode code);
        public bool DeleteDiscountCode(DiscountCode code);
        public bool DeleteDiscountCode(string id);
        public bool DiscountCodeExists(string code);
        public bool EditDiscountCode(DiscountCode code);
        public void Save();
    }
}
