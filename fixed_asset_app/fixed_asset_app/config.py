from pathlib import Path
from typing import Dict, Any

class Config:
    def __init__(self):
        self.app_name = "Fixed Asset Manager"
        self.version = "1.0.0"
        self.db_path = Path('data') / 'assets.db'
        self.photo_storage = Path('data') / 'photos'
        self.default_theme = 'light'
        
        # Создаем необходимые директории
        self.db_path.parent.mkdir(exist_ok=True)
        self.photo_storage.mkdir(exist_ok=True)
    
    @property
    def themes(self) -> Dict[str, str]:
        return {
            'light': str(Path('assets') / 'styles' / 'light.qss'),
            'dark': str(Path('assets') / 'styles' / 'dark.qss')
        }