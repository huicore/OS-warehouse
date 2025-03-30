from PyQt6.QtPrintSupport import QPrinter, QPrintPreviewDialog
from PyQt6.QtWidgets import QTextEdit, QApplication
from PyQt6.QtGui import QTextDocument, QTextCursor
from PyQt6.QtCore import QMarginsF

class AssetPrinter:
    def __init__(self, data, parent=None):
        self.data = data  # Список словарей с данными ОС
        self.parent = parent

    def generate_html(self):
        """Генерация HTML для печати"""
        html = """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <style>
                body { font-family: Arial; margin: 0; padding: 20px; }
                h1 { text-align: center; }
                table { width: 100%; border-collapse: collapse; }
                th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                th { background-color: #f2f2f2; }
            </style>
        </head>
        <body>
        <h1>Реестр основных средств</h1>
        <table>
            <thead>
                <tr>
                    <th>Инв. №</th>
                    <th>Тип ОС</th>
                    <th>Наименование</th>
                    <th>Статус</th>
                    <th>МОЛ</th>
                </tr>
            </thead>
            <tbody>
        """

        for item in self.data:
            html += f"""
            <tr>
                <td>{item.get('inventory_number', '')}</td>
                <td>{item.get('asset_type', '')}</td>
                <td>{item.get('name', '')}</td>
                <td>{item.get('status', '')}</td>
                <td>{item.get('responsible', '')}</td>
            </tr>
            """
        
        html += """
            </tbody>
        </table>
        </body>
        </html>
        """
        return html

    def print_preview(self):
        """Предпросмотр печати"""
        printer = QPrinter(QPrinter.PrinterMode.HighResolution)
        printer.setPageMargins(QMarginsF(15, 15, 15, 15), QPrinter.Unit.Millimeter)
        
        preview = QPrintPreviewDialog(printer, self.parent)
        preview.paintRequested.connect(self.print_document)
        preview.exec()

    def print_document(self, printer):
        """Рендер документа для печати"""
        document = QTextDocument()
        document.setHtml(self.generate_html())
        document.print_(printer)