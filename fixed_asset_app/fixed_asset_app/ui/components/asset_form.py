from PyQt5.QtWidgets import (
    QWidget, QFormLayout, QLineEdit, QComboBox, 
    QDateEdit, QGroupBox, QVBoxLayout
)
from PyQt5.QtCore import QDate

class AssetForm(QWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self._init_ui()
    
    def _init_ui(self):
        layout = QVBoxLayout(self)
        group = QGroupBox("Информация об ОС")
        form_layout = QFormLayout()
        
        self.fields = {
            'name': QLineEdit(),
            'type': QComboBox(),
            'model': QLineEdit(),
            'serial': QLineEdit(),
            'inv_num': QLineEdit(),
            'date': QDateEdit(QDate.currentDate()),
            'department': QComboBox(),
            'responsible': QLineEdit(),
            'status': QComboBox(),
            'location': QComboBox()
        }
        
        # Настройка полей
        self.fields['date'].setCalendarPopup(True)
        
        # Добавление полей в форму
        for label, field in [
            ("Наименование*", 'name'),
            ("Тип ОС*", 'type'),
            ("Модель", 'model'),
            ("Серийный номер", 'serial'),
            ("Инв. номер*", 'inv_num'),
            ("Дата*", 'date'),
            ("Подразделение*", 'department'),
            ("МОЛ*", 'responsible'),
            ("Статус*", 'status'),
            ("Местоположение*", 'location')
        ]:
            form_layout.addRow(label, self.fields[field])
        
        group.setLayout(form_layout)
        layout.addWidget(group)
    
    def set_reference_data(self, ref_data: dict):
        """Установка справочных данных"""
        for field, items in ref_data.items():
            if field in self.fields:
                self.fields[field].clear()
                self.fields[field].addItems(items)
    
    def get_data(self) -> dict:
        """Получение данных из формы"""
        return {
            'name': self.fields['name'].text().strip(),
            'type': self.fields['type'].currentText(),
            'model': self.fields['model'].text().strip(),
            'serial': self.fields['serial'].text().strip(),
            'inv_num': self.fields['inv_num'].text().strip(),
            'date': self.fields['date'].date().toString("yyyy-MM-dd"),
            'department': self.fields['department'].currentText(),
            'responsible': self.fields['responsible'].text().strip(),
            'status': self.fields['status'].currentText(),
            'location': self.fields['location'].currentText()
        }
    
    def set_data(self, data: dict):
        """Заполнение формы данными"""
        for field, value in data.items():
            if field in self.fields:
                if isinstance(self.fields[field], QComboBox):
                    index = self.fields[field].findText(str(value))
                    if index >= 0:
                        self.fields[field].setCurrentIndex(index)
                elif isinstance(self.fields[field], QDateEdit) and value:
                    date = QDate.fromString(value, "yyyy-MM-dd")
                    self.fields[field].setDate(date)
                else:
                    self.fields[field].setText(str(value) if value else "")