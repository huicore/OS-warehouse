from PyQt6.QtWidgets import (QMainWindow, QTableView, QVBoxLayout, QWidget, 
                            QToolBar, QStatusBar, QMessageBox, QApplication)
from PyQt6.QtGui import QAction, QIcon
from PyQt6.QtCore import Qt
from utils.printer import AssetPrinter
from ui.dialogs import AssetDialog
from ui.styles.theme_manager import ThemeManager

class MainWindow(QMainWindow):
    def __init__(self, db_manager):
        super().__init__()
        self.setWindowTitle("Учет основных средств")
        self.resize(1200, 800)
        self.db_manager = db_manager
        self.dark_theme = False  # Флаг текущей темы
        
        # Центральный виджет
        self.table_view = QTableView()
        self.setCentralWidget(self.table_view)
        
        # Создание тулбара
        self._create_toolbar()
        
        # Статус бар
        self.statusBar().showMessage("Готово")
        
    def _create_toolbar(self):
        toolbar = QToolBar("Главное меню")
        toolbar.setMovable(False)
        self.addToolBar(Qt.ToolBarArea.LeftToolBarArea, toolbar)
        
        # Кнопки
        actions = [
            ("Добавить ОС", "add", self.add_asset, "document-new.svg"),
            ("Редактировать ОС", "edit", self.edit_asset, "document-edit.svg"),
            ("Удалить ОС", "delete", self.delete_asset, "document-delete.svg"),
            ("Фильтр", "filter", self.show_filter, "view-filter.svg"),
            ("Печать", "print", self.print_data, "document-print.svg"),
            ("Импорт в Excel", "export", self.export_to_excel, "x-office-spreadsheet.svg"),
            ("Настройки", "settings", self.show_settings, "preferences-system.svg"),
            ("Выход", "exit", self.close, "application-exit.svg")
        ]
        
        for text, obj_name, handler, icon in actions:
            action = QAction(QIcon.fromTheme(icon), text, self)
            action.setObjectName(obj_name)
            action.triggered.connect(handler)
            toolbar.addAction(action)
    
    def _refresh_table(self):
        """Обновление данных в таблице"""
        # Здесь должна быть реализация обновления QTableView
        pass
            
    def print_data(self):
        data = [asset.to_dict() for asset in self.db_manager.get_all_assets()]
        printer = AssetPrinter(data, self)
        printer.print_preview()
    
    def toggle_theme(self):
        current_theme = "dark" if self.dark_theme else "light"
        ThemeManager.load_theme(QApplication.instance(), current_theme)
        self.dark_theme = not self.dark_theme
    
    def add_asset(self):
        dialog = AssetDialog(self)
        if dialog.exec():
            try:
                self.db_manager.add_asset(dialog.get_data())
                self._refresh_table()
            except Exception as e:
                QMessageBox.critical(self, "Ошибка", f"Не удалось добавить ОС: {str(e)}")

    def edit_asset(self):
        # Реализация редактирования
        pass
        
    def delete_asset(self):
        # Реализация удаления
        pass
        
    def show_filter(self):
        # Реализация фильтрации
        pass
        
    def export_to_excel(self):
        # Реализация экспорта в Excel
        pass
        
    def show_settings(self):
        # Реализация настроек
        pass