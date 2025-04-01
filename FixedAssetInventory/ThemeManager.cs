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
        // ���������� ���� �� ���� �������� ������
        foreach (Form form in Application.OpenForms)
        {
            form.BackColor = CurrentTheme.BackColor;
            form.ForeColor = CurrentTheme.ForeColor;
            
            // ����������� ���������� ���� �� ���� ���������
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
            // ���������� ��� ������ ����� ���������
            
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
    // ... ������ �������� ����
}

public class LightTheme : Theme
{
    public override Color BackColor => Color.White;
    public override Color ForeColor => Color.Black;
    public override Color ButtonBackColor => Color.FromArgb(240, 240, 240);
    // ... ������ �������� ������� ����
}

public class DarkTheme : Theme
{
    public override Color BackColor => Color.FromArgb(32, 32, 32);
    public override Color ForeColor => Color.White;
    public override Color ButtonBackColor => Color.FromArgb(64, 64, 64);
    // ... ������ �������� ������ ����
}