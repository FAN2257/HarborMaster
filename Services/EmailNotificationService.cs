using HarborMaster.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    /// <summary>
    /// Email notification service using Gmail SMTP
    /// Sends automated emails for docking requests, approvals, and password recovery
    /// </summary>
    public class EmailNotificationService
    {
        public EmailNotificationService()
        {
            // Check if SMTP is configured
            if (!ApiConfiguration.IsSmtpConfigured())
            {
                throw new Exception("Gmail SMTP not configured. Please update ApiConfiguration.cs with your Gmail credentials.");
            }
        }

        /// <summary>
        /// Send email notification when docking request is approved
        /// </summary>
        public async Task<bool> SendRequestApprovedEmailAsync(User shipOwner, DockingRequest request, Ship ship, Berth berth, BerthAssignment assignment)
        {
            try
            {
                var subject = $"✅ Docking Request Approved - REQ-{request.Id:D4}";

                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; }}
        .header {{ background: #27ae60; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ padding: 20px 0; }}
        .info-box {{ background: #ecf0f1; padding: 15px; border-radius: 5px; margin: 10px 0; }}
        .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Docking Request Approved!</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{shipOwner.FullName}</strong>,</p>
            <p>Your docking request has been <strong>approved</strong>. Please find the details below:</p>

            <div class='info-box'>
                <h3>📋 Request Information</h3>
                <p><strong>Request ID:</strong> REQ-{request.Id:D4}</p>
                <p><strong>Ship:</strong> {ship.Name} ({ship.ImoNumber})</p>
                <p><strong>Ship Type:</strong> {ship.ShipType}</p>
            </div>

            <div class='info-box'>
                <h3>⚓ Berth Assignment</h3>
                <p><strong>Berth:</strong> {berth.BerthName}</p>
                <p><strong>Location:</strong> {berth.Location}</p>
                <p><strong>ETA:</strong> {assignment.ETA:dd MMMM yyyy HH:mm}</p>
                <p><strong>ETD:</strong> {assignment.ETD:dd MMMM yyyy HH:mm}</p>
                <p><strong>Duration:</strong> {(assignment.ETD - assignment.ETA).Days} days</p>
            </div>

            <div class='info-box'>
                <h3>💰 Invoice Details</h3>
                <p>An invoice has been generated for this docking.</p>
                <p>You can view and download your invoice from the <strong>My Requests</strong> section in the Harbor Master application.</p>
            </div>

            <p>Please ensure your ship arrives on time. If there are any delays, please notify the harbor master immediately.</p>
        </div>
        <div class='footer'>
            <p>This is an automated email from Harbor Master System.</p>
            <p>© {DateTime.Now.Year} Harbor Master. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                var plainTextContent = $@"
DOCKING REQUEST APPROVED

Dear {shipOwner.FullName},

Your docking request has been approved.

Request ID: REQ-{request.Id:D4}
Ship: {ship.Name} ({ship.ImoNumber})
Berth: {berth.BerthName} - {berth.Location}
ETA: {assignment.ETA:dd MMMM yyyy HH:mm}
ETD: {assignment.ETD:dd MMMM yyyy HH:mm}
Duration: {(assignment.ETD - assignment.ETA).Days} days

An invoice has been generated. You can view it in the Harbor Master application.

Please ensure your ship arrives on time.

---
Harbor Master System
© {DateTime.Now.Year} All rights reserved.
";

                return await SendEmailAsync(shipOwner.Email, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send approval email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Send email notification when docking request is rejected
        /// </summary>
        public async Task<bool> SendRequestRejectedEmailAsync(User shipOwner, DockingRequest request, Ship ship, string rejectionReason)
        {
            try
            {
                var subject = $"❌ Docking Request Rejected - REQ-{request.Id:D4}";

                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; }}
        .header {{ background: #e74c3c; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ padding: 20px 0; }}
        .info-box {{ background: #ecf0f1; padding: 15px; border-radius: 5px; margin: 10px 0; }}
        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 10px 0; }}
        .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>❌ Docking Request Rejected</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{shipOwner.FullName}</strong>,</p>
            <p>Unfortunately, your docking request has been <strong>rejected</strong>.</p>

            <div class='info-box'>
                <h3>📋 Request Information</h3>
                <p><strong>Request ID:</strong> REQ-{request.Id:D4}</p>
                <p><strong>Ship:</strong> {ship.Name} ({ship.ImoNumber})</p>
                <p><strong>Requested ETA:</strong> {request.RequestedETA:dd MMMM yyyy HH:mm}</p>
                <p><strong>Requested ETD:</strong> {request.RequestedETD:dd MMMM yyyy HH:mm}</p>
            </div>

            <div class='warning'>
                <h3>📝 Rejection Reason</h3>
                <p>{rejectionReason}</p>
            </div>

            <p>You may submit a new docking request with different dates or contact the harbor master for assistance.</p>
        </div>
        <div class='footer'>
            <p>This is an automated email from Harbor Master System.</p>
            <p>© {DateTime.Now.Year} Harbor Master. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                var plainTextContent = $@"
DOCKING REQUEST REJECTED

Dear {shipOwner.FullName},

Unfortunately, your docking request has been rejected.

Request ID: REQ-{request.Id:D4}
Ship: {ship.Name} ({ship.ImoNumber})
Requested ETA: {request.RequestedETA:dd MMMM yyyy HH:mm}
Requested ETD: {request.RequestedETD:dd MMMM yyyy HH:mm}

REJECTION REASON:
{rejectionReason}

You may submit a new request with different dates.

---
Harbor Master System
© {DateTime.Now.Year} All rights reserved.
";

                return await SendEmailAsync(shipOwner.Email, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send rejection email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Send password recovery email with reset link or temporary password
        /// </summary>
        public async Task<bool> SendPasswordRecoveryEmailAsync(string toEmail, string fullName, string temporaryPassword)
        {
            try
            {
                var subject = "🔐 Password Recovery - Harbor Master System";

                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; }}
        .header {{ background: #3498db; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ padding: 20px 0; }}
        .password-box {{ background: #fff3cd; border: 2px solid #ffc107; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center; }}
        .password {{ font-size: 24px; font-weight: bold; color: #d35400; letter-spacing: 2px; font-family: monospace; }}
        .warning {{ background: #ffe5e5; border-left: 4px solid #e74c3c; padding: 15px; margin: 10px 0; }}
        .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Recovery</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{fullName}</strong>,</p>
            <p>You have requested a password reset for your Harbor Master account.</p>

            <div class='password-box'>
                <p><strong>Your Temporary Password:</strong></p>
                <p class='password'>{temporaryPassword}</p>
            </div>

            <div class='warning'>
                <p><strong>⚠️ IMPORTANT SECURITY NOTICE:</strong></p>
                <ul>
                    <li>This is a temporary password valid for one-time use</li>
                    <li>Please login and <strong>change your password immediately</strong></li>
                    <li>Do not share this password with anyone</li>
                    <li>If you didn't request this reset, please contact us immediately</li>
                </ul>
            </div>

            <p><strong>Next Steps:</strong></p>
            <ol>
                <li>Login to Harbor Master System using the temporary password above</li>
                <li>Go to your Profile settings</li>
                <li>Change your password to a new secure password</li>
            </ol>
        </div>
        <div class='footer'>
            <p>This is an automated email from Harbor Master System.</p>
            <p>© {DateTime.Now.Year} Harbor Master. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                var plainTextContent = $@"
PASSWORD RECOVERY

Dear {fullName},

You have requested a password reset for your Harbor Master account.

YOUR TEMPORARY PASSWORD: {temporaryPassword}

⚠️ IMPORTANT SECURITY NOTICE:
- This is a temporary password valid for one-time use
- Please login and change your password immediately
- Do not share this password with anyone
- If you didn't request this reset, please contact us immediately

NEXT STEPS:
1. Login to Harbor Master System using the temporary password above
2. Go to your Profile settings
3. Change your password to a new secure password

---
Harbor Master System
© {DateTime.Now.Year} All rights reserved.
";

                return await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send password recovery email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Send password reset code email (for reset token flow)
        /// </summary>
        public async Task<bool> SendPasswordResetCodeEmailAsync(string toEmail, string fullName, string resetCode)
        {
            try
            {
                var subject = "🔐 Password Reset Code - Harbor Master System";

                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f7fa; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; }}
        .header {{ background: #3498db; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ padding: 20px 0; }}
        .code-box {{ background: #fff3cd; border: 2px solid #ffc107; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center; }}
        .code {{ font-size: 32px; font-weight: bold; color: #d35400; letter-spacing: 5px; font-family: monospace; }}
        .warning {{ background: #ffe5e5; border-left: 4px solid #e74c3c; padding: 15px; margin: 10px 0; }}
        .footer {{ text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Reset Request</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{fullName}</strong>,</p>
            <p>You have requested to reset your password for your Harbor Master account.</p>

            <div class='code-box'>
                <p><strong>Your Password Reset Code:</strong></p>
                <p class='code'>{resetCode}</p>
            </div>

            <div class='warning'>
                <p><strong>⚠️ IMPORTANT SECURITY NOTICE:</strong></p>
                <ul>
                    <li>This code is valid for <strong>30 minutes</strong></li>
                    <li>Enter this code in the password reset form</li>
                    <li>Do not share this code with anyone</li>
                    <li>If you didn't request this reset, please ignore this email</li>
                </ul>
            </div>

            <p><strong>Next Steps:</strong></p>
            <ol>
                <li>Go to the Forgot Password page in Harbor Master System</li>
                <li>Enter the reset code shown above</li>
                <li>Choose your new password</li>
                <li>Your password will be updated immediately</li>
            </ol>
        </div>
        <div class='footer'>
            <p>This is an automated email from Harbor Master System.</p>
            <p>© {DateTime.Now.Year} Harbor Master. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                var plainTextContent = $@"
PASSWORD RESET REQUEST

Dear {fullName},

You have requested to reset your password for your Harbor Master account.

YOUR PASSWORD RESET CODE: {resetCode}

⚠️ IMPORTANT SECURITY NOTICE:
- This code is valid for 30 minutes
- Enter this code in the password reset form
- Do not share this code with anyone
- If you didn't request this reset, please ignore this email

NEXT STEPS:
1. Go to the Forgot Password page in Harbor Master System
2. Enter the reset code shown above
3. Choose your new password
4. Your password will be updated immediately

---
Harbor Master System
© {DateTime.Now.Year} All rights reserved.
";

                return await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send password reset code email: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Core method to send email via Gmail SMTP using MailKit
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(ApiConfiguration.SenderName, ApiConfiguration.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // Create multipart message with both plain text and HTML
                var builder = new BodyBuilder
                {
                    TextBody = plainTextContent,
                    HtmlBody = htmlContent
                };

                message.Body = builder.ToMessageBody();

                // Send email using SMTP
                using (var client = new SmtpClient())
                {
                    // Connect to Gmail SMTP server
                    await client.ConnectAsync(ApiConfiguration.SmtpHost, ApiConfiguration.SmtpPort, SecureSocketOptions.StartTls);

                    // Authenticate
                    await client.AuthenticateAsync(ApiConfiguration.SmtpUsername, ApiConfiguration.SmtpPassword);

                    // Send email
                    await client.SendAsync(message);

                    // Disconnect
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"SMTP error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Send email with PDF attachment
        /// </summary>
        public async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlContent, string plainTextContent, string attachmentPath)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(ApiConfiguration.SenderName, ApiConfiguration.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // Create body with attachment
                var builder = new BodyBuilder
                {
                    TextBody = plainTextContent,
                    HtmlBody = htmlContent
                };

                // Add attachment if file exists
                if (File.Exists(attachmentPath))
                {
                    await builder.Attachments.AddAsync(attachmentPath);
                }

                message.Body = builder.ToMessageBody();

                // Send email using SMTP
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(ApiConfiguration.SmtpHost, ApiConfiguration.SmtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(ApiConfiguration.SmtpUsername, ApiConfiguration.SmtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"SMTP error: {ex.Message}", ex);
            }
        }
    }
}
