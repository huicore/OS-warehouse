import os
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.exc import SQLAlchemyError
from .models import Base, FixedAsset
from PyQt6.QtWidgets import QFileDialog, QMessageBox

class DatabaseManager:
    def __init__(self):
        self.engine = None
        self.Session = None
        self.db_path = None

    def init_db(self, parent_window=None):
        """Инициализация БД с диалогом выбора"""
        if not self._check_existing_db():
            return self._show_first_run_dialog(parent_window)
        return True

    def _check_existing_db(self):
        """Проверка существующей БД"""
        default_path = os.path.expanduser('~/fixed_assets.db')
        if os.path.exists(default_path):
            self.db_path = default_path
            self._create_engine()
            return True
        return False

    def _create_engine(self):
        """Создание движка SQLAlchemy"""
        self.engine = create_engine(f'sqlite:///{self.db_path}')
        Base.metadata.create_all(self.engine)
        self.Session = sessionmaker(bind=self.engine)

    def _show_first_run_dialog(self, parent):
        """Диалог первого запуска"""
        msg = QMessageBox(parent)
        msg.setWindowTitle("Работа с базой данных")
        msg.setText("База данных не найдена. Создать новую или подключить существующую?")
        msg.addButton("Создать", QMessageBox.ButtonRole.AcceptRole)
        msg.addButton("Подключить", QMessageBox.ButtonRole.ActionRole)
        msg.addButton(QMessageBox.StandardButton.Cancel)
        
        result = msg.exec()
        
        if result == 0:  # Создать
            path = QFileDialog.getExistingDirectory(parent, "Выберите папку для базы данных")
            if path:
                self.db_path = os.path.join(path, 'fixed_assets.db')
                self._create_engine()
                return True
        elif result == 1:  # Подключить
            path, _ = QFileDialog.getOpenFileName(parent, "Выбете файл базы данных", "", "SQLite DB (*.db *.sqlite)")
            if path:
                self.db_path = path
                self._create_engine()
                return True
        return False

    def get_all_assets(self):
        """Получение всех ОС"""
        session = self.Session()
        try:
            return session.query(FixedAsset).all()
        finally:
            session.close()

    def add_asset(self, asset_data):
        """Добавление новой ОС"""
        session = self.Session()
        try:
            asset = FixedAsset(**asset_data)
            session.add(asset)
            session.commit()
            return True
        except SQLAlchemyError as e:
            session.rollback()
            raise e
        finally:
            session.close()