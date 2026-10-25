# Auditoría de migración VB6 → C# .NET 8 WinForms

Fecha: 2026-10-25  
Origen: `K:\AO_Aospain\utilidades\Code`  
Destino: `K:\AO_Aospain\utilidades\EncodeNet8\EncodeNet8`

## Alcance y criterio

Esta auditoría compara los 14 archivos VB6 del indexador original con los archivos C# existentes. Los porcentajes son una estimación de cobertura funcional, no una medida de líneas traducidas. Un componente solo se considerará completo cuando conserve flujo, formato de archivo y resultado observable.

No se ha convertido DirectX 7 automáticamente. La representación equivalente se hará en WinForms/GDI+ solo para el visor del indexador; el motor DirectDraw quedará como una fase separada.

## Mapa de dependencias

```text
Encode.frm
 ├─ GameIni.bas              configuración, cabecera, INI y rutas
 ├─ GeneralES.bas            carga/guardado de GRH, bodies, cabezas, cascos, FX...
 ├─ Indices.bas              listas, estados, altas, bajas, movimientos y búsquedas
 ├─ TileEngine.bas           DirectDraw, superficies, visor, dibujo y animaciones
 ├─ Declares.bas             variables globales, Types y Enums
 ├─ DX_InIt.bas              inicialización/liberación DirectX 7
 ├─ clsIniReader.cls         parser INI
 ├─ clsSurfaceManager.cls    interfaz de superficies DirectDraw
 ├─ clsSurfaceManDyn.cls     caché dinámico/LRU de superficies
 ├─ clsSurfaceManStatic.cls  caché estático de superficies
 ├─ APIdeclaraciones.bas     APIs Win32 de ventana/resolución
 ├─ FormAuto.frm             autoindexador y generación de índices
 └─ FrmSearch.frm            búsqueda de GRH/BMP/repetidos

 FrmConfig.frm               formulario de configuración mínimo
```

## Componentes VB6 identificados

### Globales, Types y Enums

`Declares.bas` define los Types gráficos y de índices: `Grhdata`, `Grh`, `BodyData`, `HeadData`, `FxData`, `tIndiceCuerpo`, `tIndiceCuerpoLong`, `tIndiceCabeza`, `tIndiceCabezaLong`, `tIndiceFx`, `tIndiceFxLong`, alas, botas, BMP y recursos.

También define `e_EstadoIndexador`, cuyos estados son GRH, Body, Cabezas, Cascos, Escudos, Armas, Alas, Capas, FX, Botas y Resource.

`GameIni.bas` define `tCabecera` y `tGameIni`. La cabecera binaria real usada por el indexador es `String * 255 + Long + Long`, es decir, 263 bytes, seguida por los campos propios de cada índice.

### Lectura y escritura de archivos

- `Graficos.ind`: cabecera, cinco `Integer` reservados y registros GRH Integer o Long.
- `Personajes.ind`: cabecera, cantidad y registros `tIndiceCuerpo`/`tIndiceCuerpoLong`.
- `Cabezas.ind`, `Cascos.ind`, `Capas.ind`, `Alas.ind`, `Botas.ind`, `Fxs.ind`: cabecera, cantidad y estructuras específicas.
- `Armas` y `Escudos`: en esta versión original se guardan principalmente como DAT/INI.
- `Inicio.con`, `Conf.ini`, `Tips.ayu`, `FK.ind`, mapas y archivos auxiliares.
- `Graficos\N.bmp`: superficies gráficas numéricas cargadas bajo demanda o mediante caché.

### DirectX y Win32

- `DX_InIt.bas` crea y libera `DirectDraw7`, superficies primaria/secundaria y backbuffer.
- `TileEngine.bas` usa `DirectDrawSurface7`, `BltFast`, color key, `BltToDC`, locks de superficie y rutinas de dibujo de personajes/GRH.
- `clsSurfaceManDyn.cls` implementa caché dinámica y eliminación LRU.
- `clsSurfaceManStatic.cls` precarga superficies y usa búsqueda binaria.
- `APIdeclaraciones.bas` declara `FindWindow`, `EnumDisplaySettings`, `ChangeDisplaySettings`, `GetActiveWindow`.

## Matriz de cobertura

