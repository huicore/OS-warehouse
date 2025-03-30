from PyQt6.QtWidgets import QDialog, QFormLayout, QComboBox, QLineEdit, QDialogButtonBox

class AssetDialog(QDialog):
    def __init__(self, parent=None, asset=None):
        super().__init__(parent)
        self.setWindowTitle("Карточка ОС" if asset else "Добавление ОС")
        
        layout = QFormLayout()
        
        # Поля формы
        self.inventory_number = QLineEdit()
        self.asset_type = QComboBox()
        self.asset_type.addItems(['Принтер', 'Системный блок', 'Ноутбук', 'Монитор', 'ИБП'])
        
        # Добавление всех полей...
        
        buttons = QDialogButtonBox(
            QDialogButtonBox.StandardButton.Ok | QDialogButtonBox.StandardButton.Cancel
        )
        buttons.accepted.connect(self.accept)
        buttons.rejected.connect(self.reject)
        
        layout.addRow(buttons)
        self.setLayout(layout)