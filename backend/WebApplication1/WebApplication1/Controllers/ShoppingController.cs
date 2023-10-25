using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using WebApplication1.Controllers.DataAccess;
using WebApplication1.Models;
using WebApplication1.Models.EmailSender;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        readonly IDataAccess dataAccess;
        private readonly string DateFormat;
        private readonly IEmailSender emailSender;  


        public ShoppingController(IDataAccess dataAccess, IConfiguration configuration, IEmailSender emailSender)
        {
            this.dataAccess = dataAccess;
            DateFormat = configuration["Constants:DateFormat"];
            this.emailSender = emailSender;
        }

        [HttpGet("GetCategoryList")]
        public IActionResult GetCategoryList()
        {
            var result = dataAccess.GetProductCategories();
            return Ok(result);
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string category, string subcategory, int count)
        {
            var result = dataAccess.GetProducts(category, subcategory, count);
            return Ok(result);
        }

        [HttpGet("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = dataAccess.GetProduct(id);
            return Ok(result);
        }

        [HttpPost("UpdateProduct/{id}")]
        public IActionResult UpdateProduct(int id)
        {
            var result = dataAccess.UpdateProduct(id);
            return Ok(result);
        }

        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            user.CreatedAt = DateTime.Now.ToString(DateFormat);
            user.ModifiedAt = DateTime.Now.ToString(DateFormat);

            var result = dataAccess.InsertUser(user);

            string? message;
            if (result) message = "inserted";
            else message = "email not available";

            return Ok(message);
        }

        [HttpPatch("UpdateUser")]
        public IActionResult UpdateUser([FromBody] User user)
        {
            user.ModifiedAt = DateTime.Now.ToString(DateFormat);

            var result = dataAccess.UpdateUser(user);

            string? message;
            if (result) message = "updated";
            else message = "user not found";

            return Ok(message);
        }


        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user)
        {
            var token = dataAccess.IsUserPresent(user.Email, user.Password);
            if (token == "") token = "invalid";
            return Ok(token);
        }

        [HttpPost("InsertReview")]
        public IActionResult InsertReview([FromBody] Review review)
        {
            review.CreatedAt = DateTime.Now.ToString(DateFormat);
            dataAccess.InsertReview(review);
            return Ok("inserted");
        }

        [HttpGet("GetProductReviews/{ProductId}")]
        public IActionResult GetProductReviews(int ProductId)
        {
            var result = dataAccess.GetProductReviews(ProductId);
            return Ok(result);
        }

        [HttpPost("InsertCartItem/{userid}/{productid}")]
        public IActionResult InsertCartItem(int userid, int productid)
        {
            var result = dataAccess.InsertCartItem(userid, productid);
            return Ok(result ? "inserted" : "not inserted");
        }

        [HttpPost("RemoveCartItem/{userid}/{productid}")]
        public IActionResult RemoveCartItem(int userid, int productid)
        {
            var result = dataAccess.RemoveCartItem(userid, productid);
            return Ok(result ? "removed" : "not removed");
        }

        [HttpPost("EmptyCart/{userid}")]
        public IActionResult EmptyCart(int userid)
        {
            var result = dataAccess.EmptyCart(userid);
            return Ok(result ? "Cart emptied" : "Cart could not be emptied");
        }

        [HttpGet("GetActiveCartOfUser/{id}")]
        public IActionResult GetActiveCartOfUser(int id)
        {
            var result = dataAccess.GetActiveCartOfUser(id);
            return Ok(result);
        }

        [HttpGet("GetAllPreviousCartsOfUser/{id}")]
        public IActionResult GetAllPreviousCartsOfUser(int id)
        {
            var result = dataAccess.GetAllPreviousCartsOfUser(id);
            return Ok(result);
        }


        [HttpPost("InsertPayment")]
        public IActionResult InsertPayment(Payment payment)
        {
            payment.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertPayment(payment);
            return Ok(id.ToString());
        }
        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder(Order order)
        {
            order.CreatedAt = DateTime.Now.ToString();
            var id = dataAccess.InsertOrder(order);
            return Ok(id.ToString());
        }

        [HttpPost("GetOTP")]
        public async Task<ActionResult> GetOTP([FromBody]EmailRequest emailRequest)
        {

            string subject = "Email Verification";
            var message = "Your email verification OTP is: ";

            await emailSender.SendEmailAsync(emailRequest.Reciever, subject, message);

            return Ok(true);    
        }

        [HttpPost("VerifyOTP")]
        public async Task<ActionResult> VerifyOTP([FromBody]OtpRequest otpRequest)
        {
            var res =  emailSender.VerifyOTP(otpRequest.Value);

            if (res == true)
            {
                var reponse = dataAccess.VerifyUser(otpRequest.Email);
                if (reponse == true)
                {
                    return Ok(true);
                }
            }
            return NotFound(false);
        }

    }
}
