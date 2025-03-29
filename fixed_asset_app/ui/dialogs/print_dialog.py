from PyQt5.QtWidgets import (QDialog, QVBoxLayout, QHBoxLayout, QGroupBox, 
                            QRadioButton, QCheckBox, QSpinBox, QLabel,
                            QDialogButtonBox, QComboBox)
from PyQt5.QtCore import Qt

class PrintDialog(QDialog):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setWindowTitle("Настройки печати")
        self.setMinimumSize(400, 300)
        self._init_ui()
    
    def _init_ui(self):
        layout = QVBoxLayout(self)
        
        # Настройки страницы
        page_group = QGroupBox("Настройки страницы")
        page_layout = QVBoxLayout()
        
        self.orientation_group = QHBoxLayout()
        self.portrait_rb = QRadioButton("Книжная")
        self.landscape_rb = QRadioButton("Альбомная")
        self.portrait_rb.setChecked(True)
        self.orientation_group.addWidget(self.portrait_rb)
        self.orientation_group.addWidget(self.landscape_rb)
        
        self.margins_group = QHBoxLayout()
        self.margins_group.addWidget(QLabel("Поля:"))
        self.margin_spin = QSpinBox()
        self.margin_spin.setRange(5, 50)
        self.margin_spin.setValue(20)
        self.margin_spin.setSuffix(" мм")
        self.margins_group.addWidget(self.margin_spin)
        
        page_layout.addLayout(self.orientation_group)
        page_layout.addLayout(self.margins_group)
        page_group.setLayout(page_layout)
        
        # Настройки содержимого
        content_group = QGroupBox("Настройки содержимого")
        content_layout = QVBoxLayout()
        
        self.grid_cb = QCheckBox("Печатать сетку")
        self.grid_cb.setChecked(True)
        
        self.headers_cb = QCheckBox("Печатать заголовки")
        self.headers_cb.setChecked(True)
        
        self.scale_combo = QComboBox()
        self.scale_combo.addItems(["100%", "Вписать на страницу", "75%", "50%"])
        
        content_layout.addWidget(self.grid_cb)
        content_layout.addWidget(self.headers_cb)
        content_layout.addWidget(QLabel("Масштаб:"))
        content_layout.addWidget(self.scale_combo)
        content_group.setLayout(content_layout)
        
        # Кнопки
        buttons = QDialogButtonBox(
            QDialogButtonBox.Ok | QDialogButtonBox.Cancel,
            Qt.Horizontal, self
        )
        buttons.accepted.connect(self.accept)
        buttons.rejected.connect(self.reject)
        
        layout.addWidget(page_group)
        layout.addWidget(content_group)
        layout.addWidget(buttons)
    
    def get_settings(self):
        return {
            'orientation': 'portrait' if self.portrait_rb.isChecked() else 'landscape',
            'margins': self.margin_spin.value(),
            'print_grid': self.grid_cb.isChecked(),
            'print_headers': self.headers_cb.isChecked(),
            'scale': self.scale_combo.currentText()
        }