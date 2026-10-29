#pragma warning disable CS8618

namespace EncodeNet8;

public partial class MainForm : Form
{
        private void drawTimer_Tick(object? sender, EventArgs e)
        {
            AdvanceAnimationFrame(_animationClock.ElapsedMilliseconds);
        }

        private void AdvanceAnimationFrame(long elapsed)
        {
            if (_selectedRecord is null || !_selectedRecord.IsAnimation) return;
            if (_selectedRecord.Frames.Count == 0)
            {
                _drawTimer.Stop();
                return;
            }
            var elapsedSinceFrame = elapsed - _animationElapsed;
            if (elapsedSinceFrame < _animationIntervalMs) return;
    
            var framesToAdvance = (int)Math.Min(elapsedSinceFrame / _animationIntervalMs, _selectedRecord.Frames.Count);
            _animationFrame = (_animationFrame + framesToAdvance) % _selectedRecord.Frames.Count;
            _animationElapsed += (long)framesToAdvance * _animationIntervalMs;
            previewPanel.Invalidate();
        }
    
            private void ConfigureAnimationTimer(GrhRecord record)
        {
            if (!record.IsAnimation)
            {
                _drawTimer.Stop();
                return;
            }
            _animationFrame = 0;
            // VB6: Dibujado.Interval = 50 * Speed; si Speed <= 0 usa 100 ms.
            _animationIntervalMs = record.Speed > 0 ? Math.Clamp(record.Speed * 50, 16, 1000) : 100;
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
            if (_drawWalk == 0 && IsDirectionalStateForPreview(_indexerState))
            {
                if (_indexerState.Equals("Body", StringComparison.OrdinalIgnoreCase))
                    DrawBodyPreview(e.Graphics);
                else
                    DrawAllDirectionalPreview(e.Graphics);
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

        private void DrawBodyPreview(Graphics graphics)
        {
            if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _bodyRecords.Count || _file is null)
                return;

            var body = _bodyRecords[indexList.SelectedIndex];
            var width = Math.Max(1, previewPanel.ClientSize.Width);
            var height = Math.Max(1, previewPanel.ClientSize.Height);
            var margin = 12;
            var cellWidth = Math.Max(1, (width - margin * 3) / 2);
            var cellHeight = Math.Max(1, (height - margin * 3) / 2);
            var slots = new[]
            {
                new Rectangle(margin, margin, cellWidth, cellHeight),
                new Rectangle(margin * 2 + cellWidth, margin, cellWidth, cellHeight),
                new Rectangle(margin, margin * 2 + cellHeight, cellWidth, cellHeight),
                new Rectangle(margin * 2 + cellWidth, margin * 2 + cellHeight, cellWidth, cellHeight)
            };
            var directions = new[] { body.Walk1, body.Walk2, body.Walk3, body.Walk4 };
            var drawnBody = new Rectangle[4];
            GrhRecord? southBody = null;

            for (var i = 0; i < directions.Length; i++)
            {
                var record = _file.Records.FirstOrDefault(item => item.Id == directions[i]);
                if (record is null) continue;
                var frame = ResolvePreviewFrame(record);
                if (frame is null) continue;
                drawnBody[i] = DrawRecordFit(graphics, frame, slots[i]);
                if (i == 2) southBody = frame;
            }

            if (cabezaActual > 0 && cabezaActual <= 32720 && drawnBody[2].Width > 0)
            {
                var head = _file.Records.FirstOrDefault(item => item.Id == cabezaActual);
                if (head is not null)
                    DrawOverlay(graphics, head, southBody, drawnBody[2]);
            }
        }

        private void DrawAllDirectionalPreview(Graphics graphics)
        {
            if (indexList.SelectedIndex < 0 || indexList.SelectedIndex >= _directionalRecords.Count || _file is null)
                return;

            var item = _directionalRecords[indexList.SelectedIndex];
            var width = Math.Max(1, previewPanel.ClientSize.Width);
            var height = Math.Max(1, previewPanel.ClientSize.Height);
            var margin = 12;
            var cellWidth = Math.Max(1, (width - margin * 3) / 2);
            var cellHeight = Math.Max(1, (height - margin * 3) / 2);
            var slots = new[]
            {
                new Rectangle(margin, margin, cellWidth, cellHeight),
                new Rectangle(margin * 2 + cellWidth, margin, cellWidth, cellHeight),
                new Rectangle(margin, margin * 2 + cellHeight, cellWidth, cellHeight),
                new Rectangle(margin * 2 + cellWidth, margin * 2 + cellHeight, cellWidth, cellHeight)
            };

            for (var i = 0; i < item.Directions.Length && i < slots.Length; i++)
            {
                var record = _file.Records.FirstOrDefault(candidate => candidate.Id == item.Directions[i]);
                if (record is null) continue;
                var frame = ResolvePreviewFrame(record);
                if (frame is not null) DrawRecordFit(graphics, frame, slots[i]);
            }
        }

        private static bool IsDirectionalStateForPreview(string state) =>
            state is "Body" or "Cabezas" or "Cascos" or "Escudos" or "Armas" or "Alas" or "Capas" or "Botas";

        private GrhRecord? ResolvePreviewFrame(GrhRecord record)
        {
            if (!record.IsAnimation || _file is null || record.Frames.Count == 0) return record;
            var frameId = record.Frames[Math.Clamp(_animationFrame, 0, record.Frames.Count - 1)];
            return _file.Records.FirstOrDefault(item => item.Id == frameId);
        }

        private Rectangle DrawRecordFit(Graphics graphics, GrhRecord record, Rectangle slot)
        {
            var bitmapPath = Path.Combine(ResolvePath(_settings.GraphicsPath), $"{record.FileNumber}.bmp");
            if (!File.Exists(bitmapPath) || record.Width <= 0 || record.Height <= 0) return Rectangle.Empty;
            try
            {
                var bitmap = GetCachedBitmap(bitmapPath);
                var source = new Rectangle(record.X, record.Y, record.Width, record.Height);
                if (source.X < 0 || source.Y < 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height) return Rectangle.Empty;
                var scale = Math.Min((double)slot.Width / source.Width, (double)slot.Height / source.Height);
                scale = Math.Min(scale, 4D);
                var dest = new Rectangle(slot.Left + (slot.Width - (int)(source.Width * scale)) / 2,
                    slot.Top + (slot.Height - (int)(source.Height * scale)) / 2,
                    Math.Max(1, (int)(source.Width * scale)), Math.Max(1, (int)(source.Height * scale)));
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                graphics.DrawImage(bitmap, dest, source, GraphicsUnit.Pixel);
                return dest;
            }
            catch
            {
                return Rectangle.Empty;
            }
        }

        private void DrawOverlay(Graphics graphics, GrhRecord record, GrhRecord? bodyRecord, Rectangle bodyBounds)
        {
            if (bodyRecord is null || bodyRecord.Width <= 0 || bodyRecord.Height <= 0 || record.Width <= 0 || record.Height <= 0) return;
            var bitmapPath = Path.Combine(ResolvePath(_settings.GraphicsPath), $"{record.FileNumber}.bmp");
            if (!File.Exists(bitmapPath)) return;
            try
            {
                var bitmap = GetCachedBitmap(bitmapPath);
                var source = new Rectangle(record.X, record.Y, record.Width, record.Height);
                if (source.X < 0 || source.Y < 0 || source.Right > bitmap.Width || source.Bottom > bitmap.Height) return;
                var scale = Math.Min((double)bodyBounds.Width / bodyRecord.Width, (double)bodyBounds.Height / bodyRecord.Height);
                var destWidth = Math.Max(1, (int)(source.Width * scale));
                var destHeight = Math.Max(1, (int)(source.Height * scale));
                var dest = new Rectangle(
                    bodyBounds.Left + (bodyBounds.Width - destWidth) / 2 + (int)(_previewHeadOffsetX * scale),
                    bodyBounds.Bottom - destHeight + (int)((_previewHeadOffsetY - 1) * scale),
                    destWidth, destHeight);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                graphics.DrawImage(bitmap, dest, source, GraphicsUnit.Pixel);
            }
            catch
            {
                // El visor sigue mostrando el body aunque falte el GRH de cabeza.
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
            var bitmap = GetCachedBitmap(bitmapPath);
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

