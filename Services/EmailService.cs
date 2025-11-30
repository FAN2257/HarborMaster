using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    /// <summary>
    /// Service untuk mengirim email menggunakan MailKit
    /// Menggunakan Gmail SMTP untuk development
    /// </summary>
    public class EmailService
    {
        // SMTP Configuration - untuk production, pindahkan ke environment variables
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail = "harbormaster.system@gmail.com";
        private readonly string _fromName = "HarborMaster System";

        public EmailService()
        {
            // Load dari environment variables (untuk keamanan)
            // Untuk development, bisa di-set di launchSettings.json atau environment variables
            _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";

            // Fallback untuk development (JANGAN gunakan di production!)
            if (string.IsNullOrEmpty(_smtpUsername))
            {
                // TODO: Set email credentials via environment variables
                _smtpUsername = "your-email@gmail.com"; // CHANGE THIS
                _smtpPassword = "your-app-password";     // CHANGE THIS
            }
        }

        /// <summary>
        /// Kirim email berisi reset code
        /// </summary>
        public async Task<bool> SendPasswordResetEmail(string toEmail, string userName, string resetCode)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = "Password Reset - HarborMaster";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenerateEmailTemplate(userName, resetCode),
                    TextBody = $"Hello {userName},\n\nYour password reset code is: {resetCode}\n\nThis code expires in 30 minutes.\n\nIf you didn't request this, please ignore this email."
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging framework)
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate HTML email template
        /// </summary>
        private string GenerateEmailTemplate(string userName, string resetCode)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; margin: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 8px; margin-bottom: 30px; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .code-box {{ background: #f8f9fa; border: 2px dashed #667eea; padding: 20px; text-align: center; margin: 30px 0; border-radius: 8px; }}
        .code {{ font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; font-family: 'Courier New', monospace; }}
        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
        .footer {{ text-align: center; color: #6c757d; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #e9ecef; }}
        .button {{ display: inline-block; padding: 12px 30px; background-color: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>?? Password Reset Request</h1>
            <p>HarborMaster System</p>
        </div>
        
        <h2>Hello, {userName}!</h2>
        <p>You have requested to reset your password. Please use the code below to complete the process:</p>
        
        <div class='code-box'>
            <div class='code'>{resetCode}</div>
            <p style='color: #6c757d; margin-top: 10px; font-size: 14px;'>Enter this code in the application</p>
        </div>
        
        <div class='warning'>
            <strong>?? Important Security Information:</strong>
            <ul style='margin: 10px 0; padding-left: 20px;'>
                <li>This code will <strong>expire in 30 minutes</strong></li>
                <li>This code can only be used <strong>once</strong></li>
                <li>If you didn't request this password reset, please <strong>ignore this email</strong></li>
                <li>Never share this code with anyone</li>
            </ul>
        </div>
        
        <p style='margin-top: 30px;'>
            If you have any questions or concerns, please contact our support team.
        </p>
        
        <p style='margin-top: 20px;'>
            Best regards,<br>
            <strong>The HarborMaster Team</strong>
        </p>
        
        <div class='footer'>
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; 2025 HarborMaster. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Test SMTP connection (untuk debugging)
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
