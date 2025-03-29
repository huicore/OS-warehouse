from PyQt5.QtWidgets import QFrame, QVBoxLayout, QPushButton, QComboBox
from PyQt5.QtCore import pyqtSignal
from config import BUTTONS, THEMES

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
        self.setObjectName("sidebar")
        self.setMinimumWidth(200)
        self._init_ui()

    def _init_ui(self):
        """Инициализация пользовательского интерфейса"""
        layout = QVBoxLayout(self)
        layout.setContentsMargins(5, 5, 5, 5)
        layout.setSpacing(10)

        # Выбор темы
        self.theme_combo = QComboBox()
        self.theme_combo.addItems(['Светлая тема', 'Темная тема'])
        self.theme_combo.currentTextChanged.connect(self._on_theme_changed)
        layout.addWidget(self.theme_combo)

        # Кнопки действий
        self._create_action_buttons(layout)

        # Растягивающий элемент для выравнивания кнопок вверху
        layout.addStretch()

    def _create_action_buttons(self, layout):
        """Создает кнопки действий"""
        buttons_config = [
            ('import', self.import_clicked),
            ('export', self.export_clicked),
            ('add', self.add_asset_clicked),
            ('filter', self.filter_clicked),
            ('print', self.print_clicked),
            ('preview', self.preview_clicked)
        ]

        for btn_type, signal in buttons_config:
            btn = self._create_button(
                text=BUTTONS[btn_type]['text'],
                icon=BUTTONS[btn_type]['icon'],
                action=signal.emit
            )
            layout.addWidget(btn)

    def _create_button(self, text, icon, action):
        """Создает кнопку с заданными параметрами"""
        btn = QPushButton(f"{icon} {text}")
        btn.setObjectName(f"btn_{text.replace(' ', '_').lower()}")
        btn.clicked.connect(action)
        btn.setMinimumHeight(40)
        btn.setStyleSheet("""
            QPushButton {
                text-align: left;
                padding-left: 15px;
            }
        """)
        return btn

    def _on_theme_changed(self, theme_text):
        """Обработчик изменения темы"""
        theme = 'dark' if theme_text == 'Темная тема' else 'light'
        self.theme_changed.emit(theme)

    def set_theme_style(self, theme):
        """Устанавливает стиль для текущей темы"""
        colors = THEMES[theme]
        self.setStyleSheet(f"""
            QFrame#sidebar {{
                background-color: {colors['background']};
                border-right: 1px solid #555;
            }}
            QComboBox, QPushButton {{
                background-color: {colors['button_bg']};
                color: {colors['button_fg']};
                border: 1px solid #555;
                border-radius: 4px;
            }}
            QPushButton:hover {{
                background-color: {colors['button_hover']};
            }}
        """)