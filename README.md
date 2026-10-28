# EncodeNet8 — v0.0.54-beta

Migración progresiva del indexador de gráficos de Conundrum/AOSpain desde Visual Basic 6 a C# .NET 8 WinForms.

El objetivo es conservar el comportamiento del indexador original y mantener los formularios editables desde Visual Studio. La migración se realiza por fases y no convierte automáticamente el motor DirectX 7: el visor actual utiliza GDI+ como sustitución controlada.

## Estado actual

La base funcional actual incluye:

- solución WinForms .NET 8 editable en Visual Studio;
- lectura y escritura de `Graficos.ind` con formato Integer original y formato Long para GRH/frames;
- detección del encabezado de 263 bytes del formato VB6;
- carga automática de `Init\\Graficos.ind` y resolución de rutas configurables;
- menú principal inspirado en el indexador original;
- cambio de estado para Gráficos, Bodies, Cabezas, Cascos, Escudos, Armas, Alas, Capas, FX, Recursos y Botas;
- listas con restauración de selección por estado y visor GDI+ para GRH/Bodies;
- configuración persistente en `EncodeNet8.settings.json`;
- auditoría de la migración en [AUDITORIA_MIGRACION_VB6_NET8.md](AUDITORIA_MIGRACION_VB6_NET8.md).

La migración todavía es parcial. Las búsquedas, el autoindexador, la edición completa y el resto de índices están planificados en GitHub Issues.

## Manual

- [Manual de EncodeNet8 en Markdown](MANUAL_ENCODE_NET8.md)
- [Manual de EncodeNet8 en PDF](output/pdf/MANUAL_ENCODE_NET8.pdf)

## Versionado y seguimiento

La versión actual es `v0.0.54-beta`. Cada mejora relevante incrementará la versión y se registrará en `CHANGELOG.md` con la referencia al issue correspondiente.

- `beta`: etapa de migración y validación, todavía no estable.
- `patch`: corrección compatible sin añadir una fase nueva.
- `minor`: fase funcional nueva completada.
- `major`: cambio incompatible del formato o del comportamiento público.

Un issue solo se cerrará cuando su criterio de aceptación esté verificado, documentado y publicado en Git.

Las pruebas de `Graficos.ind` se ejecutan con:

```powershell
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release

# Validar archivos direccionales reales
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release -- --directional "K:\\AO_Aospain\\cli\\Init\\Personajes.ind" "K:\\AO_Aospain\\cli\\Init\\Cabezas.ind" "K:\\AO_Aospain\\cli\\Init\\Cascos.ind"

# Validar FX, armas y escudos reales
dotnet run --project .\\EncodeNet8.Tests\\EncodeNet8.Tests.csproj -c Release -- --additional "K:\\AO_Aospain\\cli\\Init\\Fxs.ind" "K:\\AO_Aospain\\cli\\Init\\Armas.dat" "K:\\AO_Aospain\\cli\\Init\\Escudos.dat"
```

## Requisitos

- Windows.
- Visual Studio 2022 con la carga de trabajo de escritorio .NET.
- SDK de .NET 8.
- Archivos de recursos del cliente: carpeta `Init` y carpeta `Graficos`.

## Compilar

Desde esta carpeta:

```powershell
dotnet restore .\\EncodeNet8.slnx
dotnet build .\\EncodeNet8.slnx -c Release
dotnet run --project .\\EncodeNet8\\EncodeNet8.csproj
```

También se puede abrir `EncodeNet8.slnx` directamente en Visual Studio y editar `MainForm` mediante el diseñador.

## Configuración

En `Herramientas > Configuración` se pueden indicar las carpetas `Init` y `Gráficos`. La configuración se guarda junto al ejecutable en `EncodeNet8.settings.json`.

Si no existe una configuración, el programa busca automáticamente `Init\\Graficos.ind` en la carpeta de ejecución y en sus directorios ascendientes.

## Arquitectura

| Componente | Responsabilidad |
|---|---|
| `MainForm.cs` | Flujo de la ventana, estados, listas, edición y visor |
| `MainForm.Designer.cs` | Controles WinForms editables desde Visual Studio |
| `GraficosIndFile.cs` | Lectura, validación y escritura de `Graficos.ind` |
| `GrhModels.cs` | Modelos GRH y registros de cuerpos |
| `IndexerSettings.cs` | Persistencia JSON de rutas |
| `SettingsForm.*` | Formulario de configuración |

## Ruta de implantación

1. Completar `Graficos.ind` y sus pruebas de compatibilidad.
2. Completar `CambiarEstado`, `ComprobarIndexLista` y las listas de todos los tipos.
3. Implementar lectores y escritores de Bodies, Cabezas, Cascos, Capas, Alas y Botas.
4. Implementar FX, Armas y Escudos.
5. Completar el visor por estado, animaciones y referencias.
6. Implementar edición completa y slots de guardado.
7. Implementar búsquedas, superficies y autoindexador.
8. Ejecutar pruebas de regresión con archivos reales Integer y Long.

El detalle de cada fase está en los [Issues del repositorio](https://github.com/scorpio21/indexador_conundrum_.net/issues).

## Criterios de compatibilidad

- No modificar archivos originales sin copia de seguridad.
- No mezclar el formato Integer con el formato Long al guardar.
- Validar siempre límites de GRH, frames y offsets antes de dibujar.
- No portar DirectX 7 automáticamente; documentar y probar cada sustitución.

## Licencia y procedencia

Este proyecto es una migración técnica de un indexador VB6 existente. Consulta los avisos y la licencia del código original antes de redistribuirlo.
