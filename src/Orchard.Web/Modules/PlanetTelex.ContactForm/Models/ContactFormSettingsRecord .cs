﻿using Orchard.ContentManagement.Records;

namespace PlanetTelex.ContactForm.Models
{
    public class ContactFormSettingsRecord : ContentPartRecord
    {
        public virtual bool EnableSpamProtection { get; set; }
        public virtual bool EnableSpamEmail { get; set; }
        public virtual string SpamEmail { get; set; }
    }
}