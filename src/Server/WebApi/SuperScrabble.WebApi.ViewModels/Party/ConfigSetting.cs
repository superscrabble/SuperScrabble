namespace SuperScrabble.WebApi.ViewModels.Party
{
    public class ConfigSetting
    {
        public string Name { get; set; } = default!; // ChooseTimerType

        public IEnumerable<SettingOption> Options { get; set; } = default!;
    }
}
