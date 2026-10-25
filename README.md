# EncodeNet8

Port inicial del indexador Conundrum de AOSpain a Windows Forms .NET 8.

- Solución editable en Visual Studio: `EncodeNet8.slnx`.
- Formulario editable: `EncodeNet8/MainForm.cs` y `MainForm.Designer.cs`.
- Carga y guardado de `Graficos.ind` en formato Integer original y Long para el ID/frames.
- Edición directa en la cuadrícula.
- Interfaz WinForms editable desde Visual Studio, con configuración JSON de las rutas `Init` y `Gráficos`.
- Carga inicial automática de `Graficos.ind` y visor GDI+ para los BMP indexados.
- Auditoría de migración VB6 disponible en `AUDITORIA_MIGRACION_VB6_NET8.md`.

El manual original se ha usado como especificación para continuar con bodies, cabezas, cascos, escudos, armas, FX, búsquedas y autoindexador.
