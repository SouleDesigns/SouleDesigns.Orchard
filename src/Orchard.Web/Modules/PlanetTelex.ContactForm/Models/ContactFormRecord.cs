﻿using Orchard.ContentManagement.Records;

namespace PlanetTelex.ContactForm.Models
{
    /// <summary>
    /// The fields this content part requires.
    /// </summary>
    public class ContactFormRecord : ContentPartRecord 
    {
        /// <summary>
        /// Gets or sets the recipient email address- the address the form will send an email to.
        /// </summary>
        public virtual string RecipientEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the static subject message- the subject of the email the recipient will receive.
        /// </summary>
        public virtual string StaticSubjectMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the static subject string, or provide an input on the form for it.
        /// </summary>
        public virtual bool UseStaticSubject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to provide an input on the form for a name field. This name will be included in the message.
        /// </summary>
        public virtual bool DisplayNameField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to require name field.
        /// </summary>
        public virtual bool RequireNameField { get; set; }
    }
}