using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NuGet;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Modules.Services;
using Orchard.Mvc.Extensions;
using Orchard.Packaging.Services;
using Orchard.Packaging.ViewModels;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using IPackageManager = Orchard.Packaging.Services.IPackageManager;
using PackageBuilder = Orchard.Packaging.Services.PackageBuilder;

namespace Orchard.Packaging.Controllers {
    [Themed, Admin]
    public class PackagingServicesController : Controller {

        private readonly ShellSettings _shellSettings;
        private readonly IPackageManager _packageManager;
        private readonly IPackagingSourceManager _packagingSourceManager;
        private readonly IAppDataFolderRoot _appDataFolderRoot;
        private readonly INotifier _notifier;
        private readonly IModuleService _moduleService;
        private readonly IRecipeHarvester _recipeHarvester;
        private readonly IRecipeManager _recipeManager;

        public PackagingServicesController(
            ShellSettings shellSettings,
            IPackageManager packageManager,
            IPackagingSourceManager packagingSourceManager,
            INotifier notifier,
            IAppDataFolderRoot appDataFolderRoot,
            IOrchardServices services,
            IModuleService moduleService,
            IRecipeHarvester recipeHarvester,
            IRecipeManager recipeManager) {

            _shellSettings = shellSettings;
            _packageManager = packageManager;
            _notifier = notifier;
            _appDataFolderRoot = appDataFolderRoot;
            _moduleService = moduleService;
            _recipeHarvester = recipeHarvester;
            _recipeManager = recipeManager;
            _packagingSourceManager = packagingSourceManager;
            Services = services;

            T = NullLocalizer.Instance;
            Logger = Logging.NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }
        public Logging.ILogger Logger { get; set; }

        public ActionResult AddTheme(string returnUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to add themes")))
                return new HttpUnauthorizedResult();

            return View();
        }

        [HttpPost, ActionName("RemoveTheme")]
        public ActionResult RemoveThemePOST(string themeId, string returnUrl, string retryUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to remove themes")))
                return new HttpUnauthorizedResult();

            return UninstallPackage(PackageBuilder.BuildPackageId(themeId, DefaultExtensionTypes.Theme), returnUrl, retryUrl);
        }

        public ActionResult AddModule(string returnUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to add modules")))
                return new HttpUnauthorizedResult();

            return View();
        }

        public ActionResult InstallGallery(string packageId, string version, int sourceId, string redirectUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to add sources")))
                return new HttpUnauthorizedResult();

            try {
                var source = _packagingSourceManager.GetSources().Where(s => s.Id == sourceId).FirstOrDefault();
                if (source == null) {
                    return HttpNotFound();
                }

                PackageInfo packageInfo = _packageManager.Install(packageId, version, source.FeedUrl, HostingEnvironment.MapPath("~/"));

                if (DefaultExtensionTypes.IsTheme(packageInfo.ExtensionType)) {
                    Services.Notifier.Information(T("The theme has been successfully installed. It can be enabled in the \"Themes\" page accessible from the menu."));
                } else if (DefaultExtensionTypes.IsModule(packageInfo.ExtensionType)) {
                    Services.Notifier.Information(T("The module has been successfully installed."));

                    IPackageRepository packageRepository = PackageRepositoryFactory.Default.CreateRepository(new PackageSource(source.FeedUrl, "Default"));
                    IPackage package = packageRepository.FindPackage(packageId);
                    ExtensionDescriptor extensionDescriptor = _packageManager.GetExtensionDescriptor(package, packageInfo.ExtensionType);

                    return InstallPackageDetails(extensionDescriptor, redirectUrl);
                }
            }
            catch (Exception exception) {
                this.Error(exception, T("Package installation failed."), Logger, Services.Notifier);
            }

            return Redirect(redirectUrl);
        }

