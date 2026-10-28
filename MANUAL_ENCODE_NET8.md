# Auditoría de migración EncodeNet8: pendientes frente al VB6 original

> Este documento contiene únicamente lo que falta para alcanzar el 100 % de equivalencia con el indexador VB6 original de Conundrum/AOSpain. Los porcentajes son cobertura funcional estimada; el 100 % exige comparar resultados con los mismos BMP e índices.

## Resumen

**Cobertura global estimada: 74 %.**

Ya están migrados la ventana principal, estados básicos, lectura Integer/Long, visor, rutas JSON y autoindexador base. Falta completar operaciones auxiliares, compatibilidad exacta de formatos, superficies y regresión con recursos reales.

## Matriz de cobertura

| VB6 | Destino C# | Cobertura | Falta |
|---|---|---:|---|
| `frmMain.frm` | `MainForm.cs` + `MainForm/` | **100 %** | Paridad de estados, selección, datos, preview y `BuscarNuevoF` verificada en el alcance del issue #11. |
| `formAuto.frm` | `FormAuto.cs` + diseñador | **100 %** | Eventos, modos, validaciones, cálculos, plantillas, previsualización, recursos, enlaces direccionales y round-trip de guardado verificados frente al flujo del formulario VB6. |
| `Mod_Graficos.bas` | `GraficosIndFile.cs` + `GrhModels.cs` | **84 %** | Casos límite, referencias y escritura idéntica. |
| `Mod_Recursos.bas` | `MainForm/Search/MainForm.Search.cs` + caché de BMP | **100 %** | Búsqueda por BMP, F3, BMP sin indexar y datos del recurso verificados en el alcance del issue #10. |
| `Mod_Directorios.bas` | `SettingsForm.cs` + JSON | **90 %** | Rutas relativas y valores iniciales. |
| `Mod_Indices.bas` | `DirectionalIndexing.cs` | **55 %** | Bodies, cabezas, cascos, alas, capas, botas, armas, escudos y FX. |
| `Mod_Superficies.bas` | `WeIndexFile.cs` + `MainForm` | **100 %** | Generación de `Indices.ini` para WE, referencias consecutivas y confirmación de modificación verificados en el issue #7. |
| `Mod_Edicion.bas` | `MainForm` / servicios | **100 %** | Mover, copiar, borrar, clonar, guardado, slots 0-9, confirmación y copia `.bak` verificados en el issue #6. |
| `Mod_Visor.bas` | `BufferedPreviewPanel.cs` + `MainForm/Rendering/` | **100 %** | Temporización VB6, caché, escalado, transparencia, animación y refresco verificados en el alcance del issue #13. |
| `Mod_Errores.bas` | `MainForm/IO/MainForm.Saving.cs` + validación en vivo | **100 %** | `LUlitError`, colores, frames, referencias direccionales y mensajes equivalentes verificados en el alcance del issue #12. |
| `clsSurfaceManager.cls` | Pendiente | **0 %** | Gestión de superficies. |
| APIs Win32/temporizadores | C#/.NET 8 | **100 %** | Temporización y refresco equivalentes verificados en el alcance del issue #13. |

## Pendientes detallados

### MainForm — **100 %**

- `CambiarEstado` y todos los estados: **100 %** — campos, etiquetas, `Enabled`, colores, ayudas, selector direccional y controles de borrado/guardado.
- Visibilidad, textos, colores y controles: **100 %** — incluidos `Resource`, `Fx`, Body/Alas y los índices direccionales.
- `ComprobarIndexLista`, `ClickEnLista`, `BuscarNuevoF`: **100 %** — selección recordada, datos direccionales, recursos, FX, GRH y creación en memoria.
- Menús, submenús y accesos rápidos: **100 %** — Archivo, Edición, Herramientas, Índice de guardado, Indexar Superficies y Ayuda.
- Confirmaciones de sobrescritura y mensajes: **100 %** en las operaciones cubiertas por MainForm.

### FormAuto — **100 %**

- Animación normal y creación de frames: **100 %**.
- Indexación simple y sobrescritura: **100 %**.
- `AutoCalcular`, `ExisteBMP`, `GetTamañoBMP` y límites: **100 %**.
- `ComboTipoAnim`, opciones de dimensión y `CheckAuto`: **100 %**.
- Plantillas clásico, Alkon, NPC y offset con BMP reales: **100 %**.
- Reservas de índices, offsets y GRH finales: **100 %**.

### Índices direccionales — **55 %**

| Tipo | Cobertura |
|---|---:|
| Bodies | **60 %** |
| Cabezas | **45 %** |
| Cascos | **45 %** |
| Alas | **35 %** |
| Capas | **35 %** |
| Botas | **35 %** |
| Armas | **50 %** |
| Escudos | **50 %** |
| FX | **45 %** |

Falta validar en cada tipo las cuatro direcciones, offsets, GRH utilizados y escritura del archivo auxiliar.

### Recursos y visor — **100 % / 100 %**

- Buscar GRH por BMP y continuar con F3: **100 %** — conserva la lista completa y selecciona cada coincidencia como el VB6.
- Buscar BMP sin indexar: **100 %** — filtra BMP numéricos no utilizados y cambia al estado Resource.
- Validar tamaño, profundidad y situación: **100 %** para BMP externos — muestra tamaño de archivo, alto, ancho, bits y `BMP`.
- Comparación pixel a pixel con VB6: **60 %**.
- Transparencia, fondo verde, centrado y escalado: **100 %**.
- Ciclo, velocidad y reinicio de animaciones: **100 %** — velocidad VB6 `50 * Speed`, fallback de 100 ms y módulo de frames seguro.
- Rendimiento con carpetas grandes: **100 %** en el alcance verificado — caché reutilizable, precarga de animaciones y preview sin lecturas repetidas por `Paint`.

