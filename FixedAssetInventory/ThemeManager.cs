public static class ThemeManager
{
    public static Theme CurrentTheme { get; private set; } = new LightTheme();

    public static void SetTheme(string themeName)
    {
        CurrentTheme = themeName == "dark" ? new DarkTheme() : new LightTheme();
        ApplyTheme();
    }

    private static void ApplyTheme()
    {
        // Применение темы ко всем открытым формам
        foreach (Form form in Application.OpenForms)
        {
            form.BackColor = CurrentTheme.BackColor;
            form.ForeColor = CurrentTheme.ForeColor;
            
            // Рекурсивное применение темы ко всем контролам
            ApplyThemeToControls(form.Controls);
        }
    }

    private static void ApplyThemeToControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            if (control is Button button)
            {
                button.BackColor = CurrentTheme.ButtonBackColor;
                button.ForeColor = CurrentTheme.ButtonForeColor;
                button.FlatAppearance.MouseOverBackColor = CurrentTheme.ButtonHoverColor;
            }
            // Аналогично для других типов контролов
            
            if (control.HasChildren)
            {
                ApplyThemeToControls(control.Controls);
            }
        }
    }
}

public abstract class Theme
{
    public abstract Color BackColor { get; }
    public abstract Color ForeColor { get; }
    public abstract Color ButtonBackColor { get; }
    // ... другие свойства темы
}

public class LightTheme : Theme
{
    public override Color BackColor => Color.White;
    public override Color ForeColor => Color.Black;
    public override Color ButtonBackColor => Color.FromArgb(240, 240, 240);
    // ... другие свойства светлой темы
}

public class DarkTheme : Theme
{
    public override Color BackColor => Color.FromArgb(32, 32, 32);
    public override Color ForeColor => Color.White;
    public override Color ButtonBackColor => Color.FromArgb(64, 64, 64);
    // ... другие свойства темной темы
}