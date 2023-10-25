namespace WebApplication1.Models.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters);

        Boolean VerifyOTP(string otp);
    }
}
