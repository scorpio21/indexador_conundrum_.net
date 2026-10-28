# Changelog

## [0.0.53-beta] - 2026-10-28

### Integración y guardado — Issue #14

- Se verificó la generación automática del enlace Body con el orden direccional VB6 `{24, 26, 23, 25}`.
- Se verificó el guardado y la recarga de los 26 GRH generados en formato Long y del índice direccional `Personajes.ind` en formato Integer.
- La prueba WinForms ahora cubre generación, integración de enlace y round-trip de archivos.
- Build Release y pruebas verificadas: 0 errores y 0 advertencias.

## [0.0.52-beta] - 2026-10-28

### Pruebas — Issue #14

- Se añadió una prueba WinForms en STA para ejecutar realmente los tres modos del `FormAuto`.
- Se verifican la animación clásica de 26 GRH, las posiciones VB6 `X=25`/`Y=45`, los grupos direccionales 6/6/5/5, el modo offset y la indexación simple.
- Build Release y la suite de pruebas verificadas: 0 errores y 0 advertencias.

## [0.0.51-beta] - 2026-10-28

### Corrección — Issue #14

- Se conserva el texto de desplazamiento VB6 `-38º0` al seleccionar la plantilla `(pj) clásico`, manteniendo también el valor Y en cero.
- Build Release y pruebas existentes verificadas: 0 errores y 0 advertencias.

## [0.0.50-beta] - 2026-10-28

### Avance — Issue #14

- Se añadió la previsualización en tiempo real del Autoindexador sobre el visor principal.
- El visor muestra el BMP de trabajo y delimita en rojo los frames temporales mientras se editan los parámetros.
- Se fuerza el repintado del visor durante el diálogo modal, equivalente al comportamiento de `Dibujado` del VB6.
- La previsualización no modifica `Graficos.ind` hasta pulsar `Crear`.
- Build Release y pruebas existentes verificadas: 0 errores y 0 advertencias.

## [0.0.49-beta] - 2026-10-28

### Avance — Issue #14

- Se igualaron las plantillas direccionales `(pj) clásico` y `(pj) Alkon` con las posiciones X/Y del `formAuto.frm` original.
- Los frames base de esas dos plantillas conservan la referencia VB6 `1..22`; NPC y offset mantienen sus referencias generadas y sus cuadrículas correspondientes.
- Se mantiene la creación del GRH final y la conexión con el índice direccional auxiliar de Body/Arma/Escudo/Alas/Espalda/Botas.
- Build Release y pruebas existentes verificadas: 0 errores y 0 advertencias.

## [0.0.48-beta] - 2026-10-28

### Avance — Issue #14

- Se añadió validación en tiempo real de los campos de animación normal, avanzada y simple.
- Se igualaron los colores de error y advertencia para BMP, primer índice y dimensiones.
- Se completaron las opciones `Body`, `Arma`, `Escudo`, `Alas`, `Espalda` y `Botas`.
- Se corrigió la búsqueda de huecos para devolver `0` cuando no existe espacio disponible.
- El autoindexador avanzado genera las cuatro animaciones direccionales reservadas por VB6.
- Se conectó el índice direccional generado con `MainForm` y sus registros auxiliares.
- Release compilada correctamente: 0 errores y 0 advertencias.

Todos los cambios relevantes de este proyecto se documentan aquí.

## [0.0.47-beta] - 2026-10-28

### En progreso — Issue #7

- Se creó `MANUAL_ENCODE_NET8.md`, basado en el manual original de Conundrum y adaptado al flujo actual de EncodeNet8.
- Se documentaron los menús, estados, visor, validaciones, formatos Integer/Long y los tres modos del autoindexador.
- Se conectó `FormAuto` como único autoindexador activo y se retiró `AutoIndexerForm`.
- Se adaptaron los botones Buscar, AutoCalcular, Crear e Indexar de los tres modos al flujo VB6.
- Se documentaron las funcionalidades implantadas y las pendientes de la migración.
- Se corrigió la incompatibilidad del diseñador causada por lambdas dentro de `MainForm.Designer.cs`.
- Build Release correcta con 0 errores y 0 advertencias.

## [0.0.46-beta] - 2026-10-27

### En progreso — Issue #7

