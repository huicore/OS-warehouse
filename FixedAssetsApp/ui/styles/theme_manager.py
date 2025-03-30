from PyQt6.QtCore import QFile, QTextStream

class ThemeManager:
    @staticmethod
    def load_theme(app, theme_name):
        """Загрузка темы из QSS файла"""
        theme_file = QFile(f":/ui/styles/{theme_name}_theme.qss")
        if theme_file.open(QFile.OpenModeFlag.ReadOnly | QFile.OpenModeFlag.Text):
            stream = QTextStream(theme_file)
            app.setStyleSheet(stream.readAll())
            theme_file.close()