        public ActionResult InstallLocally(string redirectUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to install packages")))
                return new HttpUnauthorizedResult();

            try {
                if (Request.Files == null ||
                    Request.Files.Count == 0 ||
                    string.IsNullOrWhiteSpace(Request.Files.Get(0).FileName)) {

                    throw new OrchardException(T("Select a file to upload."));
                }

                HttpPostedFileBase file = Request.Files.Get(0);
                string fullFileName = Path.Combine(_appDataFolderRoot.RootFolder, Path.GetFileName(file.FileName)).Replace(Path.DirectorySeparatorChar, '/');
                file.SaveAs(fullFileName);
                ZipPackage package = new ZipPackage(fullFileName);
                PackageInfo packageInfo = _packageManager.Install(package, _appDataFolderRoot.RootFolder, HostingEnvironment.MapPath("~/"));
                ExtensionDescriptor extensionDescriptor = _packageManager.GetExtensionDescriptor(package, packageInfo.ExtensionType);
                System.IO.File.Delete(fullFileName);

                if (DefaultExtensionTypes.IsTheme(extensionDescriptor.ExtensionType)) {
                    Services.Notifier.Information(T("The theme has been successfully installed. It can be enabled in the \"Themes\" page accessible from the menu."));
                } else if (DefaultExtensionTypes.IsModule(extensionDescriptor.ExtensionType)) {
                    Services.Notifier.Information(T("The module has been successfully installed."));

                    return InstallPackageDetails(extensionDescriptor, redirectUrl);
                }
            }
            catch (Exception exception) {
                this.Error(exception, T("Package uploading and installation failed."), Logger, Services.Notifier);
            }

            return Redirect(redirectUrl);
        }

        private ActionResult InstallPackageDetails(ExtensionDescriptor extensionDescriptor, string redirectUrl) {
            if (DefaultExtensionTypes.IsModule(extensionDescriptor.ExtensionType)) {
                List<PackagingInstallFeatureViewModel> features = extensionDescriptor.Features
                    .Where(featureDescriptor => !DefaultExtensionTypes.IsTheme(featureDescriptor.Extension.ExtensionType))
                    .Select(featureDescriptor => new PackagingInstallFeatureViewModel {
                        Enable = true, // by default all features are enabled
                        FeatureDescriptor = featureDescriptor
                    }).ToList();

                List<PackagingInstallRecipeViewModel> recipes = _recipeHarvester.HarvestRecipes(extensionDescriptor.Id)
                    .Select(recipe => new PackagingInstallRecipeViewModel {
                        Execute = false, // by default no recipes are executed
                        Recipe = recipe
                    }).ToList();

                if (features.Count > 0) {
                    return View("InstallModuleDetails", new PackagingInstallViewModel {
                        Features = features,
                        Recipes = recipes,
                        ExtensionDescriptor = extensionDescriptor
                    });
                }
            }

            return Redirect(redirectUrl);
        }

        [HttpPost, ActionName("InstallPackageDetails")]
        public ActionResult InstallPackageDetailsPOST(PackagingInstallViewModel packagingInstallViewModel, string redirectUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to add sources")))
                return new HttpUnauthorizedResult();

            try {
                IEnumerable<Recipe> recipes = _recipeHarvester.HarvestRecipes(packagingInstallViewModel.ExtensionDescriptor.Id)
                    .Where(loadedRecipe => packagingInstallViewModel.Recipes.FirstOrDefault(recipeViewModel => recipeViewModel.Execute && recipeViewModel.Recipe.Name.Equals(loadedRecipe.Name)) != null);

                foreach (Recipe recipe in recipes) {
                    try {
                        _recipeManager.Execute(recipe);
                    }
                    catch {
                        Services.Notifier.Error(T("Recipes contains {0} unsuported module installation steps.", recipe.Name));
                    }
                }

                // Enable selected features
                if (packagingInstallViewModel.Features.Count > 0) {
                    IEnumerable<string> featureIds = packagingInstallViewModel.Features
                        .Where(feature => feature.Enable)
                        .Select(feature => feature.FeatureDescriptor.Id);

                    // Enable the features and its dependencies
                    _moduleService.EnableFeatures(featureIds, true);
                }
            } catch (Exception exception) {
                Services.Notifier.Error(T("Post installation steps failed with error: {0}", exception.Message));
            }

            return Redirect(redirectUrl);
        }

        public ActionResult UninstallPackage(string id, string returnUrl, string retryUrl) {
            if (_shellSettings.Name != ShellSettings.DefaultName || !Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to uninstall packages")))
                return new HttpUnauthorizedResult();

            try {
                _packageManager.Uninstall(id, HostingEnvironment.MapPath("~/"));

                _notifier.Information(T("Uninstalled package \"{0}\"", id));

                return this.RedirectLocal(returnUrl, "~/");
            } catch (Exception exception) {
                this.Error(exception, T("Uninstall failed: {0}", exception.Message), Logger, Services.Notifier);

                return Redirect(retryUrl);
            }
        }
    }
}