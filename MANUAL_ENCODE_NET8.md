# Auditoría de migración EncodeNet8 frente al VB6 original

Versión documentada: **0.0.81-beta**<br>
Fecha de auditoría: **2026-10-30**

## Veredicto

La migración **no está al 100 %**. El proyecto .NET reproduce el flujo principal del indexador: `Graficos.ind`, Integer/Long, estados, edición, visor GDI+, autoindexador y `Indices.ini` para WE.

No puede declararse equivalencia total porque ResF está desactivado, no se ha reproducido `Inicio.con` y el motor DirectX7/`clsSurfaceManager` fue sustituido por GDI+.

Los porcentajes son cobertura funcional estimada frente a `K:\AO_Aospain\utilidades\Code`, no porcentaje de líneas.

## Inventario

Se revisaron 15 archivos VB6:

- Formularios: `Encode.frm`, `FormAuto.frm`, `FrmConfig.frm`, `FrmSearch.frm`.
- Módulos: `APIdeclaraciones.bas`, `Declares.bas`, `DX_InIt.bas`, `GameIni.bas`, `GeneralES.bas`, `Indices.bas`, `TileEngine.bas`.
- Clases: `clsIniReader.cls`, `clsSurfaceManager.cls`, `clsSurfaceManDyn.cls`, `clsSurfaceManStatic.cls`.

## Matriz de cobertura

| Componente VB6 | Destino .NET | Estado | Cobertura | Pendiente |
|---|---|---|---:|---|
| `Encode.frm` | `MainForm.cs` + `MainForm/` + `EncodeMapsForm` | Parcial alto | **90 %** | Herramientas legacy restantes y paridad exacta de menús. |
| `FormAuto.frm` | `FormAuto.cs` + diseñador | Parcial alto | **90 %** | Validación exhaustiva con recursos reales. |
| `FrmConfig.frm` | `SettingsForm.cs` + JSON | Parcial | **75 %** | Equivalencia completa con Conf.ini e Inicio.con. |
| `FrmSearch.frm` | `FrmSearch.cs` + `FrmSearch.Designer.cs` + `MainForm/Search` | Implementado | **100 %** | Búsquedas, navegación y menú contextual conectados a `FormAuto`. |
| `GameIni.bas` | `IndexerSettings` + carga auxiliar | Parcial | **55 %** | Inicio.con, cabecera binaria, Tips y recursos auxiliares. Issue #18. |
| `GeneralES.bas` | Lectores direccionales | Parcial alto | **80 %** | Variantes legacy reales. Issue #18. |
| `Indices.bas` | Lectores/escritores y MainForm | Parcial alto | **85 %** | Casos completos de formatos y compatibilidad cruzada. |
| Superficies WE | `WeIndexFile` + MainForm | Implementado | **90 %** | Validación contra todos los mapas y WE real. |
| Edición/guardado | `MainForm/Editing` + `Saving` | Implementado | **90 %** | Comparación exhaustiva con todos los casos VB6. |
| Recursos BMP/ResF | `MainForm/Search` | Parcial | **65 %** | BMP funciona; ResF está pendiente. Issue #17. |
| Visor/TileEngine | GDI+ `Rendering` | Sustitución | **70 %** | No es el motor DirectDraw7 original. |
| `DX_InIt.bas` | Sin equivalente DirectX7 | No migrado | **0 %** | No se porta DirectX7 automáticamente. |
| `clsIniReader.cls` | Lectores INI específicos | Parcial | **60 %** | Parser y semántica legacy completa. Issue #18. |
| `clsSurfaceManager*.cls` | Sin equivalente | Fuera del alcance actual | **0 %** | Superficies runtime del cliente. |
| APIs Win32/temporizadores | .NET/GDI+/WinForms | Sustitución parcial | **70 %** | Equivalencia exacta del runtime DirectX. |

## Funciones implementadas

- Lectura y guardado de `Graficos.ind` Integer y Long.
- Validación de cabeceras, registros, frames y truncados.
- Estados Gráficos, Bodies, Cabezas, Cascos, Alas, Capas, Botas, Armas, Escudos, FX y Resource.
- Selector de Bodies: Todos, Norte, Este, Sur y Oeste.
- Selector de índices direccionales: `Todos` dibuja las cuatro direcciones en Cabezas, Cascos, Escudos, Armas, Alas, Capas y Botas; las opciones individuales dibujan solo la dirección seleccionada.
- `Checkcabeza`, offset de cabeza y edición de datos.
- Mover, copiar, borrar, clonar y slots de guardado.
- Autoindexador normal, avanzado, offset y simple.
- `AutoCalcular`, búsqueda de índices libres y límites.
- Preview GDI+ de BMP, recortes y animaciones básicas.
- Generación de `Indices.ini` para WE.
- Configuración JSON de Init, gráficos y Datos.
- Restauración de Gráficos al cerrar el Autoindexador.
- Ventana independiente `FrmSearch` para GRH repetidos y BMP sin indexar.
- Doble clic en resultados para navegar al recurso o GRH correspondiente.
- Menú contextual para preparar `FormAuto` como Body, animación o gráfico independiente.
- Ventana `Encode Mapas (FK.ind)` desde Herramientas y botón Encode Mapas.
- Lectura de `init\encode\mapas.dat` y generación binaria compatible de `FK.ind`.

