using DevHub.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DevHub.Services.EmailNotfiticationService;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    public EmailService(IOptions<EmailSettings> option)
    {
        _emailSettings = option.Value;
    }
    public async Task SendEmailForgotPasswordCodeAsync(UserVerificationToken vToken)
    {
        throw new NotImplementedException();
    }
    public async Task SendEmailVerificationMailAsync(UserVerificationToken vToken)
    {
        using (var smtpClient = new SmtpClient())
        {
            smtpClient.Host = _emailSettings.Host;
            smtpClient.Port = _emailSettings.Port;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.AppEmail, _emailSettings.Password);
            smtpClient.EnableSsl = true;
            var mailMessage = new MailMessage(_emailSettings.AppEmail, vToken.Email)
            {
                Subject = "Email Verification to DevHub",
                IsBodyHtml = true,
                Body = generateVerificationPage(vToken.Url)
            };
            await smtpClient.SendMailAsync(mailMessage);
//                await smtpClient.SendMailAsync(_emailSettings.AppEmail, vToken.Email, "Email Verification to DevHub", generateVerificationPage(vToken.Url));
        }
    }

    private string generateVerificationPage(string url)
    {
        return @"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Verification DevHub</title>
                        <style>
                            * {
                                box-sizing: border-box;
                                margin: 0;
                                padding: 0;
                            }
                            body { 
                                min-height: 100vh;
                                font-family: sans-serif;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                background-color: #0f1419;
                                color: whitesmoke;
                            }
                            #email-box
                            {
                                text-align: center;
                                width: 550px;
                                height: auto;
                                padding: 2rem;
                                border-radius: 6px;
                                box-shadow: 1px 1px 4px black;
                                background-color: #252b3d;
                            }
                            img {
                                filter: drop-shadow(5px 5px 10px rgba(0, 0, 0, 0.5));
                            }
                            p {
                                margin: auto;
                                color: whitesmoke;
                                font-size: 0.8rem;
                                width: 50%;
                                padding: 0.3rem;
                                margin-top: 0.81rem;
                             }
                             a {
                                display: block;
                                width: fit-content;
                                margin: auto;
                                margin-top: 0.81rem;
                                padding: 0.6rem 1.2rem;
                                border-radius: 0.5rem;
                                background-color: #8b5cf6;
                                color: whitesmoke;
                                font-size: 1rem;
                                text-decoration: none;
                            }
                            a:hover
                            {
                                box-shadow: 0px 2px 4px #3b82f6
                            }
                        </style>
                    </head>
                    <body>
                        <div>
                            <div id='email-box'>
                                <div class='img-cont'>
                                    <img src='https://localhost:7146/site-images/logo.png' width='60' height='60' />
                                </div>
                                <h2>Welcome to DevHub</h2>
                                <p>Please verify your email address by clicking the button below to complete setup.</p>
" + $@"
                                <a href='{url}'>Verify Email</a>
                            </div>
                        </div>    
                    </body>
                    </html>";
    }
}

