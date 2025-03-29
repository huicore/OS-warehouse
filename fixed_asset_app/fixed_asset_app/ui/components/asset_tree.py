from PyQt5.QtWidgets import QTreeWidget, QTreeWidgetItem
from PyQt5.QtCore import Qt

class AssetTree(QTreeWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setObjectName("assetTree")
        self._init_ui()

    def _init_ui(self):
        self.setHeaderLabels(["ID", "Наименование", "Тип", "Дата поступления", 
                           "Инв. №", "Подразделение", "Статус"])
        self.setSortingEnabled(True)
        self.setAlternatingRowColors(True)

    def load_data(self, data):
        self.clear()
        for item_data in data:
            item = QTreeWidgetItem(self)
            item.setText(0, str(item_data.get('id', '')))
            item.setText(1, item_data.get('Наименование', ''))
            item.setText(2, item_data.get('Тип', ''))
            item.setText(3, item_data.get('Дата поступления', ''))
            item.setText(4, item_data.get('Инв. №', ''))
            item.setText(5, item_data.get('Подразделение', ''))
            item.setText(6, item_data.get('Статус', ''))