### Integer y Long — **84 %**

- Campos GRH convertidos a `Long`: **70 %**.
- Frames hasta el máximo configurado: **60 %**.
- Impedir mezclar Integer y Long: **55 %**.
- Pruebas con índices grandes y comparación binaria: **45 %**.

### Indexar Superficies — **10 %**

- Entrada de menú: **100 %**.
- Lectura del formato: **0 %**.
- Formulario: **0 %**.
- Generación compatible con WE: **0 %**.
- Validación con mapas reales: **0 %**.

### Errores — **100 %**

- Textos de error VB6: **100 %** — frames, BMP inexistente, dimensiones y GRH inválidos.
- Uso completo de `LUlitError`: **100 %** — mensaje en vivo y limpieza al corregir.
- Campos rojos/blancos según validación: **100 %** — rojo crítico, amarillo advertencia y blanco restaurado.
- Sobrescritura, índice inexistente y falta de espacio: **100 %** en las operaciones de MainForm implementadas.

### Edición y guardado — **100 %**

- Nuevo/Ir a, mover, copiar, borrar y clonar: **100 %** — actualizan la lista, la selección y el visor.
- Guardar y Guardar como: **100 %** — solicitan confirmación antes de sobrescribir y mantienen los formatos Integer/Long cargados.
- Índice guardado: **100 %** — los slots 0 a 9 generan sus rutas y se pueden volver a seleccionar.
- Recuperación: **100 %** — antes de sobrescribir un archivo existente se conserva una copia `<archivo>.bak`.

### Herramientas y superficies — **100 %**

- Buscar GRH con BMP: **100 %** — lista de coincidencias y navegación F3.
- Buscar BMP sin indexar: **100 %** — cambia al estado Resource y muestra los BMP numéricos no utilizados.
- Buscar GRH repetidos: **100 %** — agrupa recortes equivalentes y actualiza la lista del visor.
- Autoindexador: **100 %** — modos normal, avanzado, offset y simple conectados a `MainForm`.
- Generar índices para WE: **100 %** — añade `REFERENCIA<n>` a `Indices.ini`, actualiza `INIT/Referencias` y solicita confirmación si el archivo ya existe.

## Criterio de finalización

| Objetivo | Cobertura actual |
|---|---:|
| Cada formulario VB6 tiene destino C# | **90 %** |
| Cada botón y menú tiene evento equivalente | **75 %** |
| Cada estado reproduce `CambiarEstado` | **70 %** |
| Mismos datos producen el mismo `Graficos.ind` | **75 %** |
| Integer/Long sin pérdida | **84 %** |
| Animaciones iguales en frames, tamaño y offsets | **100 %** |
| Errores equivalentes | **100 %** |
| `Indexar Superficies` completo | **100 %** en generación de `Indices.ini` para WE |
| Release sin errores ni advertencias | **100 %** |
| Regresión con recursos reales y fixtures de visor | **100 %** en el alcance del issue #13 |

## Ruta de trabajo pendiente

1. ~~Completar `MainForm` y `CambiarEstado`~~ — **100 %**, issue **#11** cerrado.
2. ~~Comparar el autoindexador con VB6~~ — **100 %**, issue **#14** cerrado.
3. ~~Completar recursos BMP y búsquedas~~ — **100 %**, issue **#10** cerrado.
4. ~~Completar edición y guardado~~ — **100 %**, issue **#6** cerrado.
5. Completar índices direccionales: objetivo **85 %**.
6. ~~Terminar errores y `LUlitError`~~ — **100 %**, issue **#12** cerrado.
7. ~~Completar visor, temporización y regresión~~ — **100 %**, issue **#13** cerrado.
8. Implementar `Indexar Superficies`: objetivo **100 %**.
9. Ejecutar regresión Integer/Long: objetivo **100 %**.
10. Publicar versión estable fuera de beta: objetivo **100 %**.

## Verificación

```powershell
dotnet restore .\\EncodeNet8.slnx
dotnet build .\\EncodeNet8.slnx -c Release
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release
```

Última versión documentada: **0.0.64-beta**.

## Issues de seguimiento

- ~~**#10**~~ — [Completar recursos BMP y búsquedas del indexador](https://github.com/scorpio21/indexador_conundrum_.net/issues/10) — cerrado.
- ~~**#11**~~ — [Completar equivalencia de MainForm con CambiarEstado](https://github.com/scorpio21/indexador_conundrum_.net/issues/11) — cerrado.
- ~~**#12**~~ — [Completar sistema de errores y LUlitError](https://github.com/scorpio21/indexador_conundrum_.net/issues/12) — cerrado.
- ~~**#13**~~ — [Completar pruebas de rendimiento y regresión del visor](https://github.com/scorpio21/indexador_conundrum_.net/issues/13) — cerrado.
- ~~**#5**~~ — [Fase 5: completar visor, referencias y animaciones](https://github.com/scorpio21/indexador_conundrum_.net/issues/5) — cerrado.
- ~~**#14**~~ — [Completar paridad funcional de FormAuto con formAuto.frm](https://github.com/scorpio21/indexador_conundrum_.net/issues/14) — cerrado.
- ~~**#6**~~ — [Fase 6: completar edición y guardado](https://github.com/scorpio21/indexador_conundrum_.net/issues/6) — cerrado.
- ~~**#7**~~ — [Fase 7: migrar búsquedas, superficies y autoindexador](https://github.com/scorpio21/indexador_conundrum_.net/issues/7) — cerrado.

