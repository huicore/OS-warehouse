from PyQt5.QtWidgets import QTreeWidget, QTreeWidgetItem

class AssetTree(QTreeWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self._init_ui()
    
    def _init_ui(self):
        self.setColumnCount(8)
        self.setHeaderLabels([
            "ID", "Наименование", "Тип", "Модель", "Инв. №", 
            "Подразделение", "Статус", "Местоположение"
        ])
        self.setColumnWidth(0, 60)
        self.setColumnWidth(1, 250)
        self.setColumnWidth(2, 150)
        self.setColumnWidth(3, 180)
        self.setColumnWidth(4, 120)
        self.setColumnWidth(5, 200)
        self.setColumnWidth(6, 120)
        self.setColumnWidth(7, 150)
        
        self.setStyleSheet("""
            QTreeWidget {
                background-color: white;
                alternate-background-color: #f5f5f5;
            }
            QTreeWidget::item {
                height: 35px;
            }
        """)
        self.setAlternatingRowColors(True)