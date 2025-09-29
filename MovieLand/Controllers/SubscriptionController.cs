using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using MovieLand.Models;
using MovieLand.Models.Zarinpal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using RestSharp;
using MovieLand.Models.Context;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace MovieLand.Controllers
{
    public class SubscriptionController : Controller
    {
        private MovieLandDbContext _context;
        private IUserRepository _userRepository;
        private ITransactionRepository _transactionRepository;
        private IDiscountCodeRepository _discountCodeRepository;
        public SubscriptionController(MovieLandDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(_context);
            _transactionRepository = new TransactionRepository(_context);
            _discountCodeRepository = new DiscountCodeRepository(_context);
        }
        [Authorize]
        public IActionResult BuySub()
        {
            return View(_context.SubCards);
        }



        // اتصال به زرین پال -------------------------------------------------



        string merchant = "00000000-0000-0000-0000-000000000000";
        string amount;
        string authority;
        string description;
        string callbackurl;
        public IActionResult Payment(int cardId, string discountCode)
        {
            callbackurl = $"https://localhost:7283/Subscription/VerifyPayment/{cardId}/{discountCode}";
            string cardName = "";
            var subCard = _context.SubCards.Find(cardId);
            amount = Convert.ToInt64(subCard.Price * 10).ToString();

            if (discountCode != null)
            {
                if (_discountCodeRepository.DiscountCodeExists(discountCode))
                {
                    var codeDetails = _discountCodeRepository.GetById(discountCode);
                    if (codeDetails.TotalUsed < codeDetails.MaxUsers && codeDetails.ExpireDateTime > DateTime.Now)
                    {
                        double discountPercent = codeDetails.DiscountPercent / 100.0;
                        amount = Convert.ToInt64(int.Parse(amount) - int.Parse(amount) * discountPercent).ToString();
                        if (amount == "0")
                        {
                            codeDetails.TotalUsed++;
                            _discountCodeRepository.EditDiscountCode(codeDetails);

                            // add transaction
                            _transactionRepository.AddTransaction(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), cardId, true, Convert.ToDecimal(amount));
                            _transactionRepository.Save();
                            ViewBag.Description = description;
                            if (CultureInfo.CurrentCulture.Name == "fa-IR")
                                TempData["ProfileAlert"] = "کارت اشتراک " + cardName + " مووی لند با " + codeDetails.DiscountPercent + "% تخفیف برای شما فعال شد.";
                            else
                                TempData["ProfileAlert"] = "Your MovieLand " + cardName + " subscription card has been activated with a " + codeDetails.DiscountPercent + "% discount.";

                            return Redirect("/Account/Profile");
                        }
                    }
                }
            }
            if (CultureInfo.CurrentUICulture.Name == "fa-IR")
            {
                switch (cardId)
                {
                    case 2: { cardName = "برنز"; break; }
                    case 3: { cardName = "نقره ای"; break; }
                    case 4: { cardName = "طلایی"; break; }
                }
                description = "خرید کارت اشتراک " + cardName + " مووی لند";
            }
            else
            {
                switch (cardId)
                {
                    case 2: { cardName = "Bronze"; break; }
                    case 3: { cardName = "Silver"; break; }
                    case 4: { cardName = "Gold"; break; }
                }
                description = "Purchase of MovieLand " + cardName + " Subscription Card";
            }

            //try
            //{
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = _userRepository.GetById(userId);
                Models.Zarinpal.RequestParameters Parameters = new Models.Zarinpal.RequestParameters(merchant, amount, description, callbackurl, user.Phone, user.Email);



                //be dalil in ke metadata be sorate araye ast va do meghdare mobile va email dar metadata gharar mmigirad
                //shoma mitavanid in maghadir ra az kharidar begirid va set konid dar gheir in sorat khali ersal konid

                var client = new RestClient(URLs.requestUrl);

                RestSharp.Method method = RestSharp.Method.Post;

                var request = new RestRequest("", method);

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");

                request.AddJsonBody(Parameters);

                var requestresponse = client.ExecuteAsync(request);

                JObject jo = JObject.Parse(requestresponse.Result.Content);

                string errorscode = jo["errors"].ToString();

                JObject jodata = JObject.Parse(requestresponse.Result.Content);

                string dataauth = jodata["data"].ToString();


                if (dataauth != "{}")
                {
                    authority = jodata["data"]["authority"].ToString();

                    string gatewayUrl = URLs.gateWayUrl + authority;

                    return Redirect(gatewayUrl);

                }
                else
                {


                    return BadRequest("error " + errorscode);


                }


            //}

            //catch (Exception ex)
            //{
            //    //    throw new Exception(ex.Message);


            //}
            return null;
        }

        public IActionResult VerifyPayment(int id, string? code)
        {

            string cardName = "";
            var subCard = _context.SubCards.Find(id);
            amount = Convert.ToInt64(subCard.Price * 10).ToString();
            if (code != null)
            {
                if (_discountCodeRepository.DiscountCodeExists(code))
                {
                    var codeDetails = _discountCodeRepository.GetById(code);
                    double discountPercent = codeDetails.DiscountPercent / 100.0;
                    amount = Convert.ToInt64(int.Parse(amount) - int.Parse(amount) * discountPercent).ToString();
                }
            }
            if (CultureInfo.CurrentUICulture.Name == "fa-IR")
            {
                switch (id)
                {
                    case 2: { cardName = "برنز"; break; }
                    case 3: { cardName = "نقره ای"; break; }
                    case 4: { cardName = "طلایی"; break; }
                }
                description = "خرید کارت اشتراک " + cardName + " مووی لند";
            }
            else
            {
                switch (id)
                {
                    case 2: { cardName = "Bronze"; break; }
                    case 3: { cardName = "Silver"; break; }
                    case 4: { cardName = "Gold"; break; }
                }
                description = "Purchase of MovieLand " + cardName + " Subscription Card";
            }
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // string authorityverify;

            try
            {
                VerifyParameters parameters = new VerifyParameters();


                if (HttpContext.Request.Query["Authority"] != "")
                {
                    authority = HttpContext.Request.Query["Authority"];
                }

                parameters.authority = authority;
                parameters.amount = amount;
                parameters.merchant_id = merchant;


                var client = new RestClient(URLs.verifyUrl);
                RestSharp.Method method = RestSharp.Method.Post;
                var request = new RestRequest("", method);

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");
                request.AddJsonBody(parameters);

                var response = client.ExecuteAsync(request);


                JObject jodata = JObject.Parse(response.Result.Content);

                string data = jodata["data"].ToString();

                JObject jo = JObject.Parse(response.Result.Content);

                string errors = jo["errors"].ToString();

                if (data != "{}")
                {
                    if (code != null)
                    {
                        var discountCode = _discountCodeRepository.GetById(code);
                        discountCode.TotalUsed++;
                        _discountCodeRepository.EditDiscountCode(discountCode);
                        if (CultureInfo.CurrentCulture.Name == "fa-IR")
                            TempData["ProfileAlert"] = "کارت اشتراک " + cardName + " مووی لند با " + discountCode.DiscountPercent + "% تخفیف برای شما فعال شد.";
                        else
                            TempData["ProfileAlert"] = "Your MovieLand " + cardName + " subscription card has been activated with a " + discountCode.DiscountPercent + "% discount.";

                    }

                    string refid = jodata["data"]["ref_id"].ToString();
                    // add transaction
                    _transactionRepository.AddTransaction(userId, id, true, Convert.ToDecimal(amount));
                    _transactionRepository.Save();
                    ViewBag.code = refid;
                    ViewBag.Description = description;
                    if (code == null)
                    {
                        if (CultureInfo.CurrentCulture.Name == "fa-IR")
                            TempData["ProfileAlert"] = "کارت اشتراک " + cardName + " مووی لند برای شما فعال شد.";
                        else
                            TempData["ProfileAlert"] = "Your MovieLand " + cardName + " subscription card has been activated.";
                    }
                    return Redirect("/Account/Profile");
                }
                else if (errors != "{}")
                {

                    // add transaction
                    _transactionRepository.AddTransaction(userId, id, false, Convert.ToDecimal(amount));
                    _transactionRepository.Save();
                    string errorscode = jo["errors"]["code"].ToString();

                    return Redirect("/Subscription/BuySub");

                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return NotFound();
        }

        public async Task<IActionResult> PaymenBytHttpClient()
        {

            try
            {

                using (var client = new HttpClient())
                {
                    Models.Zarinpal.RequestParameters parameters = new Models.Zarinpal.RequestParameters(merchant, amount, description, callbackurl, "09123456789", "test@example.com");

                    var json = JsonConvert.SerializeObject(parameters);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(URLs.requestUrl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jo = JObject.Parse(responseBody);
                    string errorscode = jo["errors"].ToString();

                    JObject jodata = JObject.Parse(responseBody);
                    string dataauth = jodata["data"].ToString();


                    if (dataauth != "[]")
                    {


                        authority = jodata["data"]["authority"].ToString();

                        string gatewayUrl = URLs.gateWayUrl + authority;

                        return Redirect(gatewayUrl);

                    }
                    else
                    {

                        return BadRequest("error " + errorscode);


                    }

                }


            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
            return NotFound();
        }

        public async Task<IActionResult> VerifyByHttpClient()
        {
            try
            {

                VerifyParameters parameters = new VerifyParameters();


                if (HttpContext.Request.Query["Authority"] != "")
                {
                    authority = HttpContext.Request.Query["Authority"];
                }

                parameters.authority = authority;

                parameters.amount = amount;

                parameters.merchant_id = merchant;


                using (HttpClient client = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(parameters);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(URLs.verifyUrl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jodata = JObject.Parse(responseBody);

                    string data = jodata["data"].ToString();

                    JObject jo = JObject.Parse(responseBody);

                    string errors = jo["errors"].ToString();

                    if (data != "[]")
                    {
                        string refid = jodata["data"]["ref_id"].ToString();

                        ViewBag.code = refid;

                        return View();
                    }
                    else if (errors != "[]")
                    {

                        string errorscode = jo["errors"]["code"].ToString();

                        return BadRequest($"error code {errorscode}");

                    }
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }
            return NotFound();
        }
    }
}
