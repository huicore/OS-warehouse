using System.Drawing;
using System.Windows.Forms;

namespace FixedAssetInventory.Controls
{
    public class CustomDataGridView : DataGridView
    {
        public CustomDataGridView()
        {
            // Базовая настройка внешнего вида
            this.BorderStyle = BorderStyle.None;
            this.BackgroundColor = Color.White;
            this.GridColor = Color.FromArgb(240, 240, 240);
            this.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            this.EnableHeadersVisualStyles = false;
            this.DoubleBuffered = true;

            // Настройка заголовков столбцов
            this.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(3)
            };

            // Настройка строк
            this.RowHeadersVisible = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Альтернативные цвета строк
            this.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.FromArgb(248, 248, 248)
            };

            // Стиль ячеек
            this.DefaultCellStyle = new DataGridViewCellStyle()
            {
                Padding = new Padding(3),
                SelectionBackColor = Color.FromArgb(229, 243, 255),
                SelectionForeColor = Color.Black
            };
        }

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            // Подсветка строки при наведении
            if ((e.State & DataGridViewElementStates.Selected) != DataGridViewElementStates.Selected)
            {
                if (this.Rows[e.RowIndex].Selected || 
                    this.Rows[e.RowIndex].Cells[0].Selected)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(229, 243, 255)))
                    {
                        e.Graphics.FillRectangle(brush, e.RowBounds);
                    }
                }
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            // Кастомный рендеринг заголовков столбцов
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                using (var brush = new SolidBrush(this.ColumnHeadersDefaultCellStyle.ForeColor))
                {
                    e.Graphics.DrawString(e.FormattedValue.ToString(),
                        this.ColumnHeadersDefaultCellStyle.Font,
                        brush,
                        new Rectangle(e.CellBounds.X + 3, e.CellBounds.Y, 
                                     e.CellBounds.Width - 6, e.CellBounds.Height),
                        new StringFormat 
                        { 
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        });
                }

                // Граница снизу
                using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                {
                    e.Graphics.DrawLine(pen,
                        e.CellBounds.Left, e.CellBounds.Bottom - 1,
                        e.CellBounds.Right, e.CellBounds.Bottom - 1);
                }

                e.Handled = true;
            }

            // Границы ячеек
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                using (var pen = new Pen(this.GridColor))
                {
                    e.Graphics.DrawLine(pen,
                        e.CellBounds.Left, e.CellBounds.Bottom - 1,
                        e.CellBounds.Right, e.CellBounds.Bottom - 1);
                }
            }
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            // Обновление при скроллинге для плавного отображения
            this.Invalidate();
            base.OnScroll(e);
        }
    }
}