| VB6 FILE | C# FILE | % migrado | Estado | Observaciones |
|---|---|---:|---|---|
| `Encode.frm` | `MainForm.cs`, `MainForm.Designer.cs` | 35% | Parcial | Menús, configuración, estados GRH/Body, selección, visor y acciones básicas. Faltan edición completa, todos los estados y formularios auxiliares. |
| `GeneralES.bas` | `GraficosIndFile.cs`, `MainForm.cs`, `GrhModels.cs` | 20% | Parcial | GRH Integer/Long y Body Integer/Long parcial. Faltan cabezas, cascos, capas, alas, botas, FX, armas, escudos y guardado equivalente. |
| `Indices.bas` | `MainForm.cs` | 15% | Parcial | Lista GRH, `CambiarEstado`, `ComprobarIndexLista`, selección y Body inicial. Faltan altas, bajas, mover, clonar, búsquedas y listas de todos los estados. |
| `TileEngine.bas` | `MainForm.cs` | 8% | Parcial | Visor GDI+ de BMP/recorte y timer de animación. No es una migración del motor DirectDraw. |
| `Declares.bas` | `GrhModels.cs` | 10% | Parcial | `GrhRecord`, `GrhStorageFormat` y `BodyIndexRecord`. Faltan la mayoría de Types, globales y Enums. |
| `GameIni.bas` | `IndexerSettings.cs`, `MainForm.cs` | 35% | Parcial | Rutas configurables y JSON. Falta `Inicio.con`, cabecera completa, recursos y utilidades de BMP. |
| `clsIniReader.cls` | `IndexerSettings.cs` | 15% | Parcial | JSON sustituye parte de la configuración; no existe parser INI compatible completo. |
| `clsSurfaceManager.cls` | — | 0% | No implementado | Interfaz DirectDraw sin equivalente C#. |
| `clsSurfaceManDyn.cls` | — | 0% | No implementado | Caché dinámica/LRU DirectDraw no migrada. |
| `clsSurfaceManStatic.cls` | — | 0% | No implementado | Precarga y caché estática DirectDraw no migrada. |
| `DX_InIt.bas` | — | 0% | No implementado intencionadamente | No se convierte DirectX 7 automáticamente. |
| `APIdeclaraciones.bas` | — | 0% | No implementado | Solo debe migrarse lo necesario para el comportamiento WinForms final. |
| `FormAuto.frm` | — | 0% | No implementado | Autoindexador y plantillas de animación pendientes. |
| `FrmSearch.frm` | — | 0% | No implementado | Búsqueda de GRH/BMP/repetidos pendiente. |
| `FrmConfig.frm` | `SettingsForm.cs`, `SettingsForm.Designer.cs` | 70% | Parcial | Existe ventana de configuración de rutas, pero no reproduce toda la configuración VB6. |

## Destino C# actual

- `GraficosIndFile.cs`: lector/escritor de `Graficos.ind`, con conservación de cabecera.
- `GrhModels.cs`: modelos GRH y Body inicial.
- `MainForm.cs`: menú, rutas JSON, estados, listas, selección, visor GDI+ y timer.
- `MainForm.Designer.cs`: controles editables del formulario.
- `IndexerSettings.cs`: configuración JSON.
- `SettingsForm.cs`/`SettingsForm.Designer.cs`: diálogo WinForms de rutas.
- `Program.cs`: punto de entrada.

## Plan de implementación incremental

1. Congelar el comportamiento GRH ya corregido y crear pruebas de lectura contra los `.ind` originales.
2. Extraer un modelo común `IndexState` para que `CambiarEstado` y `ComprobarIndexLista` funcionen igual en todos los tipos.
3. Implementar lectores binarios específicos para Cabezas, Cascos, Capas, Alas, Botas y FX.
4. Implementar Armas y Escudos respetando sus DAT/INI originales.
5. Implementar `ClickEnLista` por estado y el dibujo de las cuatro direcciones.
6. Implementar Guardar, Borrar, Mover, Copiar y Clonar por estado.
7. Migrar búsquedas y autoindexador desde `FrmSearch.frm` y `FormAuto.frm`.
8. Sustituir únicamente el visor DirectDraw del indexador por GDI+/WinForms; no trasladar DirectX 7 al azar.
9. Verificar cada fase contra los archivos binarios originales y compilar después de cada grupo.

## Restricciones de la migración

- No reescribir código ya cubierto sin una prueba de regresión.
- No usar `Integer` donde el archivo indique un índice GRH Long.
- No cambiar offsets de cabecera o tamaños de estructuras sin verificar el formato binario.
- No marcar una rutina como completa solo porque compila: debe cargar, editar, guardar y volver a abrir con el mismo resultado.

Esta fase solo genera el informe. No se han modificado archivos C# como parte de esta auditoría.
