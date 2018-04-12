using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Email.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using PlanetTelex.ContactForm.Models;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Orchard.AntiSpam.Models;
using Orchard.Services;
using System.Globalization;
using Orchard.AntiSpam.ViewModels;

namespace PlanetTelex.ContactForm.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly IJsonConverter _jsonConverter;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _orchardServices;        
        private readonly IRepository<ContactFormRecord> _contactFormRepository;                
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        private const string ReCaptchaSecureUrl = "https://www.google.com/recaptcha/api/siteverify";


        public ContactFormService(IOrchardServices orchardServices, INotifier notifier, IRepository<ContactFormRecord> contactFormRepository, IJsonConverter jsonConverter)
        {
            _notifier = notifier;
            _orchardServices = orchardServices;
            _contactFormRepository = contactFormRepository;
            _jsonConverter = jsonConverter;
            Logger = NullLogger.Instance;
        }
        
        #region Implementation of IContactFormService

        /// <summary>
        /// Gets the contact form record.
        /// </summary>
        /// <param name="id">The record id.</param>
        public ContactFormRecord GetContactForm(int id)
        {
            return _contactFormRepository.Get(id);
        }

        /// <summary>
        /// Sends a contact email.
        /// </summary>
        /// <param name="name">The name of the sender.</param>
        /// <param name="email">The email address of the sender.</param>
        /// <param name="spamBotEmail">The email address entered in by spam bot</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The email message.</param>
        /// <param name="sendTo">The email address to send the message to.</param>
        /// <param name="requiredName">Bool of Name is required</param>
        /// <param name="recaptcha">Recaptcha response string to be validated prior to sending</param>
        public void SendContactEmail(string name, string email, string spamBotEmail, string subject, string message, string sendTo, bool requiredName, string recaptcha)
        {
            if (ValidateContactFields(name, email, message, requiredName, recaptcha))
            {
                if (string.IsNullOrEmpty(name))
                    name = email;

                var smtpSettings = _orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();
                if (smtpSettings != null && smtpSettings.IsValid())
                {
                    var mailClient = BuildSmtpClient(smtpSettings);
                    var contactFormSettings = _orchardServices.WorkContext.CurrentSite.As<ContactFormSettingsPart>().Record;

                    if (!string.IsNullOrEmpty(spamBotEmail))
                    {
                        // This option allows spam to be sent to a separate email address.
                        if (contactFormSettings.EnableSpamEmail && !string.IsNullOrEmpty(contactFormSettings.SpamEmail))
                        {
                            try
                            {
                                var mailMessage = new MailMessage(email, contactFormSettings.SpamEmail, subject, name + " writes:\r\n\r\n" + message) {IsBodyHtml = false};
                                mailClient.Send(mailMessage);
                                string spamMessage = string.Format("Your message was flagged as spam. If you feel this was in error contact us directly at: {0}", sendTo);
                                _notifier.Information(T(spamMessage));
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, "An unexpected error while sending a contact form message flagged as spam to {0} at {1}", contactFormSettings.SpamEmail, DateTime.Now.ToLongDateString());
                                string errorMessage = string.Format("Your message was flagged as spam. If you feel this was in error contact us directly at: {0}", sendTo);
                                _notifier.Error(T(errorMessage));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var mailMessage = new MailMessage(email, sendTo, subject, name + " writes:\r\n\r\n" + message) {IsBodyHtml = false};
                            mailClient.Send(mailMessage);
                            Logger.Debug("Contact form message sent to {0} at {1}", sendTo, DateTime.Now.ToLongDateString());
                            _notifier.Information(T("Thank you for your inquiry, we will respond to you shortly."));
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "An unexpected error while sending a contact form message to {0} at {1}", sendTo, DateTime.Now.ToLongDateString());
                            var errorMessage = string.Format("An unexpected error occured when sending your message. You may email us directly at: {0}", sendTo);
                            _notifier.Error(T(errorMessage));
                        }
                    }
                }
                else
                {
                    string errorMessage = string.Format("Our email server isn't configured. You may email us directly at: {0}", sendTo);
                    _notifier.Error(T(errorMessage));
                }
            }
        }

        #endregion

        private SmtpClient BuildSmtpClient(SmtpSettingsPart smtpSettings)
        {
            var mailClient = new SmtpClient {
                Host = smtpSettings.Host, 
                Port = smtpSettings.Port, 
                EnableSsl = smtpSettings.EnableSsl, 
                DeliveryMethod = SmtpDeliveryMethod.Network, 
                UseDefaultCredentials = !smtpSettings.RequireCredentials
            };
            if (!mailClient.UseDefaultCredentials && !String.IsNullOrWhiteSpace(smtpSettings.UserName))
            {
                mailClient.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
            }
            return mailClient;
        }

        private bool ValidateContactFields(string name, string email, string message, bool nameRequired, string recaptcha)
        {            
            var isValid = true;
                
            const string emailAddressRegex = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

            if ((nameRequired && String.IsNullOrEmpty(name)) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(recaptcha)) {
                _notifier.Error(T("All contact fields are required."));
                isValid = false;
            }
            else
            {
                Match emailMatch = Regex.Match(email, emailAddressRegex);
                if (!emailMatch.Success)
                {
                    _notifier.Error(T("Invalid email address."));
                    isValid = false;
                }

                if (!ValidateRecaptcha(recaptcha))
                {
                    _notifier.Error(T("Are you a sure you're not a robot?"));
                    isValid = false;
                }
            }            

            return isValid;
        }


        private bool ValidateRecaptcha(string recaptcha)
        {
            // Pull recaptcha settings from antispam settings part
            var recaptchaSettings = _orchardServices.WorkContext.CurrentSite.As<ReCaptchaSettingsPart>();
            var privateKey = recaptchaSettings.PrivateKey;
            var remoteip = _orchardServices.WorkContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            var result = string.Empty;

            // Format post data
            var postData = String.Format(CultureInfo.InvariantCulture,
                "secret={0}&response={1}&remoteip={2}",
                privateKey,
                recaptcha,
                remoteip
            );

            WebRequest request = WebRequest.Create(ReCaptchaSecureUrl + "?" + postData);
            request.Method = "GET";
            request.Timeout = 5000; //milliseconds
            request.ContentType = "application/x-www-form-urlencoded";

            // Get response result
            using (WebResponse webResponse = request.GetResponse())
            {
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            // Parse json result and return 
            ReCaptchaPartResponseModel responseModel = _jsonConverter.Deserialize<ReCaptchaPartResponseModel>(result);
            return responseModel.Success;
        }

    }
}