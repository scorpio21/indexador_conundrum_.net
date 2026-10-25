# Changelog

Todos los cambios relevantes de este proyecto se documentan aquí.

## [Unreleased]

### Añadido

- Hoja de ruta de migración y matriz de cobertura en `AUDITORIA_MIGRACION_VB6_NET8.md`.
- Configuración de rutas `Init` y `Gráficos` mediante JSON.
- Carga automática de `Init\\Graficos.ind`.
- Lectura de `Graficos.ind` con cabecera VB6 de 263 bytes.
- Soporte inicial para IDs GRH y frames en formato Integer y Long.
- Cambio de estado y visor inicial para GRH y Bodies.
- Menús principales compatibles con la organización del indexador VB6.

### Pendiente

- Completar todos los estados de índice y sus formatos binarios.
- Completar edición, guardado, búsquedas, superficies y autoindexador.
- Añadir fixtures y pruebas de compatibilidad Integer/Long.

## [0.1.0] - 2026-10-25

### Añadido

- Port inicial a C# .NET 8 WinForms.
- Solución editable desde Visual Studio.
- Primer visor GDI+ para recursos indexados.
- Publicación inicial en GitHub.
