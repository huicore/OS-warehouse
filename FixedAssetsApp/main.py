import sys
from PyQt6.QtWidgets import QApplication
from ui.main_window import MainWindow
from database.db_manager import DatabaseManager

def main():
    app = QApplication(sys.argv)
    
    # Инициализация БД
    db_manager = DatabaseManager()
    if not db_manager.init_db():
        sys.exit(1)
    
    window = MainWindow(db_manager)
    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    main()