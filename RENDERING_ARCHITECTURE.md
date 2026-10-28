# Arquitectura del visor

## Decisión del issue #9

El visor del indexador no porta DirectX7. Se mantiene una implementación GDI+ específica para la herramienta:

- `BufferedPreviewPanel` activa `DoubleBuffered` para evitar parpadeo.
- `MainForm/Rendering/MainForm.Rendering.cs` resuelve el GRH, obtiene el BMP desde la caché, aplica el rectángulo `X/Y/Width/Height` y lo dibuja con `Graphics.DrawImage`.
- El temporizador WinForms reproduce la velocidad del índice y avanza los frames con un reloj monotónico.
- Los errores de BMP ausente, frame inexistente y recorte fuera de límites se muestran en `LUlitError` y no provocan una excepción del visor.

## APIs necesarias

La preview solo necesita `System.Drawing.Bitmap`, `Graphics`, transparencia, `System.Windows.Forms.Timer` y el panel doblemente almacenado. No necesita dispositivo Direct3D, superficies DirectDraw, `DirectX7`, `Microsoft.DirectX` ni `Direct3D7`.

## Alcance y limitaciones

Este visor administra una preview 2D de índices, no es el renderer del cliente de juego. No implementa aceleración GPU, batching, mapas completos ni el gestor de superficies runtime del cliente. Esa diferencia es intencionada y evita introducir dependencia COM o DirectX7 en el indexador.

Si GDI+ resulta insuficiente en el futuro, la evolución debe introducir una interfaz aislada (`IPreviewRenderer`) y una segunda implementación intercambiable. La comparación se hará con los mismos BMP, rectángulos, frames y capturas; el renderer GDI+ permanecerá como referencia de regresión.

## Verificación

La búsqueda del repositorio no encuentra llamadas a `Microsoft.DirectX`, `DirectDraw`, `Direct3D` ni `DX7` en los archivos C# del proyecto. Las pruebas WinForms cubren preview, animaciones de 1/4/15/25 frames, caché y errores de recursos.
