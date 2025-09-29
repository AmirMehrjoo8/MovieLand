using System.Globalization;

namespace MovieLand.Classes
{
    public static class UserProfile
    {
        // اگه پسوند عکس رو نداشتم این متد فقط با اسم فایلو پیدا میکنه
        public static string ProfilePictureLocation(string fileNameWithoutExt)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ProfilePictures");

            var file = Directory.GetFiles(folderPath, fileNameWithoutExt + ".*").FirstOrDefault();

            if (file != null)
            {
                string fileNameWithExt = Path.GetFileName(file);
                string url = "\\Images\\ProfilePictures\\" + fileNameWithExt; // آدرس کامل عکس
                return url;
            }
            return "\\Images\\ProfilePictures\\DefaultProfilePicture.jpg";
        }
        public static string ProfilePictureFullPath(string pictureNameWithoutExt)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ProfilePictures");
            string fileNameWithoutExt = pictureNameWithoutExt;

            var file = Directory.GetFiles(folderPath, fileNameWithoutExt + ".*").FirstOrDefault();

            if (file != null)
            {
                string fileNameWithExt = Path.GetFileName(file);
                string url = Directory.GetCurrentDirectory() + "\\wwwroot\\Images\\ProfilePictures\\" + fileNameWithExt; // آدرس کامل عکس
                return url;
            }
            return "";
        }

        public static string GetUserType(int typeId, int subId)
        {
            var lan = CultureInfo.CurrentUICulture.Name;
            if (lan == "fa-IR")
            {
                switch (typeId)
                {
                    case 0: // کاربر معمولی
                        return "کاربر معمولی";
                    case 1: // ادمین
                        {
                            switch (subId)
                            {
                                case 1:
                                    return "ادمین";
                                case 2:
                                    return "ادمین برنز";
                                case 3:
                                    return "ادمین نقره ای";
                                case 4:
                                    return "ادمین طلایی";
                                default:
                                    return "ادمین";
                            }
                        }
                    case 2: // اشتراک دار
                        {
                            switch (subId)
                            {
                                case 1:
                                    return "کاربر معمولی";
                                case 2:
                                    return "کاربر برنز";
                                case 3:
                                    return "کاربر نقره ای";
                                case 4:
                                    return "کاربر طلایی";
                                default:
                                    return "کاربر ویژه";
                            }
                        }
                }
            }
            else if (lan == "en-US")
            {
                switch (typeId)
                {
                    case 0: // Normal user
                        return "Normal User";
                    case 1: // Admin
                        {
                            switch (subId)
                            {
                                case 1:
                                    return "Admin";
                                case 2:
                                    return "Bronze Admin";
                                case 3:
                                    return "Silver Admin";
                                case 4:
                                    return "Gold Admin";
                                default:
                                    return "Admin";
                            }
                        }
                    case 2: // Subscribed user
                        {
                            switch (subId)
                            {
                                case 1:
                                    return "Normal User";
                                case 2:
                                    return "Bronze User";
                                case 3:
                                    return "Silver User";
                                case 4:
                                    return "Gold User";
                                default:
                                    return "VIP User";
                            }
                        }
                }
            }

            return typeId.ToString();
        }

        public static bool SaveProfilePicture(int userId, IFormFile picture)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Images",
                        "ProfilePictures",
                        userId + Path.GetExtension(picture.FileName));
                var lastProfile = ProfilePictureFullPath(userId.ToString());
                if (System.IO.File.Exists(lastProfile))
                    System.IO.File.Delete(lastProfile);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    picture.CopyTo(stream);
                }
                return true;
            }
            catch { return false; }
        }

        public static string SubTimeRemaining(this DateTime subExpireDate)
        {
            var remaining = subExpireDate - DateTime.Now;
            string lan = CultureInfo.CurrentUICulture.Name;
            if (lan == "fa-IR")
            {
                if (remaining.TotalDays > 2)
                    return remaining.Days.ToString() + " روز";
                else if (remaining.TotalDays <= 2)
                    return remaining.Hours.ToString() + " ساعت";
                else if (remaining.TotalHours < 2)
                    return remaining.Minutes.ToString() + " دقیقه";
            }

            if (lan == "en-US")
            {
                if (remaining.TotalDays > 2)
                    return remaining.Days.ToString() + " days";
                else if (remaining.TotalDays <= 2)
                    return remaining.Hours.ToString() + " hours";
                else if (remaining.TotalHours < 2)
                    return remaining.Minutes.ToString() + " minutes";
            }

            return remaining.TotalDays.ToString();
        }
    }
}
