using Orchard.ContentManagement;

namespace PlanetTelex.ContactForm.Models
{
    /// <summary>
    /// The content part model, uses the record class for storage.
    /// </summary>
    public class ContactFormPart : ContentPart<ContactFormRecord>
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string RecipientEmailAddress 
        {
            get { return Record.RecipientEmailAddress; }
            set { Record.RecipientEmailAddress = value; }
        }

        /// <summary>
        /// Gets or sets the static subject message.
        /// </summary>
        public string StaticSubjectMessage
        {
            get { return Record.StaticSubjectMessage; }
            set { Record.StaticSubjectMessage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the static subject message.
        /// </summary>
        public bool UseStaticSubject
        {
            get { return Record.UseStaticSubject; }
            set { Record.UseStaticSubject = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the name field.
        /// </summary>
        public bool DisplayNameField
        {
            get { return Record.DisplayNameField; }
            set { Record.DisplayNameField = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require the name field.
        /// </summary>
        public bool RequireNameField
        {
            get { return Record.RequireNameField; }
            set { Record.RequireNameField = value; }
        }
    }
}