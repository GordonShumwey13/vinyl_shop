namespace VinylShop.Utils
{
    public static class SongDuration
    {
        public const string FormatHint = "мм:сс або гг:мм:сс, наприклад 03:45";

        // "03:45" — це 3 хвилини 45 секунд (а не 3 години, як у TimeSpan.TryParse), "01:02:03" — години:хвилини:секунди
        public static bool TryParse(string? value, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;
            if (string.IsNullOrWhiteSpace(value)) return false;

            var parts = value.Trim().Split(':');
            if (parts.Length is < 2 or > 3) return false;

            var numbers = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out numbers[i]) || numbers[i] < 0) return false;
            }

            int hours = parts.Length == 3 ? numbers[0] : 0;
            int minutes = numbers[^2];
            int seconds = numbers[^1];

            if (seconds > 59 || (parts.Length == 3 && minutes > 59)) return false;

            duration = new TimeSpan(hours, minutes, seconds);
            return duration > TimeSpan.Zero;
        }
    }
}
