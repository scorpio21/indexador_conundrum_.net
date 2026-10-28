# Fixtures de regresión

Las pruebas crean aquí su colección lógica de fixtures en un directorio temporal único por ejecución. No se modifican recursos del cliente ni se escriben archivos dentro de `init` o `graficos`.

La colección cubre:

- `Graficos.ind` Integer y Long, con GRH simple, animación y límites de frames.
- Índices direccionales con y sin offsets en Integer y Long.
- FX y archivos INI de armas, escudos y botas.
- `Indices.ini` de WE con referencias consecutivas.
- BMP real temporal, BMP ausente, recortes inválidos y archivos truncados.
- Todos los estados de `MainForm`: Grh, Body, Cabezas, Cascos, Escudos, Armas, Alas, Capas, Botas, Fx y Resource.
