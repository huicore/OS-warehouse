# Настройки интерфейса
BUTTONS = {
    'import': {
        'text': "Импорт из Excel",
        'icon': "📊"
    },
    'print': {
        'text': "Печать",
        'icon': "🖨️"
    }
}

# Настройки Excel
REQUIRED_COLUMNS = [
    'Наименование', 'Тип', 'Инв. №'
]

# Настройки печати
PRINT_STYLES = """
    table { width: 100%; border-collapse: collapse; }
    th { background: #f0f0f0; text-align: left; }
    td, th { padding: 5px; border: 1px solid #ddd; }
"""