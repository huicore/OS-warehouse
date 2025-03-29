from fpdf import FPDF
from datetime import datetime
from pathlib import Path
from PIL import Image
import tempfile

class PDFGenerator:
    def __init__(self, theme: str = 'light'):
        self.theme = theme
        self.font_sizes = {
            'title': 16,
            'header': 12,
            'body': 10
        }
        self.colors = {
            'light': {
                'text': (0, 0, 0),
                'background': (255, 255, 255),
                'accent': (43, 87, 154)
            },
            'dark': {
                'text': (255, 255, 255),
                'background': (40, 40, 40),
                'accent': (66, 135, 245)
            }
        }

    def generate_asset_card(self, asset_data: dict, output_path: str):
        pdf = FPDF()
        pdf.add_page()
        theme_colors = self.colors[self.theme]
        
        # Установка темы
        pdf.set_fill_color(*theme_colors['background'])
        pdf.set_text_color(*theme_colors['text'])
        pdf.rect(0, 0, 210, 297, 'F')  # Фон страницы
        
        # Заголовок
        pdf.set_font("Arial", 'B', self.font_sizes['title'])
        pdf.set_text_color(*theme_colors['accent'])
        pdf.cell(0, 10, f"Карточка ОС: {asset_data['name']", 0, 1, 'C')
        pdf.ln(10)
        
        # Добавление фото (если есть)
        if asset_data.get('photo') and Path(asset_data['photo']).exists():
            self._add_image_to_pdf(pdf, asset_data['photo'], y=30)
        
        # Генерация данных
        self._add_asset_details(pdf, asset_data, theme_colors)
        
        pdf.output(output_path)

    def _add_image_to_pdf(self, pdf: FPDF, image_path: str, y: int):
        """Оптимизированное добавление изображения"""
        try:
            with tempfile.NamedTemporaryFile(suffix='.jpg') as temp_img:
                img = Image.open(image_path)
                img.thumbnail((150, 150))
                img.save(temp_img.name, 'JPEG', quality=85)
                pdf.image(temp_img.name, x=10, y=y, w=50)
        except Exception as e:
            print(f"Ошибка добавления изображения: {e}")

    def _add_asset_details(self, pdf: FPDF, asset_data: dict, colors: dict):
        """Добавление деталей актива"""
        pdf.set_font("Arial", size=self.font_sizes['body'])
        pdf.set_text_color(*colors['text'])
        
        fields = [
            ("Наименование", asset_data['name']),
            ("Тип", asset_data['type']),
            ("Модель", asset_data['model']),
            ("Серийный номер", asset_data.get('serial', '-')),
            ("Инв. номер", asset_data['inv_num']),
            ("Дата постановки на учет", asset_data['date']),
            ("Подразделение", asset_data['department']),
            ("МОЛ", asset_data['responsible']),
            ("Статус", asset_data['status']),
            ("Местоположение", asset_data['location'])
        ]
        
        for name, value in fields:
            pdf.set_font("Arial", 'B', self.font_sizes['body'])
            pdf.cell(40, 8, f"{name}:", 0, 0)
            pdf.set_font("Arial", size=self.font_sizes['body'])
            pdf.multi_cell(0, 8, str(value) if value else "-", 0, 1)
            pdf.ln(2)