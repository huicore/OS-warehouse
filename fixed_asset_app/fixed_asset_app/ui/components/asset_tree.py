from PyQt5.QtWidgets import QTreeWidget, QTreeWidgetItem
from PyQt5.QtCore import Qt

class AssetTree(QTreeWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self._init_ui()
        self.setSortingEnabled(True)

    def _init_ui(self):
        self.setColumnCount(8)
        headers = [
            "ID", "Наименование", "Тип", "Модель", 
            "Инв. №", "Подразделение", "Статус", "Местоположение"
        ]
        self.setHeaderLabels(headers)
        
        # Настройка стилей
        self.setAlternatingRowColors(True)
        self.setStyleSheet("""
            QTreeWidget::item { padding: 5px; }
            QTreeWidget::item:hover { background: #e0e0e0; }
        """)

    def load_data(self, data):
        """Оптимизированная загрузка данных"""
        self.clear()
        items = []
        for row in data:
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
            items.append(item)
        self.addTopLevelItems(items)  # Пакетное добавление