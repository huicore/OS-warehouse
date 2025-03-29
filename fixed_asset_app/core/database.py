import sqlite3
from contextlib import contextmanager
from pathlib import Path
from typing import Optional, List, Dict, Any

class DatabaseManager:
    def __init__(self, db_path: str = 'assets.db'):
        self.db_path = Path(db_path)
        self._ensure_db_directory()
        self._init_db()
    
    @contextmanager
    def _get_cursor(self):
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        try:
            yield cursor
            conn.commit()
        except Exception as e:
            conn.rollback()
            raise e
        finally:
            conn.close()
    
    def _ensure_db_directory(self):
        self.db_path.parent.mkdir(exist_ok=True)
    
    def _init_db(self):
        with self._get_cursor() as cursor:
            cursor.execute("""
                CREATE TABLE IF NOT EXISTS assets (
                    id INTEGER PRIMARY KEY,
                    name TEXT NOT NULL,
                    inv_num TEXT UNIQUE,
                    date TEXT,
                    department TEXT,
                    responsible TEXT,
                    status TEXT,
                    location TEXT,
                    type TEXT,
                    model TEXT,
                    serial TEXT,
                    photo TEXT,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )
            """)
            # Добавляем триггер для обновления updated_at
            cursor.execute("""
                CREATE TRIGGER IF NOT EXISTS update_assets_timestamp
                AFTER UPDATE ON assets
                BEGIN
                    UPDATE assets SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END;
            """)

    def get_asset(self, asset_id: int) -> Optional[Dict[str, Any]]:
        """Получение одного актива по ID"""
        with self._get_cursor() as cursor:
            cursor.execute("SELECT * FROM assets WHERE id=?", (asset_id,))
            row = cursor.fetchone()
            if row:
                columns = [col[0] for col in cursor.description]
                return dict(zip(columns, row))
            return None