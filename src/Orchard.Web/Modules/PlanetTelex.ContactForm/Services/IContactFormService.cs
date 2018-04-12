using Orchard;
using PlanetTelex.ContactForm.Models;

namespace PlanetTelex.ContactForm.Services
{
    public interface IContactFormService : IDependency
    {
        /// <summary>
        /// Gets the contact form record.
        /// </summary>
        /// <param name="id">The record id.</param>
        ContactFormRecord GetContactForm(int id);

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
        void SendContactEmail(string name, string email, string spamBotEmail, string subject, string message, string sendTo, bool requiredName, string recaptcha);
    }
}
