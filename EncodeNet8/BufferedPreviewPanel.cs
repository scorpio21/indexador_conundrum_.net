namespace EncodeNet8;

public sealed class BufferedPreviewPanel : Panel
{
    public BufferedPreviewPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}
