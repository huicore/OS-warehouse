import os
import pandas as pd
from PyQt5.QtWidgets import (QMainWindow, QFileDialog, QMessageBox, 
                           QTreeWidgetItem, QWidget, QHBoxLayout)
from PyQt5.QtPrintSupport import QPrintDialog, QPrinter, QPrintPreviewDialog
from PyQt5.QtGui import QTextDocument
from PyQt5.QtCore import QDate
from ui.components.sidebar import Sidebar
from ui.components.asset_tree import AssetTree

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Учет основных средств")
        self.setGeometry(100, 100, 1200, 800)
        self._print_html_cache = None  # Кэш для HTML
        
        self._init_ui()
        self._connect_signals()

    def _init_ui(self):
        """Инициализация UI с проверкой создания виджетов"""
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

    def _import_from_excel(self):
        """Безопасный импорт с проверкой формата"""
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
            
            # Чтение с проверкой колонок
            df = pd.read_excel(file_path)
            required_columns = {'Наименование', 'Тип', 'Инв. №'}
            if not required_columns.issubset(df.columns):
                missing = required_columns - set(df.columns)
                raise ValueError(f"Отсутствуют колонки: {', '.join(missing)}")
            
            # Кэширование данных
            self._print_html_cache = None
            self.asset_tree.load_data(df.to_dict('records'))
            
        except Exception as e:
            QMessageBox.critical(self, "Ошибка импорта", 
                               f"Ошибка: {str(e)}\n\nПодробности в логах")
            print(f"Import error: {str(e)}")

    def _show_print_dialog(self):
        """Показывает диалог печати"""
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
        """Генерирует HTML для печати"""
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
        
        html += """
        </table>
        <p style="margin-top: 30px; font-size: 0.8em;">
            Дата печати: {date}
        </p>
        </body>
        </html>
        """.format(date=QDate.currentDate().toString("dd.MM.yyyy"))
        
        return html

    def _show_print_preview(self):
        """Показывает предпросмотр перед печатью"""
        try:
            printer = QPrinter(QPrinter.HighResolution)
            preview = QPrintPreviewDialog(printer, self)
            preview.paintRequested.connect(self._print_preview_content)
            preview.exec_()
        except Exception as e:
            QMessageBox.critical(self, "Ошибка", 
                               f"Ошибка предпросмотра:\n{str(e)}")

    def _print_preview_content(self, printer):
        """Генерирует содержимое для предпросмотра"""
        doc = QTextDocument()
        doc.setHtml(self._generate_print_html())
        doc.print_(printer)
