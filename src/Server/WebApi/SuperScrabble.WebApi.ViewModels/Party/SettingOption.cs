namespace SuperScrabble.WebApi.ViewModels.Party
{
    public class SettingOption
    {
        public string Name { get; set; } = default!;

        public int Value { get; set; } = default!;

        public bool IsSelected { get; set; }
    }
}
