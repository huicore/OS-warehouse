import os
import pandas as pd
import openpyxl
from PyQt5.QtWidgets import (
    QMainWindow, QFileDialog, QMessageBox, 
    QWidget, QHBoxLayout, QDialog, 
    QFormLayout, QLineEdit, QDialogButtonBox,
    QInputDialog, QColorDialog
)
from PyQt5.QtPrintSupport import QPrintDialog, QPrinter, QPrintPreviewDialog
from PyQt5.QtGui import QTextDocument
from PyQt5.QtCore import QDate
from ui.components.sidebar import Sidebar
from ui.components.asset_tree import AssetTree
from config import THEMES

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Учет основных средств")
        self.setGeometry(100, 100, 1200, 800)
        self._print_html_cache = None
        self._current_theme = 'light'
        
        self._init_ui()
        self._connect_signals()
        self._change_theme(self._current_theme)

    def _init_ui(self):
        """Инициализация UI"""
        try:
            self.sidebar = Sidebar()
            self.asset_tree = AssetTree()
            
            central_widget = QWidget()
            self.setCentralWidget(central_widget)
            
            main_layout = QHBoxLayout(central_widget)
            main_layout.setContentsMargins(0, 0, 0, 0)
            main_layout.addWidget(self.sidebar)
            main_layout.addWidget(self.asset_tree)
            
        except Exception as e:
            QMessageBox.critical(None, "Ошибка инициализации", 
                               f"Не удалось создать интерфейс: {str(e)}")
            raise

    def _connect_signals(self):
        """Подключение сигналов"""
        try:
            self.sidebar.import_clicked.connect(self._import_from_excel)
            self.sidebar.export_clicked.connect(self._export_to_excel)
            self.sidebar.print_clicked.connect(self._show_print_dialog)
            self.sidebar.preview_clicked.connect(self._show_print_preview)
            self.sidebar.add_asset_clicked.connect(self._show_add_asset_dialog)
            self.sidebar.filter_clicked.connect(self._show_filter_dialog)
            self.sidebar.theme_changed.connect(self._change_theme)
            
        except Exception as e:
            QMessageBox.critical(self, "Ошибка подключения сигналов",
                               f"Не удалось подключить сигналы: {str(e)}")
            raise

    def _change_theme(self, theme_name):
        """Смена цветовой темы"""
        self._current_theme = theme_name
        theme = THEMES[theme_name]
        
        style = f"""
            QWidget {{
                background-color: {theme['background']};
                color: {theme['foreground']};
            }}
            QPushButton {{
                background-color: {theme['button_bg']};
                color: {theme['button_fg']};
                border: 1px solid #555;
                padding: 5px;
                min-height: 30px;
            }}
            QTreeWidget {{
                alternate-background-color: {theme['button_bg']};
            }}
            QHeaderView::section {{
                background-color: {theme['button_bg']};
                padding: 5px;
                border: 1px solid #555;
            }}
            QComboBox {{
                background-color: {theme['button_bg']};
                color: {theme['button_fg']};
                padding: 5px;
            }}
        """
        self.setStyleSheet(style)

    def _show_add_asset_dialog(self):
        """Диалог добавления нового ОС"""
        dialog = QDialog(self)
        dialog.setWindowTitle("Добавить новое основное средство")
        
        layout = QFormLayout(dialog)
        
        fields = {
            'Наименование': QLineEdit(),
            'Тип': QLineEdit(),
            'Инв. №': QLineEdit(),
            'Подразделение': QLineEdit(),
            'Статус': QLineEdit()
        }
        
        for label, widget in fields.items():
            layout.addRow(label, widget)
        
        buttons = QDialogButtonBox(
            QDialogButtonBox.Ok | QDialogButtonBox.Cancel,
            parent=dialog
        )
        buttons.accepted.connect(dialog.accept)
        buttons.rejected.connect(dialog.reject)
        layout.addRow(buttons)
        
        if dialog.exec_() == QDialog.Accepted:
            asset_data = {
                'Наименование': fields['Наименование'].text(),
                'Тип': fields['Тип'].text(),
                'Инв. №': fields['Инв. №'].text(),
                'Подразделение': fields['Подразделение'].text(),
                'Статус': fields['Статус'].text(),
                'Дата поступления': QDate.currentDate().toString("dd.MM.yyyy")
            }
            
            self.asset_tree._add_item(asset_data)
            self._print_html_cache = None

    def _show_filter_dialog(self):
        """Диалог фильтрации"""
        text, ok = QInputDialog.getText(
            self, 'Фильтр', 'Введите текст для поиска:'
        )
        if ok:
            self._apply_filter(text if text else "")

    def _apply_filter(self, filter_text):
        """Применение фильтра"""
        for i in range(self.asset_tree.topLevelItemCount()):
            item = self.asset_tree.topLevelItem(i)
            match = any(
                filter_text.lower() in item.text(col).lower()
                for col in range(self.asset_tree.columnCount())
            )
            item.setHidden(not match)

    def _import_from_excel(self):
        """Импорт данных из Excel"""
        try:
            file_path, _ = QFileDialog.getOpenFileName(
                self, "Выберите файл Excel", "", 
                "Excel Files (*.xlsx *.xls);;All Files (*)"
            )
            
            if not file_path:
                return
                
            if not os.path.exists(file_path):
                raise FileNotFoundError("Файл не найден")
                
            if not file_path.lower().endswith(('.xlsx', '.xls')):
                raise ValueError("Поддерживаются только файлы .xlsx и .xls")
            
            df = pd.read_excel(file_path)
            required_columns = {'Наименование', 'Тип', 'Инв. №'}
            if not required_columns.issubset(df.columns):
                missing = required_columns - set(df.columns)
                raise ValueError(f"Отсутствуют колонки: {', '.join(missing)}")
            
            self._print_html_cache = None
            self.asset_tree.load_data(df.to_dict('records'))
            
        except Exception as e:
            QMessageBox.critical(self, "Ошибка импорта", 
                               f"Ошибка: {str(e)}\n\nПодробности в логах")
            print(f"Import error: {str(e)}")

    def _export_to_excel(self):
        """Экспорт данных в Excel"""
        try:
            file_path, _ = QFileDialog.getSaveFileName(
                self, "Экспорт в Excel", "",
                "Excel Files (*.xlsx);;All Files (*)"
            )
            
            if not file_path:
                return
                
            visible_data = self.asset_tree.get_visible_items()
            
            if not visible_data:
                QMessageBox.warning(self, "Нет данных", "Нет данных для экспорта!")
                return
                
            df = pd.DataFrame(visible_data)
            
            # Сохраняем в Excel
            df.to_excel(file_path, index=False)
            QMessageBox.information(self, "Экспорт", "Данные успешно экспортированы!")
            
        except Exception as e:
            QMessageBox.critical(self, "Ошибка экспорта", f"Ошибка: {str(e)}")

    def _show_print_dialog(self):
        """Диалог печати"""
        try:
            printer = QPrinter(QPrinter.HighResolution)
            print_dialog = QPrintDialog(printer, self)
            
            if print_dialog.exec_() == QPrintDialog.Accepted:
                doc = QTextDocument()
                doc.setHtml(self._generate_print_html())
                doc.print_(printer)
                
        except Exception as e:
            QMessageBox.critical(self, "Ошибка печати", 
                               f"Не удалось выполнить печать:\n{str(e)}")

    def _generate_print_html(self):
        """Генерация HTML для печати"""
        try:
            if not hasattr(self, 'asset_tree') or self.asset_tree.topLevelItemCount() == 0:
                return "<html><body><h1>Нет данных для печати</h1></body></html>"

            html = """
            <html>
            <head>
            <style>
                body { font-family: Arial; }
                h1 { color: #2b579a; }
                table { border-collapse: collapse; width: 100%; margin-top: 20px; }
                th { background-color: #f0f0f0; padding: 8px; text-align: left; }
                td { padding: 6px; border-bottom: 1px solid #ddd; }
            </style>
            </head>
            <body>
            <h1>Реестр основных средств</h1>
            <table>
                <tr>
                    <th>ID</th>
                    <th>Наименование</th>
                    <th>Тип</th>
                    <th>Инв. №</th>
                    <th>Подразделение</th>
                    <th>Статус</th>
                </tr>
            """
            
            for i in range(self.asset_tree.topLevelItemCount()):
                item = self.asset_tree.topLevelItem(i)
                if not item.isHidden():  # Печатаем только видимые элементы
                    html += f"""
                    <tr>
                        <td>{item.text(0)}</td>
                        <td>{item.text(1)}</td>
                        <td>{item.text(2)}</td>
                        <td>{item.text(4)}</td>
                        <td>{item.text(5)}</td>
                        <td>{item.text(6)}</td>
                    </tr>
                    """
            
            html += f"""
            </table>
            <p style="margin-top: 30px; font-size: 0.8em;">
                Дата печати: {QDate.currentDate().toString("dd.MM.yyyy")}
            </p>
            </body>
            </html>
            """
            
            return html
            
        except Exception as e:
            error_html = f"""
            <html>
            <body>
            <h1>Ошибка генерации отчета</h1>
            <p>{str(e)}</p>
            </body>
            </html>
            """
            return error_html

    def _show_print_preview(self):
        """Предпросмотр печати"""
        try:
            printer = QPrinter(QPrinter.HighResolution)
            preview = QPrintPreviewDialog(printer, self)
            preview.paintRequested.connect(self._print_preview_content)
            preview.exec_()
        except Exception as e:
            QMessageBox.critical(self, "Ошибка", 
                               f"Ошибка предпросмотра:\n{str(e)}")

    def _print_preview_content(self, printer):
        """Содержимое для предпросмотра"""
        doc = QTextDocument()
        doc.setHtml(self._generate_print_html())
        doc.print_(printer)