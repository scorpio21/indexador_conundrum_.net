#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void drawTimer_Tick(object? sender, EventArgs e)
        {
            if (_selectedRecord is null || !_selectedRecord.IsAnimation) return;
            if (_selectedRecord.Frames.Count == 0)
            {
                _drawTimer.Stop();
                return;
            }
            var elapsed = _animationClock.ElapsedMilliseconds;
            var elapsedSinceFrame = elapsed - _animationElapsed;
            if (elapsedSinceFrame < _animationIntervalMs) return;
    
            var framesToAdvance = (int)Math.Min(elapsedSinceFrame / _animationIntervalMs, _selectedRecord.Frames.Count);
            _animationFrame = (_animationFrame + framesToAdvance) % _selectedRecord.Frames.Count;
            _animationElapsed += (long)framesToAdvance * _animationIntervalMs;
            previewPanel.Invalidate();
        }
    
            private void ConfigureAnimationTimer(GrhRecord record)
        {
            if (!record.IsAnimation || record.Speed <= 0)
            {
                _drawTimer.Stop();
                return;
            }
            _animationFrame = 0;
            _animationIntervalMs = Math.Clamp((int)Math.Round(record.Speed * (1000D / 60D)), 16, 1000);
            _drawTimer.Interval = Math.Min(15, _animationIntervalMs);
            WarmAnimationBitmaps(record);
            _animationClock.Restart();
            _animationElapsed = 0;
            _drawTimer.Start();
        }
    
            private void WarmAnimationBitmaps(GrhRecord animation)
        {
            var graphicsFolder = ResolvePath(_settings.GraphicsPath);
            foreach (var frameId in animation.Frames)
            {
                var frame = _file?.Records.FirstOrDefault(x => x.Id == frameId);
                if (frame is null) continue;
                var bitmapPath = Path.Combine(graphicsFolder, $"{frame.FileNumber}.bmp");
                if (File.Exists(bitmapPath)) _ = GetCachedBitmap(bitmapPath);
            }
        }
    
            private GrhRecord? GetPreviewRecord(out string? error)
        {
            error = null;
            if (_selectedRecord is null) return null;
            if (!_selectedRecord.IsAnimation) return _selectedRecord;
            if (_file is null)
            {
                error = "No hay un índice gráfico cargado para resolver la animación.";
                return null;
            }
            if (_animationFrame < 0 || _animationFrame >= _selectedRecord.Frames.Count)
            {
                error = $"Frame fuera de rango: {_animationFrame + 1}.";
                return null;
            }
            var frameId = _selectedRecord.Frames[_animationFrame];
            var frame = _file.Records.FirstOrDefault(x => x.Id == frameId);
            if (frame is null) error = $"No existe el GRH de frame {frameId}.";
            return frame;
        }
    
            private void previewPanel_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.Clear(previewPanel.BackColor);
            if (_autoPreviewRecords is { Count: > 0 })
            {
                DrawAutoIndexerPreview(e.Graphics, _autoPreviewRecords);
                return;
            }
            if (_selectedRecord is null)
            {
                ClearUtilError();
                return;
            }
    
            var record = GetPreviewRecord(out var frameError);
            if (record is null)
            {
                SetUtilError(frameError ?? "No hay un recurso seleccionado.");
                using var errorBrush = new SolidBrush(Color.DarkRed);
                e.Graphics.DrawString(frameError ?? "No hay un recurso seleccionado.", Font, errorBrush, new PointF(12, 12));
                return;
            }
    
            var graphicsFolder = ResolvePath(_settings.GraphicsPath);
            var bitmapPath = Path.Combine(graphicsFolder, $"{record.FileNumber}.bmp");
            if (!File.Exists(bitmapPath))
            {
                SetUtilError($"BMP {record.FileNumber} no encontrado: {bitmapPath}");
                using var brush = new SolidBrush(Color.DimGray);
                using var font = new Font(Font.FontFamily, 10F);
                e.Graphics.DrawString($"BMP {record.FileNumber} no encontrado\n{bitmapPath}", font, brush, new PointF(12, 12));
                return;
            }
    
            try
            {
                var bitmap = GetCachedBitmap(bitmapPath);
                var source = new Rectangle(record.X, record.Y, record.Width, record.Height);
                if (source.Width <= 0 || source.Height <= 0)
                {
                    SetUtilError($"GRH {record.Id}: dimensiones inválidas ({source.Width}x{source.Height}).");
                    using var invalidBrush = new SolidBrush(Color.DarkRed);
                    e.Graphics.DrawString($"GRH {record.Id}: dimensiones inválidas ({source.Width}x{source.Height}).", Font, invalidBrush, new PointF(12, 12));
                    return;
                }
                if (source.X < 0 || source.Y < 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height)
                {
                    SetUtilError($"GRH {record.Id}: recorte fuera de BMP {record.FileNumber} ({bitmap.Width}x{bitmap.Height}).");
                    using var invalidBrush = new SolidBrush(Color.DarkRed);
                    e.Graphics.DrawString($"GRH {record.Id}: recorte fuera de BMP {record.FileNumber} ({bitmap.Width}x{bitmap.Height}).", Font, invalidBrush, new PointF(12, 12));
                    return;
                }
                var scale = Math.Min((double)(previewPanel.ClientSize.Width - 20) / source.Width, (double)(previewPanel.ClientSize.Height - 20) / source.Height);
                scale = Math.Min(scale, 4D);
                var dest = new Rectangle((previewPanel.ClientSize.Width - (int)(source.Width * scale)) / 2, (previewPanel.ClientSize.Height - (int)(source.Height * scale)) / 2, (int)(source.Width * scale), (int)(source.Height * scale));
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                e.Graphics.DrawImage(bitmap, dest, source, GraphicsUnit.Pixel);
                ClearUtilError();
            }
            catch (Exception ex)
            {
                SetUtilError($"No se pudo dibujar el BMP: {ex.Message}");
                using var brush = new SolidBrush(Color.DarkRed);
                e.Graphics.DrawString($"No se pudo dibujar el BMP: {ex.Message}", Font, brush, new PointF(12, 12));
            }
        }

        private void DrawAutoIndexerPreview(Graphics graphics, IReadOnlyList<GrhRecord> records)
        {
            var first = records[0];
            var graphicsFolder = ResolvePath(_settings.GraphicsPath);
            var bitmapPath = Path.Combine(graphicsFolder, $"{first.FileNumber}.bmp");
            if (!File.Exists(bitmapPath))
            {
                using var brush = new SolidBrush(Color.DarkRed);
                graphics.DrawString($"BMP {first.FileNumber} no encontrado", Font, brush, new PointF(12, 12));
                return;
            }
            using var bitmap = new Bitmap(bitmapPath);
            var scale = Math.Min((double)(previewPanel.ClientSize.Width - 20) / bitmap.Width,
                (double)(previewPanel.ClientSize.Height - 20) / bitmap.Height);
            scale = Math.Min(scale, 4D);
            var destination = new Rectangle(
                (previewPanel.ClientSize.Width - (int)(bitmap.Width * scale)) / 2,
                (previewPanel.ClientSize.Height - (int)(bitmap.Height * scale)) / 2,
                (int)(bitmap.Width * scale), (int)(bitmap.Height * scale));
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            graphics.DrawImage(bitmap, destination);
            using var pen = new Pen(Color.Red, Math.Max(1F, (float)scale));
            using var textBrush = new SolidBrush(Color.Red);
            foreach (var record in records)
            {
                if (record.FileNumber != first.FileNumber || record.Width <= 0 || record.Height <= 0) continue;
                var rectangle = new Rectangle(
                    destination.Left + (int)(record.X * scale),
                    destination.Top + (int)(record.Y * scale),
                    Math.Max(1, (int)(record.Width * scale)),
                    Math.Max(1, (int)(record.Height * scale)));
                graphics.DrawRectangle(pen, rectangle);
                graphics.DrawString(record.Id.ToString(), Font, textBrush, rectangle.Location);
            }
        }
}

#pragma warning restore CS8618

