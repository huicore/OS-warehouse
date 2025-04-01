using System.Drawing;

namespace FixedAssetInventory.Resources.Themes
{
    public static class LightTheme
    {
        // Основные цвета
        public static Color BackgroundColor => Color.White;
        public static Color TextColor => Color.FromArgb(64, 64, 64);
        public static Color BorderColor => Color.FromArgb(230, 230, 230);
        public static Color HighlightColor => Color.FromArgb(0, 120, 215);

        // Цвета элементов управления
        public static class ControlColors
        {
            public static Color ButtonBackColor => Color.FromArgb(240, 240, 240);
            public static Color ButtonForeColor => TextColor;
            public static Color ButtonHoverColor => Color.FromArgb(230, 230, 230);
            public static Color ButtonPressedColor => Color.FromArgb(220, 220, 220);
            
            public static Color TextBoxBackColor => Color.White;
            public static Color TextBoxBorderColor => Color.FromArgb(204, 204, 204);
            
            public static Color ComboBoxBackColor => Color.White;
            public static Color ComboBoxArrowColor => Color.FromArgb(120, 120, 120);
            
            public static Color GridHeaderColor => HighlightColor;
            public static Color GridHeaderTextColor => Color.White;
            public static Color GridLineColor => Color.FromArgb(240, 240, 240);
            public static Color GridAlternatingRowColor => Color.FromArgb(248, 248, 248);
        }

        // Цвета панелей
        public static class PanelColors
        {
            public static Color MainPanelColor => Color.White;
            public static Color SecondaryPanelColor => Color.FromArgb(248, 248, 248);
            public static Color PanelBorderColor => Color.FromArgb(225, 225, 225);
        }

        // Цвета состояний
        public static class StateColors
        {
            public static Color SuccessColor => Color.FromArgb(46, 125, 50);
            public static Color WarningColor => Color.FromArgb(237, 108, 2);
            public static Color ErrorColor => Color.FromArgb(211, 47, 47);
            public static Color InfoColor => Color.FromArgb(2, 136, 209);
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
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                        
                    case ComboBox comboBox:
                        comboBox.BackColor = ControlColors.ComboBoxBackColor;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        break;
                        
                    case DataGridView grid:
                        grid.BackgroundColor = BackgroundColor;
                        grid.GridColor = ControlColors.GridLineColor;
                        grid.DefaultCellStyle.BackColor = BackgroundColor;
                        grid.DefaultCellStyle.ForeColor = TextColor;
                        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 243, 255);
                        grid.ColumnHeadersDefaultCellStyle.BackColor = ControlColors.GridHeaderColor;
                        grid.ColumnHeadersDefaultCellStyle.ForeColor = ControlColors.GridHeaderTextColor;
                        grid.AlternatingRowsDefaultCellStyle.BackColor = ControlColors.GridAlternatingRowColor;
                        break;
                }
            }
        }
    }
}