- Se ajustaron valores iniciales, colores de validación y fuente al comportamiento visible de `FormAuto.frm`.
- Se añadieron los controles de autoindexación avanzada: tipo, selector Body/Armas/Escudo, índice automático y offset.
- Se añadieron los valores iniciales de dimensiones y BMP observados en el original.
- Build Release correcta con 0 errores y 0 advertencias.

## [0.0.45-beta] - 2026-10-27

### En progreso — Issue #7

- Se rehízo `AutoIndexerForm.Designer.cs` con el patrón clásico de Visual Studio.
- Cada control es un campo, cada propiedad se asigna por separado y cada contenedor usa `Controls.Add` explícito.
- Se alinearon los controles principales con las posiciones de `FormAuto.frm`.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.44-beta] - 2026-10-27

### En progreso — Issue #7

- Los controles del autoindexador se agregan individualmente al contenedor, sin `AddRange` ni controles anónimos.
- Se mejora la compatibilidad del árbol de controles con el diseñador visual de Visual Studio.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.43-beta] - 2026-10-27

### En progreso — Issue #7

- Se sustituyó la inicialización dinámica del autoindexador por inicialización directa de controles en `InitializeComponent`.
- Se eliminaron llamadas auxiliares y lambdas del archivo del diseñador para mejorar la compatibilidad con Visual Studio.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.42-beta] - 2026-10-27

### En progreso — Issue #7

- `AutoIndexerForm.Designer.cs` ya no usa métodos auxiliares ni lambdas dentro de `InitializeComponent`.
- Los controles se inicializan directamente, siguiendo el código que genera Visual Studio.
- Se corrige el error del diseñador en la línea 24.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.41-beta] - 2026-10-26

### En progreso — Issue #7

- El diseñador del autoindexador se rehízo tomando como referencia `FormAuto.frm`.
- Se añadieron los tres modos visibles del original: `Animacion normal`, `Animacion Avanzada` e `Indexacion simple`.
- Se incorporaron los campos, radios, botones y grupos principales del formulario VB6.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.40-beta] - 2026-10-26

### En progreso — Issue #7

- `AutoIndexerOptions` se movió a un archivo independiente.
- `AutoIndexerForm.cs` contiene ahora como primera clase el formulario diseñable.
- Se corrige el error del diseñador de Visual Studio sobre la primera clase del archivo.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.39-beta] - 2026-10-26

### En progreso — Issue #7

- Se añadió metadata `SubType=Form` y la relación `AutoIndexerForm.Designer.cs` en el proyecto.
- Visual Studio identifica ahora el autoindexador como formulario WinForms y habilita `Ver diseñador`.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.38-beta] - 2026-10-26

### En progreso — Issue #7

- `AutoIndexerForm` queda separado en `AutoIndexerForm.cs` y `AutoIndexerForm.Designer.cs`.
- El formulario ahora aparece como formulario WinForms editable desde el diseñador de Visual Studio.
- Se mantiene la lógica de selección de BMP y creación de GRH/animación.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.37-beta] - 2026-10-26

### En progreso — Issue #7

- El menú `Herramientas > Autoindexador` ya abre un formulario WinForms editable.
- Se puede seleccionar un BMP, indicar cuadrícula, primer GRH, tamaño de celda y velocidad.
- Se generan GRH individuales y, opcionalmente, una animación con esos frames en memoria.
- Se validan dimensiones, máximo de GRH y colisiones antes de agregar datos.
- Se añadió una prueba de generación de cuadrícula y animación.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.36-beta] - 2026-10-26

### En progreso — Issue #7

- `Buscar Bmps sin indexar (Cry)` muestra los BMP encontrados directamente en el visor.
- La selección de un recurso sin GRH indexado ya no se interpreta como un GRH inexistente.
- `F3` continúa recorriendo los BMP sin indexar y se informa el estado mediante `LUlitError`.
- Se añadió protección para BMPs fuera del rango del formato Integer.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.35-beta] - 2026-10-26

### En progreso — Issue #7

- `Buscar Grh con bmp...` solicita el número de BMP, como el VB6 original.
- Se muestran los GRH que usan ese BMP y se activa el flujo de búsqueda siguiente con `F3`.
- Los BMP sin coincidencias muestran el mensaje en `LUlitError` sin abrir diálogos innecesarios.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.34-beta] - 2026-10-26

