from PyQt5.QtWidgets import QFrame, QVBoxLayout, QHBoxLayout, QPushButton
from PyQt5.QtCore import pyqtSignal, Qt
from config import BUTTONS  # Файл с настройками

class Sidebar(QFrame):
    theme_changed = pyqtSignal(str)
    import_clicked = pyqtSignal()
    print_clicked = pyqtSignal()
    
    def __init__(self, parent=None):
        super().__init__(parent)
        self.setObjectName("sidebar")
        self._init_ui()

    def _init_ui(self):
        layout = QVBoxLayout(self)
        layout.setContentsMargins(10, 10, 10, 10)
        
        # Кнопка импорта
        self.btn_import = self._create_button(
            text=BUTTONS['import']['text'],
            icon=BUTTONS['import']['icon'],
            action=self.import_clicked.emit
        )
        
        # Кнопка печати
        self.btn_print = self._create_button(
            text=BUTTONS['print']['text'],
            icon=BUTTONS['print']['icon'],
            action=self.print_clicked.emit
        )
        
        layout.addWidget(self.btn_import)
        layout.addWidget(self.btn_print)
        layout.addStretch()

    def _create_button(self, text, icon, action):
        """Фабричный метод для создания кнопок"""
        btn = QPushButton(f"{icon} {text}")
        btn.clicked.connect(action)
        btn.setMinimumHeight(40)
        return btn
