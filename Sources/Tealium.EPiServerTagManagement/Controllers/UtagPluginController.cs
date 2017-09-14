using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Tealium.EPiServerTagManagement.Models;
using log4net;
using Tealium.EPiServerTagManagement.Business.Extensions;
using Tealium.EPiServerTagManagement.Business.Models;
using Tealium.EPiServerTagManagement.Business.Services;

namespace Tealium.EPiServerTagManagement.Controllers
{
    [Authorize(Roles = "CmsAdmins, CmsEditors, WebAdmins, WebEditors")]
    [GuiPlugIn(Area = PlugInArea.AdminConfigMenu, Url = "/UtagPlugin", DisplayName = "Tealium Tag Management")]
    public class UtagPluginController : Controller
    {
        public const string DefaultListString = "-------------";

        private readonly SiteDefinitionRepository siteDefinitionRepository;
        private readonly ILanguageBranchRepository languageBranchRepository;
        private readonly IContentRepository contentRepository;
        private readonly IUtagConfigurationService utagConfigurationService;
        private readonly IUtagPageTypeService utagPageTypeService;
        private readonly ILog log = LogManager.GetLogger(typeof(UtagPluginController));

        public UtagPluginController()
        {
            this.siteDefinitionRepository = ServiceLocator.Current.GetInstance<SiteDefinitionRepository>();
            this.languageBranchRepository = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
            this.contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            this.utagConfigurationService = new UtagConfigurationService();
            this.utagPageTypeService = new UtagPageTypeService();
        }

        /// <summary>
        /// Index Action. GET Http request.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var viewModel = this.GenerateUtagPluginViewModel();
            string language = string.Empty;
            string siteName = string.Empty;

            var currentSite = this.siteDefinitionRepository.List().FirstOrDefault(x => x.SiteUrl.Equals(UriSupport.SiteUrl));
            if (currentSite != null)
            {
                siteName = currentSite.Name;
                language = this.GetMasterLanguage(currentSite);
            }

