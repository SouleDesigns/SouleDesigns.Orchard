using Orchard.ContentManagement;

namespace PlanetTelex.ContactForm.Models
{
    public class ContactFormSettingsPart : ContentPart<ContactFormSettingsRecord> 
    {
        public bool EnableSpamProtection
        {
            get { return Record.EnableSpamProtection; }
            set { Record.EnableSpamProtection = value; }
        }

        public bool EnableSpamEmail
        {
            get { return Record.EnableSpamEmail; }
            set { Record.EnableSpamEmail = value; }
        }

        public string SpamEmail 
        {
            get { return Record.SpamEmail; }
            set { Record.SpamEmail = value; }
        }
    }
}