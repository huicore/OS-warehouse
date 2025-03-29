import pytest
from core.database import DatabaseManager
from pathlib import Path

@pytest.fixture
def temp_db(tmp_path):
    db_path = tmp_path / 'test.db'
    db = DatabaseManager(db_path)
    yield db
    db.close()
    if db_path.exists():
        db_path.unlink()

def test_db_creation(temp_db):
    assert Path(temp_db.db_path).exists()
    with temp_db._get_cursor() as cursor:
        cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='assets'")
        assert cursor.fetchone() is not None