using System.Drawing;
using System.Windows.Forms;

namespace FixedAssetInventory.Resources.Themes
{
    public static class DarkTheme
    {
        // Основные цвета
        public static Color BackgroundColor => Color.FromArgb(32, 32, 32);
        public static Color TextColor => Color.FromArgb(240, 240, 240);
        public static Color BorderColor => Color.FromArgb(64, 64, 64);
        public static Color HighlightColor => Color.FromArgb(0, 120, 215);

        // Цвета элементов управления
        public static class ControlColors
        {
            public static Color ButtonBackColor => Color.FromArgb(51, 51, 51);
            public static Color ButtonForeColor => TextColor;
            public static Color ButtonHoverColor => Color.FromArgb(70, 70, 70);
            public static Color ButtonPressedColor => Color.FromArgb(90, 90, 90);
            
            public static Color TextBoxBackColor => Color.FromArgb(40, 40, 40);
            public static Color TextBoxBorderColor => Color.FromArgb(80, 80, 80);
            
            public static Color ComboBoxBackColor => Color.FromArgb(40, 40, 40);
            public static Color ComboBoxArrowColor => Color.FromArgb(180, 180, 180);
            
            public static Color GridHeaderColor => Color.FromArgb(45, 45, 45);
            public static Color GridHeaderTextColor => TextColor;
            public static Color GridLineColor => Color.FromArgb(60, 60, 60);
            public static Color GridAlternatingRowColor => Color.FromArgb(38, 38, 38);
        }

        // Цвета панелей
        public static class PanelColors
        {
            public static Color MainPanelColor => Color.FromArgb(42, 42, 42);
            public static Color SecondaryPanelColor => Color.FromArgb(35, 35, 35);
            public static Color PanelBorderColor => Color.FromArgb(70, 70, 70);
        }

        // Цвета состояний
        public static class StateColors
        {
            public static Color SuccessColor => Color.FromArgb(76, 175, 80);
            public static Color WarningColor => Color.FromArgb(255, 152, 0);
            public static Color ErrorColor => Color.FromArgb(244, 67, 54);
            public static Color InfoColor => Color.FromArgb(33, 150, 243);
        }

        // Шрифты
        public static class Fonts
        {
            public static Font Default => new Font("Segoe UI", 9);
            public static Font Header => new Font("Segoe UI", 12, FontStyle.Bold);
            public static Font Title => new Font("Segoe UI", 10, FontStyle.Bold);
            public static Font GridHeader => new Font("Segoe UI", 9, FontStyle.Bold);
        }

        // Методы применения темы
        public static void ApplyTo(Control control)
        {
            control.BackColor = BackgroundColor;
            control.ForeColor = TextColor;
            control.Font = Fonts.Default;

            foreach (Control childControl in control.Controls)
            {
                ApplyTo(childControl);
                
                switch (childControl)
                {
                    case Button button:
                        button.BackColor = ControlColors.ButtonBackColor;
                        button.ForeColor = ControlColors.ButtonForeColor;
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderColor = BorderColor;
                        button.FlatAppearance.MouseOverBackColor = ControlColors.ButtonHoverColor;
                        button.FlatAppearance.MouseDownBackColor = ControlColors.ButtonPressedColor;
                        break;
                        
                    case TextBox textBox:
                        textBox.BackColor = ControlColors.TextBoxBackColor;
                        textBox.ForeColor = TextColor;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                        
                    case ComboBox comboBox:
                        comboBox.BackColor = ControlColors.ComboBoxBackColor;
                        comboBox.ForeColor = TextColor;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        break;
                        
                    case DataGridView grid:
                        grid.BackgroundColor = BackgroundColor;
                        grid.GridColor = ControlColors.GridLineColor;
                        grid.DefaultCellStyle.BackColor = BackgroundColor;
                        grid.DefaultCellStyle.ForeColor = TextColor;
                        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(55, 55, 55);
                        grid.ColumnHeadersDefaultCellStyle.BackColor = ControlColors.GridHeaderColor;
                        grid.ColumnHeadersDefaultCellStyle.ForeColor = ControlColors.GridHeaderTextColor;
                        grid.AlternatingRowsDefaultCellStyle.BackColor = ControlColors.GridAlternatingRowColor;
                        break;
                        
                    case Panel panel:
                        panel.BackColor = PanelColors.MainPanelColor;
                        panel.BorderStyle = BorderStyle.FixedSingle;
                        break;
                }
            }
        }
    }
}