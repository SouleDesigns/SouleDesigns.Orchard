using System;
using System.Web;
using System.Web.Mvc;
using Orchard.Experimental.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.UI.Admin;

namespace Orchard.Experimental.Controllers {

    [Themed, Admin]
    public class HomeController : Controller {
        private readonly INotifier _notifier;

        public HomeController(INotifier notifier, IShapeFactory shapeFactory) {
            _notifier = notifier;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }

        public Localizer T { get; set; }

        public ActionResult Index() {
            return View();
        }

        public ActionResult NotAuthorized() {
            _notifier.Warning(T("Simulated error goes here."));
            return new HttpUnauthorizedResult();
        }

        public ActionResult Simple() {
            return View(new Simple { Title = "This is a simple text", Quantity = 5 });
        }

        public ActionResult _RenderableAction() {
            return PartialView("_RenderableAction", "This is render action");
        }

        public ActionResult SimpleMessage() {
            _notifier.Information(T("Notifier works without BaseViewModel"));
            return RedirectToAction("Simple");
        }

        [Themed(false)]
        public ActionResult SimpleNoTheme() {
            return View("Simple", new Simple { Title = "This is not themed", Quantity = 5 });
        }

        public ActionResult FormShapes() {
            var model = Shape.Form()
                .Fieldsets(Shape.Fieldsets(typeof(Array))
                    .Add(Shape.Fieldset(typeof(Array)).Name("site")
                        .Add(Shape.InputText().Name("SiteName").Text(T("Site Name")).Value(T("some default/pre-pop value...").Text))
                    )
                    .Add(Shape.Fieldset(typeof(Array)).Name("admin")
                        .Add(Shape.InputText().Name("AdminUsername").Text(T("Admin Username")))
                        .Add(Shape.InputPassword().Name("AdminPassword").Text(T("Admin Password")))
                    )
                    .Add(Shape.Fieldset(typeof(Array)).Name("actions")
                        .Add(Shape.FormSubmit().Text(T("Finish Setup")))
                    )
                );

            // get at the first input?
            model.Fieldsets[0][0].Attributes(new {autofocus = "autofocus"}); // <-- could be applied by some other behavior - need to be able to modify attributes instead of clobbering them like this

            return View(model);
        }

        [HttpPost, ActionName("FormShapes")]
        public ActionResult FormShapesPOST() {
            //all reqs are fail
            ModelState.AddModelError("AdminUsername", "The admin username is wrong.");
            return FormShapes();
        }

        public ActionResult UsingShapes() {

            ViewModel.Page = Shape.Page()
                .Main(Shape.Zone(typeof (Array), Name: "Main"))
                .Messages(Shape.Zone(typeof (Array), Name: "Messages"))
                .Sidebar(Shape.Zone(typeof (Array), Name: "Sidebar"));

            //ViewModel.Page.Add("Messages:5", New.Message(Content: T("This is a test"), Severity: "Really bad!!!"));

            ViewModel.Page.Messages.Add(
                Shape.Message(Content: T("This is a test"), Severity: "Really bad!!!"));

            ViewModel.Page.Sidebar.Add(
                Shape.Link(Url: "http://orchard.codeplex.com", Content: Shape.Image(Url: "http://orchardproject.net/Content/images/orchardLogo.jpg").Attributes(new { @class = "bigredborderfromabadclassname" })));

            var model = Shape.Message(
                Content: Shape.Explosion(Height: 100, Width: 200),
                Severity: "Meh");

            ViewModel.Page.Messages.Add(new HtmlString("<hr/>abuse<hr/>"));
            ViewModel.Page.Messages.Add("<hr/>encoded<hr/>");

            return View(model);
        }

        public static string Break(dynamic view) {
            return view.Model.Box.Title;
        }
    }

}