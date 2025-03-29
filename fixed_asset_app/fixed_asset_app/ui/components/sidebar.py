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

    def _connect_signals(self):
        """Подключает сигналы к слотам"""
        try:
            # Используем правильные имена кнопок из Sidebar
            self.sidebar.btn_import.clicked.connect(self._import_from_excel)
            self.sidebar.btn_print.clicked.connect(self._show_print_dialog)
            
            # Также можно использовать сигналы, если они нужны
            self.sidebar.import_clicked.connect(self._import_from_excel)
            self.sidebar.print_clicked.connect(self._show_print_dialog)
            
        except Exception as e:
            QMessageBox.critical(self, "Ошибка подключения сигналов",
                               f"Не удалось подключить сигналы: {str(e)}")
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
        """Генерирует HTML для печати с проверкой данных"""
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
                # Безопасное получение текста из колонок
                id_text = item.text(0) if item.text(0) else ""
                name_text = item.text(1) if item.text(1) else ""
                type_text = item.text(2) if item.text(2) else ""
                inv_text = item.text(4) if item.text(4) else ""
                dep_text = item.text(5) if item.text(5) else ""
                status_text = item.text(6) if item.text(6) else ""
                
                html += f"""
                <tr>
                    <td>{id_text}</td>
                    <td>{name_text}</td>
                    <td>{type_text}</td>
                    <td>{inv_text}</td>
                    <td>{dep_text}</td>
                    <td>{status_text}</td>
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