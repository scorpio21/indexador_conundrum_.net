# Changelog

Todos los cambios relevantes de este proyecto se documentan aquí.

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