### En progreso — Issue #6

- `f. verde` alterna el fondo verde del visor y el fondo normal.
- `Edición > Color de fondo...` permite elegir un color personalizado mediante `ColorDialog`.
- El cambio solo afecta a la previsualización y no modifica los datos del índice.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.33-beta] - 2026-10-26

### En progreso — Issue #6

- El botón de guardado actualiza su texto según el estado activo.
- Se muestran nombres y extensiones coherentes: `Graficos.ind`, `Personajes.ind`, `Alas.ind`, `Fxs.ind`, `Armas.dat`, etc.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.32-beta] - 2026-10-26

### En progreso — Issue #6

- Los slots `Índice de guardado` se amplían a índices auxiliares.
- Se conservan las extensiones: `Cabezas1.ind`, `Armas1.dat`, `Fxs1.ind`, etc.
- Se confirma la escritura y se actualizan las rutas activas tras guardar la copia.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.31-beta] - 2026-10-26

### En progreso — Issue #6

- `Archivo > Cargar` ahora respeta el estado seleccionado.
- Carga `Graficos.ind`, índices auxiliares binarios, archivos INI y FX con sus lectores correspondientes.
- Los filtros del diálogo se adaptan a `.ind` o `.dat` y se actualizan las listas, referencias y rutas activas.
- `Resource` permanece protegido porque no es un índice editable.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.30-beta] - 2026-10-26

### En progreso — Issue #6

- Se añaden al formulario los botones `Alas` y `Botas`.
- Ambos usan el manejador común de cambio de estado y cargan sus índices existentes.
- Se recoloca `Fx` debajo de los nuevos botones para evitar solapamientos.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.29-beta] - 2026-10-26

### En progreso — Issue #6

- `Archivo > Guardar...` ya permite guardar los índices auxiliares.
- Se usa formato `.ind` para índices binarios y `.dat` para Armas/Escudos y estados cargados desde INI.
- Se actualizan las rutas activas y las referencias después de guardar.
- `Resource` permanece protegido porque no representa un índice editable.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.28-beta] - 2026-10-26

### En progreso — Issue #6

- Se implementan los slots `Índice de guardado` para `Graficos.ind`.
- El slot 0 conserva el guardado normal y los slots 1–9 crean copias numeradas junto al índice cargado.
- Se confirma la escritura antes de sobrescribir o crear una copia.
- Los slots numerados de índices auxiliares quedan pendientes de sus diálogos Guardar como específicos.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.27-beta] - 2026-10-26

### En progreso — Issue #6

- `Nuevo/Ir A` permite seleccionar un GRH existente, buscar el primer hueco libre usando `0` y crear un GRH nuevo en memoria.
- Los GRH nuevos empiezan con un frame propio, BMP 1 y tamaño 32×32 para poder completar sus datos desde el formulario.
- La lista de GRH se refresca ordenada por identificador y la selección se sitúa automáticamente en el destino.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.26-beta] - 2026-10-26

### En progreso — Issue #6

- `Mover`, `Copiar` y `Clonar...` se amplían a Bodies, Cabezas, Cascos, Alas, Capas, Botas, Armas, Escudos y FX.
- Los registros direccionales conservan sus cuatro GRH y offsets X/Y.
- Bodies mantiene sincronizada su representación de compatibilidad con el índice direccional.
- Se mantienen validaciones de límites, confirmación de sobrescritura y protección de Resource.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.25-beta] - 2026-10-26

### En progreso — Issue #6

- `Suprimir` ahora funciona para GRH, Bodies, Cabezas, Cascos, Alas, Capas, Botas, Armas, Escudos y FX cuando el lector/escritor del estado está disponible.
- Se confirma la eliminación, se actualiza la lista, se detiene el visor y se reconstruyen las referencias.
- Los huecos se conservan para que los escritores binarios/INI los persistan como registros vacíos, igual que el VB6.
- Los recursos quedan protegidos porque no son registros editables del índice.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.24-beta] - 2026-10-26

### En progreso — Issue #6

- Se implementan `Mover`, `Copiar` y `Clonar...` para registros GRH.
- Se validan los límites del índice y se confirma antes de sobrescribir un GRH existente.
- El destino se selecciona automáticamente y se conserva la edición en memoria hasta guardar.
- Los estados auxiliares permanecen protegidos hasta disponer de sus escritores específicos.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.23-beta] - 2026-10-26

