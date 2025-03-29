from PyQt5.QtWidgets import QTreeWidget, QTreeWidgetItem
from PyQt5.QtCore import Qt

class AssetTree(QTreeWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setObjectName("assetTree")
        self._data = []
        self._init_ui()

    def _init_ui(self):
        self.setHeaderLabels(["ID", "Наименование", "Тип", "Дата поступления", 
                           "Инв. №", "Подразделение", "Статус"])
        self.setSortingEnabled(True)
        self.setAlternatingRowColors(True)

    def load_data(self, data):
        self.clear()
        self._data = data
        for item_data in data:
            self._add_item(item_data)

    def _add_item(self, item_data):
        item = QTreeWidgetItem(self)
        item.setText(0, str(item_data.get('id', len(self._data) + 1)))  # Исправлено: добавлена закрывающая скобка
        item.setText(1, item_data.get('Наименование', ''))
        item.setText(2, item_data.get('Тип', ''))
        item.setText(3, item_data.get('Дата поступления', ''))
        item.setText(4, item_data.get('Инв. №', ''))
        item.setText(5, item_data.get('Подразделение', ''))
        item.setText(6, item_data.get('Статус', ''))
        self._data.append(item_data)
        return item

    def get_visible_items(self):
        """Возвращает данные только видимых (не отфильтрованных) элементов"""
        return [
            {
                'id': self.topLevelItem(i).text(0),
                'Наименование': self.topLevelItem(i).text(1),
                'Тип': self.topLevelItem(i).text(2),
                'Дата поступления': self.topLevelItem(i).text(3),
                'Инв. №': self.topLevelItem(i).text(4),
                'Подразделение': self.topLevelItem(i).text(5),
                'Статус': self.topLevelItem(i).text(6)
            }
            for i in range(self.topLevelItemCount())
            if not self.topLevelItem(i).isHidden()
        ]