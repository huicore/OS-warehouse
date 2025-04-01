public class CustomDataGridView : DataGridView
{
    public CustomDataGridView()
    {
        DoubleBuffered = true;
        EnableHeadersVisualStyles = false;
    }

    protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
    {
        // Кастомный рендеринг строк
        base.OnRowPostPaint(e);
        
        if (Rows[e.RowIndex].Selected)
        {
            using (var brush = new SolidBrush(Color.FromArgb(229, 243, 255)))
            {
                e.Graphics.FillRectangle(brush, e.RowBounds);
            }
        }
    }

    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
        // Кастомный рендеринг ячеек
        base.OnCellPainting(e);
        
        if (e.RowIndex == -1 && e.ColumnIndex >= 0) // Заголовки столбцов
        {
            e.PaintBackground(e.CellBounds, true);
            
            using (var brush = new SolidBrush(ThemeManager.CurrentTheme.HeaderForeColor))
            {
                e.Graphics.DrawString(e.FormattedValue.ToString(), 
                    e.CellStyle.Font, brush, e.CellBounds, 
                    new StringFormat 
                    { 
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center 
                    });
            }
            
            e.Handled = true;
        }
    }
}