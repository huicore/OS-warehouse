from PyQt5.QtWidgets import QDialog, QVBoxLayout, QFormLayout, QLineEdit, QComboBox, QDateEdit
from PyQt5.QtCore import QDate

class AssetDialog(QDialog):
    def __init__(self, parent=None, asset_data=None):
        super().__init__(parent)
        self.setWindowTitle("Редактирование актива" if asset_data else "Добавление актива")
        self.setMinimumSize(500, 400)
        
        self.layout = QVBoxLayout(self)
        self.form_layout = QFormLayout()
        
        self.fields = {
            'name': QLineEdit(),
            'type': QComboBox(),
            'inv_num': QLineEdit(),
            'date': QDateEdit(QDate.currentDate()),
            # ... остальные поля
        }
        
        self._setup_ui()
        if asset_data:
            self._fill_form(asset_data)
            
    def _setup_ui(self):
        """Настройка интерфейса диалога"""
        for field_name, widget in self.fields.items():
            self.form_layout.addRow(field_name.capitalize(), widget)
        
        self.layout.addLayout(self.form_layout)
        self._setup_buttons()
        
    def get_data(self) -> dict:
        """Возвращает данные из формы"""
        return {
            'name': self.fields['name'].text(),
            'type': self.fields['type'].currentText(),
            # ... остальные поля
        }