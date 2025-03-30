import pandas as pd
from openpyxl import Workbook

def export_to_excel(data, filename):
    df = pd.DataFrame(data)
    writer = pd.ExcelWriter(filename, engine='openpyxl')
    df.to_excel(writer, index=False)
    writer.close()