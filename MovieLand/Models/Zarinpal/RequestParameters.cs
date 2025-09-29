namespace MovieLand.Models.Zarinpal
{
    public class Metadata
    {
        public string Mobile { get; set; }
        public string Email { get; set; }
    }

    public class RequestParameters
    {
        public string merchant_id { get; set; }
        public string amount { get; set; }
        public string description { get; set; }
        public string callback_url { get; set; }
        public Metadata metadata { get; set; }

        public RequestParameters(string merchant_id, string amount, string description, string callback_url, string mobile, string email)
        {
            this.merchant_id = merchant_id;
            this.amount = amount;
            this.description = description;
            this.callback_url = callback_url;
            this.metadata = new Metadata
            {
                Mobile = mobile,
                Email = email
            };
        }
    }

}