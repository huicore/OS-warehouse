from sqlalchemy import Column, Integer, String, Date, Enum
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

# Полный список подразделений
DEPARTMENTS = [
    'МТО', 'ОКС', 'СБ', 'СГИ', 'СГЭ', 
    'Служба обеспечения производства', 
    'СОТиПБ', 'СпГР', 
    'Служба по металлургии', 
    'Служба по минеральным ресурсам', 
    'Служба технического обслуживания и ремонтов', 
    'Служба управления персоналом', 
    'Финансово-экономическая служба', 
    'Администрация', 
    'Бизнес-система', 
    'Отдел охраны окружающей среды', 
    'Отдел технического контроля'
]

class FixedAsset(Base):
    __tablename__ = 'fixed_assets'
    
    id = Column(Integer, primary_key=True)
    inventory_number = Column(String, unique=True)
    asset_type = Column(Enum('Принтер', 'Системный блок', 'Ноутбук', 'Монитор', 'ИБП', name='asset_type_enum'))
    name = Column(String)
    serial_number = Column(String)
    status = Column(Enum('В работе', 'На складе', 'Требуется ремонт', 'Вышел из строя', name='status_enum'))
    department = Column(Enum(*DEPARTMENTS, name='department_enum'))  # Исправлено: развернут список
    responsible = Column(String)
    location = Column(String)
    acquisition_date = Column(Date)
    notes = Column(String)