### En progreso — Issue #5

- Se adapta la validación de `GrhCorrectoNormal` del VB6.
- El campo `Frames:` equivalente a `TextDatos(1)` se marca en rojo cuando falta un frame, es inválido o no tiene BMP.
- Se marca en amarillo cuando los frames tienen dimensiones diferentes.
- También se colorean `BMP`, `Alto`, `Ancho`, `Posición X` y `Posición Y` según el tipo de error.
- Los mensajes detallados se muestran en `LUlitError`.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.22-beta] - 2026-10-26

### En progreso — Issue #5

- Se adapta el label VB6 `LUlitError` como un label inferior editable desde el diseñador WinForms.
- Se limpia al cambiar de estado, seleccionar otro índice o vaciar la memoria.
- Muestra errores persistentes de frames, BMP ausentes, dimensiones inválidas y recortes fuera de rango sin depender de un `MessageBox`.
- Las búsquedas muestran en ese mismo área los mensajes equivalentes a “F3 para continuar la búsqueda” y “Se terminó la búsqueda”.
- Build Release y pruebas pasan con 0 errores y 0 advertencias.

## [0.0.21-beta] - 2026-10-26

### En progreso — Issue #5

- El intervalo del visor se ajusta a la lógica VB6: un ciclo de render de aproximadamente 60 FPS por unidad de `Speed`.
- Se corrige la conversión anterior de `Speed × 25 ms`, que hacía las animaciones más lentas y menos fluidas que el cliente original.
- El loop continúa reiniciando en el primer frame al superar el último.
- Issue #5 sigue abierto para validar fluidez visual con recursos reales.

## [0.0.20-beta] - 2026-10-26

### En progreso — Issue #5

- La ficha del GRH muestra la lista completa de frames en el campo `Frames`.
- `Número Frames` muestra ahora la cantidad de frames, como en el indexador VB6.
- Para animaciones se muestran las dimensiones y los títulos del primer frame; el GRH raíz conserva su BMP, offsets y velocidad.
- El timer deja de sobrescribir la lista de frames con el número del frame actual.
- Issue #5 sigue abierto para validar visualmente errores de recursos ausentes.

## [0.0.19-beta] - 2026-10-26

### En progreso — Issue #5

- La lista de `Graficos.ind` marca los GRH animados con `[animación]`, igual que el indexador VB6.
- Las búsquedas conservan esa etiqueta sin perder la resolución del ID real al seleccionar un elemento.
- Issue #5 sigue abierto para validar fluidez con recursos reales y completar pruebas visuales automatizadas.

## [0.0.18-beta] - 2026-10-26

### En progreso — Issue #5

- Los frames de una animación se precargan al seleccionarla para evitar tirones en el primer ciclo.
- El avance usa `Stopwatch` y un timer de 15 ms para corregir retrasos acumulados del timer de WinForms.
- El ritmo almacenado del GRH se conserva como duración de cada frame.
- Issue #5 sigue abierto para validar fluidez con recursos reales y completar pruebas visuales automatizadas.

## [0.0.17-beta] - 2026-10-26

### En progreso — Issue #5

- El visor reutiliza los BMP ya cargados en memoria en lugar de abrirlos y aplicar transparencia en cada frame.
- El panel de previsualización usa doble buffer para reducir parpadeos y tirones durante la animación.
- La caché se libera al cerrar el programa o al cambiar las rutas de configuración.
- Issue #5 sigue abierto para validar fluidez con los recursos reales y completar pruebas visuales automatizadas.

## [0.0.16-beta] - 2026-10-26

### En progreso — Issue #5

- El contador de referencias se reconstruye con todos los índices auxiliares disponibles: Bodies, cabezas, cascos, alas, capas, botas, armas, escudos y FX.
- La reconstrucción se ejecuta al cargar o cambiar `Graficos.ind` y después de guardar un índice auxiliar.
- Los índices auxiliares inválidos se omiten individualmente para que no bloqueen el visor.
- Issue #5 sigue abierto para completar las pruebas visuales automatizadas y la cobertura de recursos no indexados.

## [0.0.15-beta] - 2026-10-26

### En progreso — Issue #5

