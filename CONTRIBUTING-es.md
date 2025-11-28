# 🤝 Contribuyendo a NeoBleeper

Antes que nada, ¡gracias por considerar contribuir a NeoBleeper! Tus contribuciones son fundamentales para el éxito de este proyecto. Ya sea reportando un error, proponiendo una función, mejorando la documentación, subiendo archivos BMM o NBPML antiguos, o enviando código, tu participación es muy valiosa.

## 📑 Índice
1. [Código de Conducta](#codigo-de-conducta)
2. [¿Cómo puedo contribuir?](#como-puedo-contribuir)
- [Informes de errores](#informes-de-errores)
- [Solicitudes de funciones](#solicitudes-de-funciones)
- [Contribuciones de código](#contribuciones-de-código)
- [Documentación](#documentación)
- [Contribuciones de archivos BMM y NBPML](#contribuciones-de-archivos-bmm-y-nbpml)
3. [Proceso de solicitud de extracción](#proceso-de-solicitud-de-extracción)
4. [Guías de estilo](#guias-de-estilo)
- [Estilo de código](#estilo-de-código)
- [Notas específicas de C#](#notas-específicas-de-c-sharp)
5. [Soporte de la comunidad](#soporte-de-la-comunidad)

## 🌟 Código de Conducta
Al participar en este proyecto, aceptas cumplir con el Código de Conducta. Sé respetuoso y considerado con los demás miembros de la comunidad. Consulta el archivo `CODE_OF_CONDUCT.md` para obtener más información.

## 🤝🙋‍♂️ ¿Cómo puedo contribuir?

### 🪲 Informes de errores
Si encuentras un error en NeoBleeper, crea un informe e incluye la siguiente información:
- Un título claro y descriptivo.
- La versión de NeoBleeper o el hash de confirmación, si corresponde.
- Pasos para reproducir el problema o un fragmento de código.
- Comportamiento esperado y real.
- Cualquier otro detalle relevante, incluyendo capturas de pantalla o registros de fallos.

### 💭 Solicitudes de funciones
¡Agradecemos tus ideas! Para solicitar una función:
1. Revisa los informes para ver si alguien más la ha solicitado.
2. 2. De lo contrario, abra una nueva incidencia y comparta una descripción detallada que incluya:
- Antecedentes de la solicitud.
- Por qué es valiosa.
- Posibles impactos, riesgos o consideraciones.

### 👩‍💻 Contribuciones de código
1. Bifurque el repositorio y cree una nueva rama a partir de `main`. Asigne a la rama un nombre descriptivo, como `feature/add-tune-filter`.
2. Abra la carpeta del repositorio en Visual Studio:
- Asegúrese de tener instalado [Visual Studio](https://visualstudio.microsoft.com/) con las cargas de trabajo necesarias (por ejemplo, "Desarrollo de escritorio .NET" para NeoBleeper).
- Clone la bifurcación del repositorio en su equipo local (puede usar las herramientas Git integradas de Visual Studio o la CLI de Git).
- Una vez clonado, abra el archivo de la solución (`.sln`) en Visual Studio. 3. Instalar paquetes NuGet:
- Restaurar las dependencias necesarias haciendo clic en "Restaurar paquetes NuGet" en la barra superior o ejecutando "dotnet restore" desde la terminal.
4. Agregar los cambios:
- Usar las funciones de Visual Studio, como IntelliSense, la depuración y el formato de código, para contribuir eficazmente.
- Asegurarse de que se incluyan las pruebas correctas y de que todas las existentes sean correctas.
- Asegurarse de que el código se ajuste a la guía de estilo.
5. Agregar su nombre o alias a la página "Acerca de":
- Abrir el archivo "about_neobleeper.cs" y localizar el componente "listView1".
- Seleccionar el componente "listView1" en el diseñador de Visual Studio.
- Hacer clic en la flecha pequeña de la esquina superior derecha del componente para abrir el menú desplegable.
- Seleccionar "Editar elementos" para abrir el editor de la colección de elementos de ListView.
- Agregar un nuevo "ListViewItem":
- Escribir su nombre o alias en la propiedad "Texto". Para tus contribuciones/tareas:

- Localiza la propiedad **SubItems**.

- Haz clic en los tres puntos (`...`) a la derecha del campo `(Collection)`.

- Agrega o edita el **SubItem** con una breve descripción de tus tareas.

- Si ya has añadido tu nombre, edita el SubItem o actualiza la entrada existente antes de confirmar los cambios.

6. Prueba tu código:

- Ejecuta las pruebas con el Explorador de pruebas de Visual Studio.

- Corrige las pruebas fallidas y valida los cambios.

7. Confirma los cambios con mensajes claros y concisos.

- Usa las herramientas Git integradas de Visual Studio para preparar y confirmar los cambios.

8. Sube tu rama y abre una solicitud de extracción en el repositorio.

9. Prepárate para colaborar con los revisores y realizar las revisiones necesarias.

### 🧾 Documentación
¡Mejorar nuestra documentación es una de las maneras más fáciles de contribuir! No dudes en agregar o actualizar ejemplos, aclarar secciones o mejorar la legibilidad general.

### 🎼 Contribuciones de archivos BMM y NBPML
NeoBleeper admite archivos BMM (Bleeper Music Maker) y NBPML (lenguaje de marcado del proyecto NeoBleeper) heredados. Si contribuye o trabaja con estos tipos de archivos, asegúrese de lo siguiente:
- Validar que los archivos BMM se analicen correctamente y se rendericen como se espera en NeoBleeper.
- Probar la compatibilidad con los formatos heredados y la implementación actual.
- Para los archivos NBPML, cumpla con las especificaciones más recientes del lenguaje de marcado del proyecto NeoBleeper.

Si encuentra algún problema específico con estos formatos de archivo, siga las directrices de la sección "Informes de errores". También agradecemos las solicitudes de funciones para mejorar la compatibilidad con archivos BMM y NBPML.

## ⬇️ Proceso de solicitud de extracción
Todos los envíos deben realizarse mediante solicitudes de extracción. Este es el proceso:
1. Complete la plantilla de solicitud de extracción.
2. Asegúrese de que su solicitud de extracción no duplique las existentes.
3. Agregue los detalles de los cambios en la descripción, haciendo referencia a los problemas relacionados siempre que sea posible.
4. Responda a todos los comentarios o cambios solicitados por los revisores.
5. Las solicitudes de extracción deben superar todas las comprobaciones de CI/CD, incluyendo pruebas y controles de calidad del código.

## 📖 Guías de estilo
### ✨ Estilo de código
Siga las [Convenciones de codificación .NET](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Puntos clave:
- Prefiera las propiedades automáticas a los campos públicos.
- Use `var` para variables locales cuando el tipo sea obvio.
- Evite las cadenas mágicas y los números. Use constantes o enumeraciones.

### 📒 Notas específicas de C#
- Coloca `{` en la misma línea que el código anterior.
- Usa PascalCase para los nombres de clases y métodos, y camelCase para las variables locales.
- Sigue las [Directrices de nomenclatura de Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Soporte de la comunidad
Si tienes alguna pregunta, no dudes en abrir una discusión en GitHub o contactarnos a través de la sección de incidencias. Animamos a todos a compartir conocimientos y a ayudar a otros colaboradores.

¡Gracias por contribuir a NeoBleeper y por ayudar a crear algo increíble!
