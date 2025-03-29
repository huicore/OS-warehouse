from PyQt5.QtWidgets import QDialog, QVBoxLayout, QFormLayout, QLineEdit, QComboBox, QDialogButtonBox, QDateEdit
from PyQt5.QtCore import QDate, Qt  # Добавляем импорт Qt

class AddAssetDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Добавить основное средство")
        self.setMinimumSize(400, 300)
        self._init_ui()
    
    def _init_ui(self):
        layout = QVBoxLayout(self)
        form = QFormLayout()
        
        self.name_input = QLineEdit()
        self.type_input = QComboBox()
        self.inv_num_input = QLineEdit()
        self.date_input = QDateEdit(QDate.currentDate())
        self.date_input.setCalendarPopup(True)
        
        form.addRow("Наименование*:", self.name_input)
        form.addRow("Тип ОС*:", self.type_input)
        form.addRow("Инв. номер*:", self.inv_num_input)
        form.addRow("Дата*:", self.date_input)
        
        buttons = QDialogButtonBox(
            QDialogButtonBox.Ok | QDialogButtonBox.Cancel,
            Qt.Horizontal, self  # Теперь Qt определен
        )
        buttons.accepted.connect(self._validate_and_accept)
        buttons.rejected.connect(self.reject)
        
        layout.addLayout(form)
        layout.addWidget(buttons)
    
    def _validate_and_accept(self):
        if not self.name_input.text().strip():
            from PyQt5.QtWidgets import QMessageBox
            QMessageBox.warning(self, "Ошибка", "Введите наименование!")
            return
        if not self.inv_num_input.text().strip():
            from PyQt5.QtWidgets import QMessageBox
            QMessageBox.warning(self, "Ошибка", "Введите инвентарный номер!")
            return
        self.accept()
    
    def set_types(self, types):
        self.type_input.clear()
        self.type_input.addItems(types)
    
    def get_data(self):
        return {
            'name': self.name_input.text().strip(),
            'type': self.type_input.currentText(),
            'inv_num': self.inv_num_input.text().strip(),
            'date': self.date_input.date().toString("yyyy-MM-dd")
        }