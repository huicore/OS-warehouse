from PyQt5.QtWidgets import QFrame, QVBoxLayout, QPushButton, QComboBox
from PyQt5.QtCore import pyqtSignal

class Sidebar(QFrame):
    theme_changed = pyqtSignal(str)
    import_clicked = pyqtSignal()
    export_clicked = pyqtSignal() 
    print_clicked = pyqtSignal()
    preview_clicked = pyqtSignal()
    add_asset_clicked = pyqtSignal()
    filter_clicked = pyqtSignal()
    
    def __init__(self, parent=None):
        super().__init__(parent)
        self._init_ui()

    def _init_ui(self):
        layout = QVBoxLayout(self)
        
        # Кнопка смены темы
        self.theme_combo = QComboBox()
        self.theme_combo.addItems(['Светлая', 'Темная'])
        self.theme_combo.currentTextChanged.connect(
            lambda: self.theme_changed.emit('dark' if self.theme_combo.currentText() == 'Темная' else 'light')
        )
        layout.addWidget(self.theme_combo)

        # Кнопки действий
        buttons = [
            ('import', self.import_clicked),
            ('export', self.export_clicked),
            ('add', self.add_asset_clicked),
            ('filter', self.filter_clicked),
            ('print', self.print_clicked),
            ('preview', self.preview_clicked)
        ]
        
        for btn_type, signal in buttons:
            btn = QPushButton(f"{BUTTONS[btn_type]['icon']} {BUTTONS[btn_type]['text']}")
            btn.clicked.connect(signal.emit)
            layout.addWidget(btn)
        
        layout.addStretch()