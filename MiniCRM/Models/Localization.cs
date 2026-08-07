namespace MiniCRM.Models
{
    public class Localization
    {
        public int Id { get; set; }

        public string Key { get; set; } = string.Empty;

        public string Culture { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}