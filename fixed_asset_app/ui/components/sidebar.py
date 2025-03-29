from PyQt5.QtWidgets import QFrame, QVBoxLayout, QPushButton
from PyQt5.QtCore import pyqtSignal, Qt

class Sidebar(QFrame):
    theme_changed = pyqtSignal(str)
    import_excel_clicked = pyqtSignal()
    print_clicked = pyqtSignal()
    
    def __init__(self, parent=None):
        super().__init__(parent)
        self._init_ui()
    
    def _init_ui(self):
        self.setFixedWidth(250)
        self.setObjectName("sidebar")
        
        layout = QVBoxLayout(self)
        layout.setContentsMargins(10, 10, 10, 10)
        layout.setSpacing(10)
        
        # Кнопка импорта из Excel
        self.btn_import = QPushButton("📊 Импорт из Excel")
        self.btn_import.clicked.connect(self.import_excel_clicked.emit)
        
        # Кнопка печати
        self.btn_print = QPushButton("🖨️ Печать")
        self.btn_print.clicked.connect(self.print_clicked.emit)
        
        # Кнопка темы
        self.btn_theme = QPushButton("🌙 Темная тема")
        self.btn_theme.setCheckable(True)
        self.btn_theme.toggled.connect(self._toggle_theme)
        
        layout.addWidget(self.btn_import)
        layout.addWidget(self.btn_print)
        layout.addStretch()
        layout.addWidget(self.btn_theme)
    
    def _toggle_theme(self, checked):
        theme = 'dark' if checked else 'light'
        self.btn_theme.setText("🌞 Светлая тема" if checked else "🌙 Темная тема")
        self.theme_changed.emit(theme)