- El visor calcula las referencias cargadas del GRH a partir de animaciones, índices direccionales y FX.
- La cifra se actualiza al seleccionar un GRH, una dirección o un FX, sustituyendo los textos fijos anteriores.
- Issue #5 sigue abierto para completar la cobertura de referencias entre todos los índices y las pruebas visuales.

## [0.0.14-beta] - 2026-10-26

### En progreso — Issue #5

- El timer del visor usa la velocidad almacenada en el GRH para calcular el intervalo de animación.
- El render GDI+ aplica transparencia negra para aproximar la clave de color del cliente original.
- Se mantienen validaciones explícitas para frames inexistentes, dimensiones inválidas y recortes fuera del BMP.
- Issue #5 sigue abierto para referencias y pruebas visuales completas.

### En progreso — Issue #5

- Se añadió al diseñador el selector de dirección Subir, Derecha, Abajo e Izquierda.
- El visor resuelve el GRH de la dirección seleccionada para Bodies, Cabezas, Cascos, Capas, Alas y Botas.
- El selector se integra con la edición y el guardado de índices direccionales.
- Issue #5 sigue abierto para completar referencias, velocidad y pruebas visuales de todos los estados.

### Completado — Issues #3 y #4

- Se añadieron escritores para índices direccionales `.ind`, FX `.ind` y equipamiento `.dat`.
- El guardado conserva la cabecera binaria cuando existe y vuelve a cargar los datos guardados sin pérdida de formato Integer/Long.
- Se añadieron pruebas de ida y vuelta para registros direccionales, FX y Botas INI.
- Bodies, Cabezas, Cascos, Capas, Alas y Botas quedan cubiertos; los tres últimos usan fixtures porque no están presentes en la copia actual del cliente.
- FX, Armas y Escudos quedan cubiertos con lectores/escritores y validación de archivos reales.
- Issues cerrados: [#3](https://github.com/scorpio21/indexador_conundrum_.net/issues/3) y [#4](https://github.com/scorpio21/indexador_conundrum_.net/issues/4).

### Corrección — Issue #6

- El guardado queda bloqueado fuera del estado Gráficos para evitar modificar `Graficos.ind` accidentalmente desde Bodies, Cabezas, FX u otros estados.
- La interfaz actualiza el estado habilitado del botón Guardar al cambiar de sección.
- Issue #6 continúa abierto para los escritores específicos y operaciones de edición restantes.

### En progreso — Issue #3

- Capas, Alas y Botas aceptan ahora tanto `.ind` binario como `.dat` INI cuando el cliente los proporciona.
- El lector INI direccional soporta `ALAS1`, `Capa1`, `Bota1` y sus cuatro direcciones.
- Se añadieron pruebas sintéticas para Botas y los formatos INI direccionales.
- El Issue #3 sigue abierto hasta completar edición y guardado de estos índices.

### En progreso — Issue #7

- Se conectaron las búsquedas de GRH con BMP, BMP sin indexar, siguiente resultado y GRH repetidos.
- Las búsquedas mantienen IDs reales, por lo que seleccionar o borrar un resultado filtrado no usa una posición incorrecta.
- El autoindexador y la generación para WE siguen pendientes de sus módulos específicos.
- Issue todavía abierto: [#7](https://github.com/scorpio21/indexador_conundrum_.net/issues/7).

### En progreso — Issue #6

- `Guardar` utiliza el archivo actualmente cargado y solicita confirmación antes de sobrescribirlo.
- `Guardar...` permite elegir un destino distinto.
- El guardado aplica los datos editados del GRH seleccionado y mantiene el formato Integer/Long elegido.
- El Issue #6 sigue abierto hasta completar edición, clonado, mover, slots y guardado de todos los estados.
- Issue relacionado: [#6](https://github.com/scorpio21/indexador_conundrum_.net/issues/6).

### En progreso — Issue #5

- El visor GDI+ resuelve los frames animados y muestra errores explícitos cuando falta un GRH.
- Los recortes con dimensiones inválidas o fuera de los límites del BMP ya no se sustituyen silenciosamente por la imagen completa.
- El timer detiene animaciones sin frames válidos.
- Issue todavía abierto: [#5](https://github.com/scorpio21/indexador_conundrum_.net/issues/5).

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
