namespace Tealium.EPiServerTagManagement.Models
{
    public class UtagPluginViewModel
    {
        public UtagPluginViewModel()
        {
            this.MainConfiguration = new MainSettingsViewModel();
            this.AdvancedSettings = new AdvancedSettingsViewModel();
            this.CommonTags = new CommonTagsViewModel();
            this.CustomTags = new CustomTagsViewModel();
            this.PageTypeTags = new PageTypeViewModel();
        }

        public MainSettingsViewModel MainConfiguration { get; set; }

        public AdvancedSettingsViewModel AdvancedSettings { get; set; }
        
        public CommonTagsViewModel CommonTags { get; set; }

        public CustomTagsViewModel CustomTags { get; set; }

        public PageTypeViewModel PageTypeTags { get; set; }

        public string ActiveTab { get; set; }
    }

    public enum UtagPluginTab
    {
        MainConfiguration,
        AdvancedSettings,
        Cms
    }
}
