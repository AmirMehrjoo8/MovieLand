using System.Globalization;
using System.Security.Policy;
namespace MovieLand.Classes
{
    public static class ConvertDate
    {
        public static string ToShamsi(DateTime date)
        {
            var lan = CultureInfo.CurrentUICulture.Name;
            PersianCalendar pc = new PersianCalendar();
            var now = DateTime.Now; // زمان فعلی
            var timeDifference = now - date; // تفاوت زمان
            switch (lan)
            {
                case "en-US":
                    {
                        if (timeDifference.TotalMinutes < 1)
                        {
                            return "Now";
                        }
                        else if (timeDifference.TotalMinutes < 60)
                        {
                            return $"{(int)timeDifference.TotalMinutes} m ago";
                        }
                        else if (timeDifference.TotalHours < 24)
                        {
                            return $"{(int)timeDifference.TotalHours} h ago";
                        }
                        else if (timeDifference.TotalDays < 30)
                        {
                            return $"{(int)timeDifference.TotalDays} d ago";
                        }
                        else
                        {
                            return date.ToString("yyyy-MM-dd");
                            // نمایش تاریخ کامل
                        }
                    }
                case "fa-IR":
                    {
                        if (timeDifference.TotalMinutes < 1)
                        {
                            return "همین الان";
                        }
                        else if (timeDifference.TotalMinutes < 60)
                        {
                            return $"{(int)timeDifference.TotalMinutes} دقیقه پیش";
                        }
                        else if (timeDifference.TotalHours < 24)
                        {
                            return $"{(int)timeDifference.TotalHours} ساعت پیش";
                        }
                        else if (timeDifference.TotalDays < 30)
                        {
                            return $"{(int)timeDifference.TotalDays} روز پیش";
                        }
                        else
                        {
                            return $"{pc.GetYear(date)}/{pc.GetMonth(date).ToString("00")}/{pc.GetDayOfMonth(date).ToString("00")}";
                            // نمایش تاریخ کامل
                        }
                    }
            }

            return date.ToString();
        }

        public static string SimpleDateTimeByLanguage(DateTime date)
        {
            try
            {
                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                {
                    var pc = new PersianCalendar();
                    return $"{pc.GetYear(date)}/{pc.GetMonth(date).ToString("00")}/{pc.GetDayOfMonth(date).ToString("00")} {date.ToString("HH:mm")}";

                }
                else
                    return date.ToString("yyyy-MM-dd HH:mm");
            }
            catch
            {
                return date.ToString();
            }
        }

        public static string SimpleDateByLanguage(DateTime date)
        {
            try
            {
                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                {
                    var pc = new PersianCalendar();
                    return $"{pc.GetYear(date)}/{pc.GetMonth(date).ToString("00")}/{pc.GetDayOfMonth(date).ToString("00")}";

                }
                else
                    return date.ToString("yyyy-MM-dd");
            }
            catch
            {
                return date.ToString();
            }
        }
    }
}