            this.PopulateSiteData(viewModel, siteName, language, string.Empty);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ChangeSiteName(string websiteName)
        {
            var viewModel = this.GenerateUtagPluginViewModel();

            if (websiteName.IsNullOrEmpty())
            {
                return View("Index", viewModel);
            }

            string language = string.Empty;
            var siteDefinition = this.siteDefinitionRepository.List().FirstOrDefault(x => x.Name.Equals(websiteName));
            if (siteDefinition != null)
            {
                language = this.GetMasterLanguage(siteDefinition);
            }
            
            this.PopulateSiteData(viewModel, websiteName, language, string.Empty);

            return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult ChangeLanguage(string websiteName, string language)
        {
            var viewModel = this.GenerateUtagPluginViewModel();

            if (websiteName.IsNullOrEmpty() || language.IsNullOrEmpty())
            {
                return View("Index", viewModel);
            }

            this.PopulateSiteData(viewModel, websiteName, language, string.Empty);

            return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult LoadPageTypeData(string websiteName, string language, string pageTypeName)
        {
            var viewModel = this.GenerateUtagPluginViewModel();

            if (websiteName.IsNullOrEmpty() || language.IsNullOrEmpty())
            {
                return View("Index", viewModel);
            }

            this.PopulateSiteData(viewModel, websiteName, language, pageTypeName);

            viewModel.ActiveTab = UtagPluginTab.Cms.ToString();

            return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult SaveMainSettings(MainSettingsViewModel viewmodel)
        {
            var indexViewModel = SaveAction<MainSettingsViewModel>(
                viewmodel,
                string.Empty,
                UtagPluginTab.MainConfiguration,
                null,
                "Main settings were successfully updated",
                this.utagConfigurationService);
            
            return View("Index", indexViewModel);
        }

        [HttpPost]
        public ActionResult SaveCommonTags(CommonTagsViewModel viewmodel, IEnumerable<string> checkboxTags)
        {
            var indexViewModel = SaveAction<CommonTagsViewModel>(
                viewmodel,
                string.Empty,
                UtagPluginTab.MainConfiguration,
                checkboxTags,
                "Common tags were successfully updated",
                this.utagConfigurationService);

            return View("Index", indexViewModel);
        }

        [HttpPost]
        public JsonResult SaveCustomTags(CustomTagsViewModel viewmodel, IEnumerable<string> tags)
        {
            var indexViewModel = this.SaveAction<CustomTagsViewModel>(
                viewmodel,
                string.Empty,
                UtagPluginTab.MainConfiguration,
                tags,
                "Custom tags were successfully updated",
                this.utagConfigurationService);

            return Json(new { ActionMessage = ViewBag.ActionMessage }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult SavePageTypeTags(PageTypeViewModel viewmodel, IEnumerable<string> tags)
        {
            var viewresult = SaveAction<PageTypeViewModel>(
                viewmodel,
                viewmodel.PageTypeName,
                UtagPluginTab.Cms,
                tags,
                "PageType tags were successfully updated",
                this.utagPageTypeService);

            return Json(new { ActionMessage = ViewBag.ActionMessage }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult SaveAdvancedSettings(AdvancedSettingsViewModel viewmodel)
        {
            var indexViewModel = this.SaveAction<AdvancedSettingsViewModel>(
                viewmodel,
                string.Empty,
                UtagPluginTab.AdvancedSettings,
                null,
                "Advanced settings were successfully updated",
                this.utagConfigurationService);

            return View("Index", indexViewModel);
        }

        [HttpPost]
        public ActionResult HardResetOfDataStore(string names)
        {
            if (names == "UtagConfigurationStoreUtagPageTypeStore")
            {
                this.utagConfigurationService.HardReset();
                this.utagPageTypeService.HardReset();
            }

            return Json("the datastore has been cleaned", JsonRequestBehavior.DenyGet);
        }

        private UtagPluginViewModel GenerateUtagPluginViewModel()
        {
            var viewModel = new UtagPluginViewModel();

            ViewBag.SitesList = MainSettingsViewModel.GetSites(this.siteDefinitionRepository);

            ViewBag.AvailableLanguages = MainSettingsViewModel.GetAvailableLanguages(this.languageBranchRepository);

            ViewBag.PageTypes = PageTypeViewModel.GetPageTypes();

            viewModel.ActiveTab = UtagPluginTab.MainConfiguration.ToString();

            return viewModel;
        }

        private string GetMasterLanguage(SiteDefinition currentSite)
        {
            string language = string.Empty;
            var languages = ViewBag.AvailableLanguages as IEnumerable<KeyValuePair<string, string>>;
            if (languages != null)
            {
                language = languages.FirstOrDefault().Key;
            }

            try
            {
                language = this.contentRepository.Get<PageData>(currentSite.StartPage).MasterLanguageBranch;
            }
            catch (TypeMismatchException ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
            }
            catch (ContentNotFoundException ex)
            {
                this.log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
            }

            return language;
        }

        private void PopulateSiteData(UtagPluginViewModel viewModel, string sitename, string language, string pageType)
        {
            viewModel.MainConfiguration.WebsiteName = sitename;
            viewModel.MainConfiguration.Language = language;

            viewModel.AdvancedSettings.WebsiteName = sitename;
            viewModel.AdvancedSettings.Language = language;

            var siteSettings = this.utagConfigurationService.Get(sitename, language);

            if (siteSettings != null)
            {
                viewModel.MainConfiguration.Enabled = siteSettings.Enabled;
                viewModel.MainConfiguration.Account = siteSettings.Account;
                viewModel.MainConfiguration.Profile = siteSettings.Profile;
                viewModel.MainConfiguration.Environment = siteSettings.Environment;
                viewModel.MainConfiguration.EnableUtagJs = siteSettings.EnableUtagJs;

                viewModel.AdvancedSettings.EnableCustomUdo = siteSettings.EnableCustomUdo;
                viewModel.AdvancedSettings.CustomUdoAssembly = siteSettings.CustomUdoAssembly;
            }

            viewModel.CommonTags = new CommonTagsViewModel
            {
                WebsiteName = sitename,
                Language = language,
                SiteTags = siteSettings != null ? siteSettings.GetCommonTags() : new Dictionary<string, string>(),
                AllTags = CommonPropertyTags.Instance.List
            };

            viewModel.CustomTags = new CustomTagsViewModel
            {
                WebsiteName = sitename,
                Language = language,
                Tags = siteSettings != null ? siteSettings.GetCustomTags() : new Dictionary<string, string>()
            };

            viewModel.PageTypeTags = new PageTypeViewModel
            {
                WebsiteName = sitename,
                Language = language
            };

            if (pageType.IsNotNullOrEmpty())
            {
                var pageTypeSettings = this.utagPageTypeService.Get(sitename, language, pageType);

                viewModel.PageTypeTags.PageTypeName = pageType;
                viewModel.PageTypeTags.Tags = pageTypeSettings != null
                                                  ? pageTypeSettings.GetContentTypeTags()
                                                  : new Dictionary<string, string>();
            }
        }

        private UtagPluginViewModel SaveAction<T>(
            T viewmodel, 
            string pagetype, 
            UtagPluginTab tab,
            IEnumerable<string> tags,
            string message,
            IUtagService service) where T : BaseViewModel
        {
            viewmodel.Save(service, tags);

            var indexViewModel = this.GenerateUtagPluginViewModel();
            indexViewModel.ActiveTab = tab.ToString();

            this.PopulateSiteData(indexViewModel, viewmodel.WebsiteName, viewmodel.Language, pagetype);

            ViewBag.ActionMessage = message;

            return indexViewModel;
        }
    }
}