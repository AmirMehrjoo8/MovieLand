using System.Text.RegularExpressions;

namespace MovieLand.Classes
{
    public static class PhoneNumber
    {
        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // حذف فاصله، +۹۸ و ۰ فارسی
            phoneNumber = phoneNumber.Trim()
                                     .Replace("۰", "0")
                                     .Replace("۹", "9")
                                     .Replace("۸", "8")
                                     .Replace("۷", "7")
                                     .Replace("۶", "6")
                                     .Replace("۵", "5")
                                     .Replace("۴", "4")
                                     .Replace("۳", "3")
                                     .Replace("۲", "2")
                                     .Replace("۱", "1");

            // قبول فرمت‌های مختلف
            if (phoneNumber.StartsWith("+98"))
                phoneNumber = phoneNumber.Replace("+98", "0");

            // عددی که با 09 شروع بشه و بعدش دقیقاً ۹ رقم دیگه باشه
            return Regex.IsMatch(phoneNumber, @"^09\d{9}$");
        }

        public static string ConvertPhoneTypeTo09_9(this string thePhoneNumber)
        {
            string phoneNumber = thePhoneNumber.Trim()
                                     .Replace("۰", "0")
                                     .Replace("۹", "9")
                                     .Replace("۸", "8")
                                     .Replace("۷", "7")
                                     .Replace("۶", "6")
                                     .Replace("۵", "5")
                                     .Replace("۴", "4")
                                     .Replace("۳", "3")
                                     .Replace("۲", "2")
                                     .Replace("۱", "1");

            if (phoneNumber.StartsWith("+98"))
                phoneNumber = phoneNumber.Replace("+98", "0");

            if (Regex.IsMatch(phoneNumber, @"^09\d{9}$"))
                return phoneNumber;
            else
                return thePhoneNumber;
        }
    }
}
