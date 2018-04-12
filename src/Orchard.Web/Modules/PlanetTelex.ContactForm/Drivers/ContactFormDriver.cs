using Orchard;
using Orchard.AntiSpam.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using PlanetTelex.ContactForm.Models;
using PlanetTelex.ContactForm.ViewModels;

namespace PlanetTelex.ContactForm.Drivers
{
    public class ContactFormDriver : ContentPartDriver<ContactFormPart>
    {
        private readonly IOrchardServices _orchardServices;

        public ContactFormDriver(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;         
        }

        /// <summary>
        /// Defines the shapes required for the part's main view.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="shapeHelper">The shape helper.</param>
        protected override DriverResult Display(ContactFormPart part, string displayType, dynamic shapeHelper)
        {
            
            var viewModel = new ContactFormViewModel();
            if (part != null && displayType.Contains("Detail"))
            {
                viewModel.ContentRecordId = part.Record.Id;
                viewModel.ShowSubjectField = !part.UseStaticSubject;
                viewModel.ShowNameField = part.DisplayNameField;
                viewModel.RequireNameField = part.RequireNameField;

                // Pull recaptcha settings from antispam settings part
                var recaptchaSettings = _orchardServices.WorkContext.CurrentSite.As<ReCaptchaSettingsPart>();
                viewModel.RecaptchaPublicKey = recaptchaSettings.PublicKey;
            }
            return ContentShape("Parts_ContactForm", () => shapeHelper.DisplayTemplate(TemplateName: "Parts/ContactForm", Model: viewModel, Prefix: Prefix));
        }

        /// <summary>
        /// Defines the shapes required for the editor view.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="shapeHelper">The shape helper.</param>
        protected override DriverResult Editor(ContactFormPart part, dynamic shapeHelper)
        {
            if (part == null)
                part = new ContactFormPart();

            return ContentShape("Parts_ContactForm_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/ContactForm", Model: part, Prefix: Prefix));
        }

        /// <summary>
        /// Runs upon the POST of the editor view.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="updater">The updater.</param>
        /// <param name="shapeHelper">The shape helper.</param>
        protected override DriverResult Editor(ContactFormPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (part == null || updater == null)
                return Editor(null, shapeHelper);
            
            if (updater.TryUpdateModel(part, Prefix, null, null)) {
                if (!part.DisplayNameField)
                {
                    part.RequireNameField = false;
                }   
            }

            return Editor(part, shapeHelper);
        }
    }
}