## Diferencias pendientes

### FrmSearch — Issue #15 — completado

La ventana `FrmSearch` ya existe en WinForms y es editable desde el diseñador. Reproduce las dos búsquedas de VB6, muestra el contador, permite seleccionar resultados y ofrece el menú contextual de indexación como Body, animación o gráfico independiente.

### EncodeMapas y FK.ind — Issue #16 — completado

El VB6 lee `INIT\\encode\\mapas.dat`, procesa `NumMaps` y `Lluvia`, y genera `FK.ind`. .NET ahora ofrece una ventana editable para indicar ambos archivos desde Herramientas > Encode Mapas (FK.ind), además del botón lateral.

El fichero generado conserva el formato binario del original: cabecera de 263 bytes, contador `Integer` de mapas y un byte `Lluvia` por mapa. Si ya existe un `FK.ind`, se conserva su cabecera; si no, se crea una cabecera compatible. Los errores se muestran dentro de la ventana y no interrumpen el formulario principal.

### ResF — Issue #17

El original contempla `ResourceFile=1`, `2` y `3`. .NET conserva `ResourceMode`, pero solo procesa BMP. ResF queda pendiente hasta definir el compresor y formato oficial de Aospain.

### Compatibilidad legacy — Issue #18

Falta demostrar equivalencia completa para:

- `Inicio.con`.
- `Tips.ayu` y datos auxiliares.
- Variantes `Graficos*.dat`.
- Variantes de slots `Armas*.dat`, `Escudos*.dat` y otros archivos.
- Semántica completa de `clsIniReader`.

La configuración JSON es una sustitución intencionada, pero no equivale automáticamente a todos los archivos legacy.

### Motor DirectX7

El original usa `DX_InIt.bas`, `TileEngine.bas` y `clsSurfaceManager`. .NET utiliza GDI+ para el visor. El issue #9 documenta esta sustitución controlada. Si se exige equivalencia pixel a pixel y rendimiento idéntico, habrá que abrir una fase específica de backend gráfico.

## Pruebas actuales

- Build Release: correcto, 0 advertencias y 0 errores.
- Integer/Long: cubierto con fixtures.
- Lectores direccionales: cubiertos parcialmente.
- Autoindexador: cubierto en modos principales.
- Estados y controles: cubiertos parcialmente.
- `Indices.ini`: generación básica cubierta.
- Selector `Todos` y render compuesto de índices direccionales: compilado y verificado en la ruta de renderizado.

Falta una batería completa con archivos reales del cliente y comparación directa de resultados entre VB6 y .NET.

## Ruta de trabajo

1. **#18** — Completar formatos legacy, `Inicio.con`, Tips y `clsIniReader`.
2. **#8** — Ampliar regresión con archivos reales.
3. **#17** — Implementar ResF cuando exista el compresor oficial.
4. **#9** — Mantener GDI+ documentado o abrir una fase de backend gráfico si se exige paridad exacta.

## Verificación

```powershell
dotnet restore .\\EncodeNet8.slnx
dotnet build .\\EncodeNet8.slnx -c Release
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release
```

Última verificación:

- Compilación correcta.
- 0 errores.
- 0 advertencias.
- Pruebas automáticas actuales correctas.

## Configuración

`IndicesPath` mantiene separada la ruta de `Indices.ini`, con `Datos` como valor predeterminado. El campo legacy `recurso` significa:

- `1`: BMP.
- `2`: ResF.
- `3`: BMP o ResF.

En la versión actual, los modos 2 y 3 requieren completar el Issue #17.

## Issues nuevos

- [#15 — Completar FrmSearch](https://github.com/scorpio21/indexador_conundrum_.net/issues/15)
- [#16 — Implementar EncodeMapas y FK.ind — completado](https://github.com/scorpio21/indexador_conundrum_.net/issues/16)
- [#17 — Implementar lector ResF](https://github.com/scorpio21/indexador_conundrum_.net/issues/17)
- [#18 — Compatibilidad legacy](https://github.com/scorpio21/indexador_conundrum_.net/issues/18)

## Conclusión

EncodeNet8 es utilizable para el flujo principal, pero **todavía no es un clon 100 % equivalente del VB6**. Esa afirmación solo podrá hacerse al cerrar los issues pendientes y validar los resultados con recursos reales.

