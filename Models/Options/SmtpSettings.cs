namespace SmartSpendAI.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public bool EnableSsl { get; set; } = true;

        public bool UseOAuth2 { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FromEmail { get; set; } = "no-reply@smartspend.local";

        public string FromName { get; set; } = "SmartSpend AI";
    }
}
