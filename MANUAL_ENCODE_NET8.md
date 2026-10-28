# Auditoría de migración EncodeNet8: pendientes frente al VB6 original

> Este documento contiene únicamente lo que falta para alcanzar el 100 % de equivalencia con el indexador VB6 original de Conundrum/AOSpain. Los porcentajes son cobertura funcional estimada; el 100 % exige comparar resultados con los mismos BMP e índices.

## Resumen

**Cobertura global estimada: 74 %.**

Ya están migrados la ventana principal, estados básicos, lectura Integer/Long, visor, rutas JSON y autoindexador base. Falta completar operaciones auxiliares, compatibilidad exacta de formatos, superficies y regresión con recursos reales.

## Matriz de cobertura

| VB6 | Destino C# | Cobertura | Falta |
|---|---|---:|---|
| `frmMain.frm` | `MainForm.cs` + `MainForm/` | **82 %** | Estados, visibilidad, colores, menús y eventos. |
| `formAuto.frm` | `FormAuto.cs` + diseñador | **84 %** | Validación, creación, modos y previsualización temporal implementados; falta cerrar la comparación exhaustiva y la regresión con recursos reales. |
| `Mod_Graficos.bas` | `GraficosIndFile.cs` + `GrhModels.cs` | **84 %** | Casos límite, referencias y escritura idéntica. |
| `Mod_Recursos.bas` | `ResourceIndexing.cs` | **65 %** | BMP no indexados, búsqueda y mensajes. |
| `Mod_Directorios.bas` | `SettingsForm.cs` + JSON | **90 %** | Rutas relativas y valores iniciales. |
| `Mod_Indices.bas` | `DirectionalIndexing.cs` | **55 %** | Bodies, cabezas, cascos, alas, capas, botas, armas, escudos y FX. |
| `Mod_Superficies.bas` | Pendiente | **10 %** | Formulario, lectura y generación para WE. |
| `Mod_Edicion.bas` | `MainForm` / servicios | **65 %** | Mover, copiar, borrar, clonar e índice guardado. |
| `Mod_Visor.bas` | `BufferedPreviewPanel.cs` | **75 %** | Escalado, transparencia, animación y refresco. |
| `Mod_Errores.bas` | Validación de formularios | **68 %** | `LUlitError`, colores y mensajes. |
| `clsSurfaceManager.cls` | Pendiente | **0 %** | Gestión de superficies. |
| APIs Win32/temporizadores | C#/.NET 8 | **70 %** | Temporización, refresco y selección de archivos. |

## Pendientes detallados

### MainForm — **82 %**

- `CambiarEstado` y todos los estados: **70 %**.
- Visibilidad, textos, colores y controles: **60 %**.
- `ComprobarIndexLista`, `ClickEnLista`, `BuscarNuevoF`: **65 %**.
- Menús, submenús y accesos rápidos: **55 %**.
- Confirmaciones de sobrescritura y mensajes: **50 %**.

### FormAuto — **78 %**

- Animación normal y creación de frames: **75 %**.
- Indexación simple y sobrescritura: **75 %**.
- `AutoCalcular`, `ExisteBMP`, `GetTamañoBMP` y límites: **65 %**.
- `ComboTipoAnim`, opciones de dimensión y `CheckAuto`: **60 %**.
- Plantillas clásico, Alkon, NPC y offset con BMP reales: **55 %**.
- Reservas de índices, offsets y GRH finales: **45 %**.

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

### Recursos y visor — **65 % / 75 %**

- Buscar GRH por BMP y continuar con F3: **55 %**.
- Buscar BMP sin indexar: **45 %**.
- Validar tamaño, profundidad y situación: **60 %**.
- Comparación pixel a pixel con VB6: **60 %**.
- Transparencia, fondo verde, centrado y escalado: **55 %**.
- Ciclo, velocidad y reinicio de animaciones: **50 %**.
- Rendimiento con carpetas grandes: **40 %**.

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

### Errores — **68 %**

- Textos de error VB6: **60 %**.
- Uso completo de `LUlitError`: **55 %**.
- Campos rojos/blancos según validación: **50 %**.
- Sobrescritura, índice inexistente y falta de espacio: **45 %**.

## Criterio de finalización

| Objetivo | Cobertura actual |
|---|---:|
| Cada formulario VB6 tiene destino C# | **90 %** |
| Cada botón y menú tiene evento equivalente | **75 %** |
| Cada estado reproduce `CambiarEstado` | **70 %** |
| Mismos datos producen el mismo `Graficos.ind` | **75 %** |
| Integer/Long sin pérdida | **84 %** |
| Animaciones iguales en frames, tamaño y offsets | **60 %** |
| Errores equivalentes | **68 %** |
| `Indexar Superficies` completo | **10 %** |
| Release sin errores ni advertencias | **100 %** |
| Regresión con recursos reales | **35 %** |

## Ruta de trabajo pendiente

1. Completar `MainForm` y `CambiarEstado`: objetivo **90 %**.
2. Comparar el autoindexador con VB6: objetivo **90 %**.
3. Completar índices direccionales: objetivo **85 %**.
4. Terminar recursos, visor y errores: objetivo **90 %**.
5. Implementar `Indexar Superficies`: objetivo **100 %**.
6. Ejecutar regresión Integer/Long: objetivo **100 %**.
7. Publicar versión estable fuera de beta: objetivo **100 %**.

## Verificación

```powershell
dotnet restore .\\EncodeNet8.slnx
dotnet build .\\EncodeNet8.slnx -c Release
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release
```

Última versión documentada: **0.0.47-beta**.

## Issues de seguimiento

- **#10** — [Completar recursos BMP y búsquedas del indexador](https://github.com/scorpio21/indexador_conundrum_.net/issues/10).
- **#11** — [Completar equivalencia de MainForm con CambiarEstado](https://github.com/scorpio21/indexador_conundrum_.net/issues/11).
- **#12** — [Completar sistema de errores y LUlitError](https://github.com/scorpio21/indexador_conundrum_.net/issues/12).
- **#13** — [Completar pruebas de rendimiento y regresión del visor](https://github.com/scorpio21/indexador_conundrum_.net/issues/13).
- **#14** — [Completar paridad funcional de FormAuto con formAuto.frm](https://github.com/scorpio21/indexador_conundrum_.net/issues/14).

