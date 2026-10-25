# Changelog

Todos los cambios relevantes de este proyecto se documentan aquí.

## [0.0.6-beta] - 2026-10-25

### En progreso — Issue #4

- Se añadió el lector `FxIndexFile` para `Fxs.ind` Integer/Long.
- Se añadió el lector INI `IniDirectionalIndexFile` para `Armas.dat` y `Escudos.dat`, incluyendo comentarios y secciones con huecos.
- FX, Armas y Escudos ya cargan sus índices ocupados en las listas y muestran sus referencias principales.
- Validación real: 20 FX, 16 armas y 5 escudos ocupados.
- Pruebas sintéticas Integer/Long y validación de archivos reales añadidas al proyecto de pruebas.
- Issue todavía abierto: [#4](https://github.com/scorpio21/indexador_conundrum_.net/issues/4).

### En progreso — Issue #3

- Se añadió `DirectionalIndexFile` para leer registros de cuatro direcciones en formatos Integer y Long.
- Se añadieron offsets X/Y para cuerpos, alas y botas.
- Bodies, Cabezas y Cascos ya cargan sus registros ocupados y se muestran en la lista y el visor.
- Validación real: `Personajes.ind` (277 cuerpos), `Cabezas.ind` (57 cabezas) y `Cascos.ind` (9 cascos).
- Se añadieron pruebas sintéticas de registros direccionales y un modo de validación de archivos reales.
- Issue todavía abierto: [#3](https://github.com/scorpio21/indexador_conundrum_.net/issues/3).

### Completado — Issue #2

- Se implementó el flujo de estados equivalente a `CambiarEstado` y `ComprobarIndexLista`.
- Los botones de Gráficos, Bodies, Cabezas, Cascos, Escudos, Armas y FX ya no son acciones vacías.
- Se añadieron los estados Alas, Capas, Recursos y Botas, con restauración de selección independiente.
- Las listas filtran entradas ocupadas en archivos binarios y `.dat`, igual que las rutinas `RenuevaLista...` del VB6.
- Los `.dat` aceptan comentarios de línea y se validan las secciones y el primer GRH de cada índice.
- Recursos carga los BMP numerados disponibles de la carpeta de gráficos.
- La selección de estados pendientes limpia el visor y evita interpretar un slot como si fuera un GRH.
- Se añadieron pruebas sintéticas para listas binarias e INI.
- Issue cerrado: [#2](https://github.com/scorpio21/indexador_conundrum_.net/issues/2).

### Completado — Issue #1

- Se endureció la lectura de `Graficos.ind`: cabecera, registros incompletos, IDs duplicados/desordenados, frames inválidos y datos posteriores al terminador ahora se detectan explícitamente.
- El escritor conserva la cabecera original y respeta el formato VB6, que finaliza al terminar el último registro.
- Se añadieron pruebas reproducibles para Integer, Long, GRH superiores a `32767` y archivos truncados.
- Se validaron archivos reales: `K:\\AO_Aospain\\cli\\Init\\Graficos.ind` (12.597 GRH) y `K:\\AO_Aospain\\utilidades\\init\\Graficos.ind` (12.620 GRH).
- Issue cerrado: [#1](https://github.com/scorpio21/indexador_conundrum_.net/issues/1).

### Añadido

- Hoja de ruta de migración y matriz de cobertura en `AUDITORIA_MIGRACION_VB6_NET8.md`.
- Configuración de rutas `Init` y `Gráficos` mediante JSON.
- Carga automática de `Init\\Graficos.ind`.
- Lectura de `Graficos.ind` con cabecera VB6 de 263 bytes.
- Soporte inicial para IDs GRH y frames en formato Integer y Long.
- Cambio de estado y visor inicial para GRH y Bodies.
- Menús principales compatibles con la organización del indexador VB6.
- Issues de implantación [#2](https://github.com/scorpio21/indexador_conundrum_.net/issues/2) a [#9](https://github.com/scorpio21/indexador_conundrum_.net/issues/9).

### Pendiente

- Completar todos los estados de índice y sus formatos binarios.
- Completar edición, guardado, búsquedas, superficies y autoindexador.
- Añadir fixtures y pruebas de compatibilidad Integer/Long.

### Política de cierre

Cada issue se cerrará únicamente cuando la funcionalidad esté implementada, probada, documentada en esta sección y subida a Git. Las versiones futuras enlazarán explícitamente cada cambio con su issue.

## [0.0.1] - 2026-10-25

### Añadido

- Port inicial a C# .NET 8 WinForms.
- Solución editable desde Visual Studio.
- Primer visor GDI+ para recursos indexados.
- Publicación inicial en GitHub.
