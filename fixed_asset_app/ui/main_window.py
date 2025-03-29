import pandas as pd
from openpyxl import load_workbook
from PyQt5.QtWidgets import (QMainWindow, QFileDialog, QMessageBox, 
                            QTreeWidgetItem, QWidget, QHBoxLayout)
from PyQt5.QtPrintSupport import QPrintDialog, QPrinter, QPrintPreviewDialog
from PyQt5.QtGui import QTextDocument  # QTextDocument теперь из QtGui
from PyQt5.QtCore import QDate
from ui.components.sidebar import Sidebar
from ui.components.asset_tree import AssetTree

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Учет основных средств")
        self.setGeometry(100, 100, 1200, 800)
        
        # Инициализируем компоненты интерфейса
        self._init_ui()
        
        # Подключаем сигналы после создания всех компонентов
        self._connect_signals()

    def _init_ui(self):
        """Инициализация пользовательского интерфейса"""
        # Создаем боковую панель
        self.sidebar = Sidebar()
        
        # Создаем таблицу активов
        self.asset_tree = AssetTree()
        
        # Настройка главного окна
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        
        main_layout = QHBoxLayout(central_widget)
        main_layout.setContentsMargins(0, 0, 0, 0)
        main_layout.setSpacing(0)
        
        main_layout.addWidget(self.sidebar)
        main_layout.addWidget(self.asset_tree)

    def _connect_signals(self):
        """Подключение сигналов к слотам"""
        self.sidebar.import_excel_clicked.connect(self._import_from_excel)
        self.sidebar.print_clicked.connect(self._show_print_dialog)

    def _import_from_excel(self):
        """Импорт данных из Excel"""
        try:
            file_path, _ = QFileDialog.getOpenFileName(
                self, "Выберите файл Excel", "", 
                "Excel Files (*.xlsx *.xls)"
            )
            
            if file_path:
                df = pd.read_excel(file_path)
                self.asset_tree.clear()
                
                for _, row in df.iterrows():
                    item = QTreeWidgetItem([
                        str(row.get('ID', '')),
                        str(row.get('Наименование', '')),
                        str(row.get('Тип', '')),
                        str(row.get('Модель', '')),
                        str(row.get('Инв. №', '')),
                        str(row.get('Подразделение', '')),
                        str(row.get('Статус', 'Активен')),
                        str(row.get('Местоположение', ''))
                    ])
                    self.asset_tree.addTopLevelItem(item)
                
                QMessageBox.information(self, "Успех", 
                    f"Импортировано {len(df)} записей из Excel")
                
        except Exception as e:
            QMessageBox.critical(self, "Ошибка", 
                f"Не удалось импортировать данные: {str(e)}")

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