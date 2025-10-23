# Manual de NeoBleeper
- ## ¿Cómo usar NeoBleeper?
  Las dos áreas principales son el teclado (área horizontal en la parte superior de la pantalla) y la lista de música (la vista de lista cerca de la esquina inferior derecha con siete columnas).
  El teclado está dividido en tres secciones de colores, cada una representando una octava.
  Para acceder a notas fuera del rango actual (más agudas que las notas verdes o más graves que las rojas), ajuste el valor de "Octava" ubicado a la derecha e izquierda del teclado.
  Al aumentar este valor en una unidad, las notas del teclado se desplazan una octava más aguda.
  
  ![image](https://github.com/user-attachments/assets/34edbb36-bc79-4d49-8eae-3333337fee12)
  
  La lista de música contiene líneas de notas. Se pueden reproducir hasta 4 notas simultáneamente.
  Las siete columnas de la lista de música son "Duración" (duración de la nota o notas en la línea), las cuatro columnas centrales para las cuatro notas, la sexta columna "Mod" (modificador de longitud) y la séptima columna "Arte" (articulaciones).
  Al pulsar las notas en el teclado, se reproducen inmediatamente a través del altavoz del sistema y se introducen en la lista de música en la columna "Nota 1" de forma predeterminada.
  Este comportamiento se puede cambiar para introducir notas en "Nota 1", "Nota 2", "Nota 3" o "Nota 4", lo que permite a los usuarios identificar las notas al escucharlas.
  Las notas pulsadas en el teclado se insertan encima de la línea seleccionada en la lista de música.
  Si no se selecciona ninguna línea, las notas se añaden al final de la música.
  Para añadir notas al final cuando se selecciona una línea, haga clic en "Deseleccionar línea" (botón turquesa a la derecha de la lista de música).

  ![image](https://github.com/user-attachments/assets/d2baa03e-d0f4-42e8-ac93-aa1f36adcef3)

  - ### Casillas de verificación multipropósito en la lista de notas

    NeoBleeper ahora incluye casillas de verificación multipropósito en la lista de notas, lo que añade potentes funciones de edición y reproducción con una interfaz sencilla e intuitiva.

    ![image1](https://github.com/user-attachments/assets/71e25e18-4e2c-4df3-be18-7d7920919ce8)

    **Características clave de las casillas de verificación multipropósito:**
    - **Reproducir ritmo en líneas marcadas (sin límite de números enteros):**
      Cuando la función "Reproducir ritmo" o una modificación de ritmo está activada, los ritmos se reproducen solo en las líneas donde la casilla está marcada. Esto permite crear patrones rítmicos personalizados, independientemente de los límites de compás o las posiciones de los números enteros; los ritmos pueden reproducirse en cualquier línea marcada para mayor flexibilidad creativa.
    - **Cortar y copiar varias notas:**
      Para cortar o copiar varias líneas a la vez, marque las líneas deseadas y luego use las acciones "Cortar" o "Copiar". Todas las líneas marcadas se incluyen en la operación, lo que permite una edición por lotes eficiente.
    - **Borrar varias notas:**
      Para borrar varias notas a la vez, marque las líneas que desea eliminar y use el botón "Borrar". Esto permite eliminar rápidamente varias notas, reduciendo el trabajo repetitivo y minimizando los errores.
    - **Reproducción de voz en líneas marcadas**
      Al usar el sistema de síntesis de voz, puedes restringir la reproducción de voz solo a las líneas marcadas. Esto facilita resaltar ciertas frases musicales o experimentar alternando entre pitidos tradicionales y síntesis de voz dentro de una canción.
    
    **Consejos de uso**
    - Puedes seleccionar o deseleccionar varias líneas en rápida sucesión para configurar rápidamente qué partes de tu música se verán afectadas por cada operación.
    - Las casillas de verificación multipropósito funcionan en conjunto con otras funciones de edición, lo que permite realizar operaciones avanzadas por lotes sin cambiar de modo.
    - Estas casillas de verificación son independientes de la selección de línea para la entrada de notas: las líneas se pueden marcar para acciones por lotes incluso si se selecciona otra línea para editar.    
      Esta función optimiza tanto la composición como la edición, brindándote mayor control sobre la reproducción, los efectos de voz y la manipulación por lotes de tu música.
      
  Para alternar entre insertar una nota o reemplazar la nota seleccionada, y elegir en cuál de las cuatro columnas de notas se insertará o reemplazará una nota, utilice el cuadro de opciones "Al pulsar la tecla" situado a la izquierda del teclado.
  Este cuadro también permite reproducir notas sin insertarlas ni reemplazarlas.

  ![image](https://github.com/user-attachments/assets/79ccc150-048f-4357-ae66-5dfab2eeaea9)

  Para cambiar la duración de la nota insertada, ajuste el valor "Duración de nota" ubicado en la parte central del teclado.
  Este menú desplegable permite seleccionar notas redondas, 1/2 (blancas), 1/4 (negras), 1/8, 1/16 o 1/32.
  
  ![imagen](https://github.com/user-attachments/assets/1dc5913e-23c0-4b23-9faf-007fcf42c8d2)
  
  Por ejemplo, para crear una blanca con puntillo, inserte una blanca y luego una negra, o inserte una blanca y aplique el modificador de duración "Con puntillo".
  
  ![imagen](https://github.com/user-attachments/assets/18bb971e-3b97-423f-b7aa-88549bf0e1ab)
  
  Si una fusa no es lo suficientemente corta, el tempo (BPM) de la canción se puede ajustar hasta 600 BPM, el doble de la capacidad de la mayoría de los secuenciadores MIDI.
  Esto permite aumentar el tempo y compensarlo duplicando la duración de cada nota.
  El control de tempo se encuentra en la parte inferior del teclado, justo a la derecha del control de compás.
  Ajusta el número de pulsaciones por minuto e incluye un botón de "Metrónomo" para representar audiblemente la velocidad del pulso.
  
  ![imagen](https://github.com/user-attachments/assets/0a2d93e3-eca7-4e2d-8ece-1de66257bf7c)
  
  Se extrae una carpeta llamada "Música" del archivo ZIP. Contiene varios archivos de guardado (NBPML) para este programa, que pueden cargarse como demostraciones. Consulte la sección "Abrir y guardar" a continuación para obtener más información.

  ![image](https://github.com/user-attachments/assets/7d69785d-5e00-45f2-b529-24a869810a98)

- ## Alternancia entre notas
  Dado que el altavoz del sistema solo puede reproducir una nota a la vez, el programa alterna entre "Nota 1", "Nota 2", "Nota 3" y "Nota 4" si varias columnas contienen una nota en la misma línea de la lista de música.
  La velocidad de esta alternancia se controla mediante el valor introducido en el cuadro de texto "Alternar entre las notas de forma sucesiva cada: ... mS", situado a la izquierda de la lista de música.
  Además, el orden de las notas alternas se puede determinar mediante los botones de opción del cuadro de texto "Alternar entre las notas de forma sucesiva cada: ... mS".

  ![image](https://github.com/user-attachments/assets/ea55c400-b070-4f24-a646-dafdf8e10c8f)

- ## Relación nota/silencio
  Este ajuste define la proporción de tiempo que una línea de la lista de música produce sonido frente a silencio. Ajustar esta relación puede reducir la emisión de tono continuo.
  
  ![image](https://github.com/user-attachments/assets/d0d12639-2581-41f7-8b00-32093cb5e79c)

- ## Abrir y guardar archivos
  La música y la configuración se pueden guardar y cargar usando los botones "Abrir", "Guardar" y "Guardar como" en el botón "Archivo" de la barra de menú. La operación de guardar incluye la lista de música y la mayoría de las opciones de configuración.
  
  Los archivos guardados tienen la extensión ".NBPML" y están basados ​​en texto XML. Estos archivos se pueden editar con editores de texto como el Bloc de notas.
  Además, NeoBleeper puede abrir archivos ".BMM", que es el formato de archivo de Bleeper Music Maker, pero no se pueden sobrescribir y deben guardarse como un archivo ".NBPML" independiente.

  ![image](https://github.com/user-attachments/assets/b63002ba-e8d5-4d4e-a7ca-4cca34dabbc0)

- ## Consejos para editar música
   Tanto los archivos NBPML como BMM están basados ​​en texto y son compatibles con editores de texto estándar. Las funciones de copiar y pegar, y buscar y reemplazar pueden ayudar con tareas repetitivas o la corrección de errores.
    
   Para borrar una línea, use el botón rojo "Borrar línea completa". Para borrar solo una columna de notas, use los dos botones azules en la misma área.
    
   ![image](https://github.com/user-attachments/assets/30b11fac-f023-4244-86cc-536f899d6f09)
    
   Para reemplazar la duración de las notas, seleccione la opción de reemplazo y habilite el reemplazo de duración. Luego, haga clic en "Línea en blanco" para cada línea para actualizar la duración sin modificar las notas.

   ![image](https://github.com/user-attachments/assets/aea06cc6-137d-4626-9c11-10206b5ccfbc)

   ![image](https://github.com/user-attachments/assets/abaf6886-8e23-4d5a-bbe4-6141ab158b53)

- ## Reproducir música
  Use el botón verde superior "Reproducir todo" para reproducir todas las notas de la lista de música. La reproducción se repite desde el principio al llegar al final, si la casilla debajo del botón "Línea en blanco" está marcada. El botón verde central reproduce desde la línea seleccionada y vuelve a ella.
  El botón verde inferior detiene la reproducción al terminar la nota actual.
  
  ![image](https://github.com/user-attachments/assets/010965c6-45da-4078-9969-4fa6fa0c760e)
  
  Al hacer clic en una línea de la lista de música, se reproduce esa línea de forma predeterminada. Para modificar este comportamiento o restringir la reproducción a una sola columna de notas, ajuste las casillas de verificación en el cuadro "Al hacer clic en una línea" debajo de "Qué notas de la lista reproducir" en la parte inferior izquierda de la ventana principal.
  Casillas similares en "Cuando se reproduce la música" controlan el comportamiento de la reproducción durante la reproducción automática.
  
  ![imagen](https://github.com/user-attachments/assets/94d17840-c72c-47c5-af47-888b000c9ea9)

- ## Modificadores de longitud y articulaciones

  NeoBleeper admite notas con puntillo y tresillo, así como staccato, spiccato y fermata. La columna "Mod" de la lista de música muestra "Dot" para notas con puntillo y "Tri" para tresillos, y la columna "Art" muestra "Sta" para staccato, "Spi" para spiccato y "Fer" para fermata.
  
  ![imagen](https://github.com/user-attachments/assets/cfd89fa8-e7ad-4ec3-8414-af1f6871c198)
  
  Para aplicar un modificador con puntillo (1,5 veces la duración original), seleccione una línea y haga clic en el botón "Con puntillo" sobre la lista de música. Una nota con puntillo equivale a la duración original más la siguiente nota más corta. Por ejemplo, una negra con puntillo equivale a una negra más una corchea.
  
  Para aplicar un modificador de tresillo (1/3 de la duración original), seleccione una línea y haga clic en el botón "Tresillo". Tres tresillos de la misma duración equivalen a una nota de la duración original. Una línea no puede ser a la vez con puntillo y tresillo.
  
  Para aplicar un modificador Staccato (la mitad de la duración original, luego silencio), seleccione una línea y haga clic en el botón "Staccato".
  
  Para aplicar un modificador de Spiccato (0,25 veces la duración original, luego silencio), seleccione una línea y haga clic en el botón "Spiccato".
  
  Para aplicar un modificador de Fermata (el doble de la duración original), seleccione una línea y haga clic en el botón "Fermata". Una línea no puede ser Staccato, Spiccato y Fermata al mismo tiempo.
  
  Para insertar notas con puntillo, tresillo, Staccato, Spiccato o Fermata, presione el botón correspondiente y luego haga clic en las notas en el teclado. Durante la reproducción, los botones "Punteado", "Tresillo", "Staccato", "Spiccato" y "Fermata" se activan automáticamente al encontrar dichos modificadores y articulaciones.

  ![image](https://github.com/user-attachments/assets/c94b9978-a831-4a70-baca-18f395dc2ee5)

- ## Visualización de compás y posición

  NeoBleeper ofrece la opción "Compás", ubicada a la izquierda del ajuste de BPM. Define el número de pulsos por compás. Esta opción afecta el sonido del metrónomo y la visualización de la posición, pero no altera el sonido de reproducción.
  
  ![image](https://github.com/user-attachments/assets/08c0cd30-effd-4af8-bc15-a4df90c66bb4)
  
  Tres visualizaciones de posición en la esquina inferior derecha muestran la posición actual en la música. La superior muestra el compás, la central muestra el pulso dentro del compás y la inferior muestra una representación tradicional con redondas, blancas (1/2), negras (1/4), corcheas, semicorcheas o fusas.
  
  ![imagen](https://github.com/user-attachments/assets/94f60731-c833-46c9-874d-a6ac21dc99e7)
  
  Los compases más bajos provocan cambios más rápidos en la pantalla superior. La pantalla central se restablece a 1 al comienzo de cada nuevo compás.
  
  La pantalla inferior no puede representar posiciones con una precisión superior a 1/32. Muestra "(Error)" con texto rojo para posiciones no compatibles, como las creadas por semicorcheas con puntillo (3/64). Una vez que la posición vuelve a ser divisible por 1/32, la pantalla vuelve a funcionar con normalidad.
  
  ![imagen](https://github.com/user-attachments/assets/98a3704a-f725-4ea5-a665-2c0de00be77d)
  
  Los tresillos también afectan la precisión de la pantalla. Tras introducir tres notas de tresillo de la misma duración, la posición se vuelve divisible por una negra, lo que restaura la funcionalidad de visualización.
  
  La reproducción de tresillos cerca del final de una lista de música larga puede requerir una cantidad considerable de recursos de CPU. Si se producen problemas de rendimiento, active la casilla "No actualizar" debajo de la visualización de la posición para desactivar las actualizaciones durante la reproducción. Las actualizaciones del modo de edición permanecen activas.
  
  Los archivos BMM antiguos creados con versiones anteriores a la revisión 127 de Bleeper Music Maker tienen un compás de 4 predeterminado al abrirse en NeoBleeper. Cambiar y guardar el compás en archivos .NBPML conserva la configuración.

- ## Registro de depuración

  A partir de la versión 0.18.0 Alpha, NeoBleeper utiliza la clase `Logger` para gestionar todos los registros y diagnósticos. La salida del registro se guarda en un archivo llamado `DebugLog.txt`, ubicado en el directorio raíz de la aplicación.
  
  La clase `Logger` proporciona información detallada en tiempo de ejecución, incluyendo errores, advertencias y mensajes generales de depuración. Este archivo de registro se crea y actualiza automáticamente durante la ejecución de la aplicación.
  
  Para una depuración avanzada, puede iniciar NeoBleeper directamente desde Visual Studio para utilizar sus herramientas integradas, como los puntos de interrupción y la ventana de salida. Sin embargo, el archivo `DebugLog.txt` garantiza que el registro esté disponible de forma constante, incluso fuera del entorno de desarrollo de Visual Studio.
  
  Los archivos de desencadenadores externos, como `logenable`, y los métodos de diagnóstico anteriores ya no son compatibles. Toda la información relevante ahora está centralizada en el archivo `DebugLog.txt` para facilitar el acceso y la revisión.

- ## Mods

  El programa incluye varias modificaciones que alteran su funcionamiento con respecto al diseño original. Estas modificaciones se listan cerca de la esquina inferior izquierda de la pantalla, junto a la lista de música. Cada mod tiene una casilla para activarlo o desactivarlo. Si no se puede desmarcar una casilla, al cerrar la ventana del mod se desactivará y se desmarcará la casilla.
  
  Haz clic en el signo de interrogación junto a la casilla de un mod para ver una breve descripción de su función (disponible para la mayoría de los mods).

  ![image](https://github.com/user-attachments/assets/d0de4ce4-7f72-4d16-badc-deff8c27a40b)
  
  - ### Synchronized Play Mod

    ![image](https://github.com/user-attachments/assets/43371947-72be-4a53-ae34-8988542b07f7)

    Este mod permite que NeoBleeper comience la reproducción a una hora específica del sistema. Está diseñado para sincronizar varias instancias de NeoBleeper, especialmente al usar archivos NBPML o BMM separados para diferentes secciones de una composición. Al configurar cada instancia para que comience a la misma hora, se puede lograr una reproducción sincronizada entre ellas.

    Al activar el mod, se abre una ventana de configuración. Esta ventana permite a los usuarios introducir una hora de inicio objetivo (hora, minuto, segundo) según el reloj del sistema. La hora actual del sistema se muestra como referencia. Por defecto, la hora objetivo está establecida un minuto por delante de la hora actual, pero este valor se puede ajustar manualmente. Los usuarios también pueden especificar si la reproducción debe comenzar desde el principio de la música o desde la línea seleccionada en la lista de música. El programa ejecutará el comando de reproducción correspondiente ("Reproducir todo" o "Reproducir desde la línea seleccionada") cuando se alcance la hora objetivo.
    
    Se proporciona un botón de control para iniciar el estado de espera. Una vez activado, la interfaz indica que el programa está esperando y la etiqueta del botón cambia a "Detener espera". Si el programa no está en estado de espera al llegar la hora objetivo, no se reproducirá.
    
    La casilla "Reproducción sincronizada" está desmarcada y la ventana está cerrada. Para volver a abrir la ventana, deshabilitar esta opción cancelará cualquier estado de espera activo.
    
    La reproducción se detiene automáticamente al iniciar el estado de espera para evitar problemas, a diferencia del Bleeper Music Maker original.
    
    La sincronización entre varios ordenadores es posible si todos los relojes del sistema están perfectamente alineados. Se recomienda sincronizar los relojes del sistema antes de usar esta función en varios dispositivos.

  - ### Mod de Sonido de Ritmo

    ![image](https://github.com/user-attachments/assets/03c7f790-971b-4d0d-b23b-ae62aa6a5271)
    
    Esta modificación garantiza que el altavoz/dispositivo de sonido del sistema emita un sonido de ritmo en cada tiempo o en cada dos tiempos, según la configuración seleccionada. El sonido se asemeja a un ritmo de estilo tecno debido a la naturaleza electrónica del altavoz/dispositivo de sonido del sistema. Al seleccionar la casilla "Reproducir un sonido de ritmo", aparece una ventana de configuración. Esta ventana permite a los usuarios elegir si el sonido de ritmo se reproduce en cada tiempo o en cada tiempo impar. Esta última opción reduce a la mitad el tempo de los sonidos de ritmo.
    
    Para demostrar el cambio de tempo, los usuarios pueden iniciar el programa, agregar cuatro negras a la lista de música, activar la opción "Reproducir un sonido de ritmo" y alternar entre las dos configuraciones de sonido de ritmo. La diferencia de tempo debería ser audible. La casilla "Reproducir sonido de ritmo" está desmarcada al cerrar la ventana de configuración.
    
  - ### Mod Bleeper Portamento
  
    ![image](https://github.com/user-attachments/assets/c776b35e-11cb-4496-b06a-015b40730dc9)
    
    Esta modificación hace que el tono del altavoz/dispositivo de sonido del sistema cambie gradualmente de la nota anterior a la nota actual. Al seleccionar la casilla "Bleeper Portamento", aparece una ventana de configuración. Esta ventana permite ajustar la velocidad de transición entre notas, desde casi instantáneas hasta duraciones prolongadas. También se puede configurar la duración de la nota al hacer clic o configurarla para que se reproduzca indefinidamente.
    
  - ### Usar el teclado como mod de piano
  
    ![image](https://github.com/user-attachments/assets/f5942039-7660-4126-b979-dfd36ec48df5)
    
    Esta función asigna el teclado de la computadora a notas musicales, lo que permite la reproducción directa mediante pulsaciones de teclas sin necesidad de dispositivos de entrada MIDI. Cada tecla corresponde a una nota específica en el piano virtual. La asignación sigue una disposición predefinida, generalmente alineada con las etiquetas visibles del teclado.
    
    Al activarse, al pulsar una tecla se activará inmediatamente la nota asociada utilizando el método de síntesis actual.
    
  - ### Sistema de voz ("Internos de voz")
  
    NeoBleeper ahora incluye un potente sistema de síntesis de voz, accesible a través de la ventana "Internos de voz". Este sistema permite un control avanzado sobre las voces sintetizadas, incluyendo formantes vocálicos, ruido y sibilancia, lo que permite crear sonidos vocales realistas o experimentales directamente en las composiciones.

    ![image](https://github.com/user-attachments/assets/2b8918e7-0825-4895-a8bd-77f3b2506071)

      - #### **Acceso al Sistema de Voz**

        Para abrir la ventana de Internos de Voz, busque la opción "Sistema de Voz" o "Internos de Voz" en el menú o en la selección del dispositivo de salida para cada nota.
        Cada columna de notas (Notas 1-4) ahora puede enrutarse individualmente al sistema de voz, al pitido tradicional o a otros dispositivos de salida mediante los nuevos menús desplegables de "Opciones de salida".
        
      - #### **Resumen de la Ventana de Internos de Voz**
      
        La ventana de Internos de Voz está organizada en secciones, cada una de las cuales le ofrece un control preciso sobre diferentes aspectos de la voz sintetizada:
      
        - ##### **Control de formantes**
        
          Hay cuatro controles deslizantes de formantes, cada uno representando una resonancia clave del tracto vocal humano:
            - Ajuste el **Volumen** y la **Frecuencia (Hz)** para cada formante.
            - Los botones preestablecidos ("Vocales abiertas", "Frontales cerradas", etc.) permiten una selección rápida de sonidos vocálicos típicos.
        
        - ##### **Sección del Oscilador**

          Los controles deslizantes **Volumen de Sierra** y **Volumen de Ruido** controlan el nivel del oscilador de diente de sierra y la fuente de ruido, que forman la base del timbre de la voz.
          Estos controles se pueden combinar con los filtros de formantes para crear diversos efectos sintéticos y vocales.

        - ##### **Sibilancia y Enmascaramiento de Sibilancia**
        
          Cuatro controles de enmascaramiento permiten simular efectos de sibilancia o consonantes modelando los componentes de ruido y enmascarando frecuencias.
          El control deslizante "Hz de Corte" establece un corte de frecuencia para el enmascaramiento de ruido.
          
        ##### **Variaciones aleatorias de formantes**

          - Los controles deslizantes de tono y rango introducen una sutil variación aleatoria en las frecuencias de los formantes, lo que añade realismo o efectos especiales.
        
        - ##### **Opciones de salida**
        
          - Asigna el motor de sonido que utiliza cada columna de notas:
            "Altavoz del sistema/Pito del dispositivo de sonido"
            "Sistema de voz"
            ...y otros, según disponibilidad.
                
          Puedes reproducir una mezcla de pitidos sintetizados por voz y pitidos del altavoz/dispositivo de sonido del sistema en una sola canción.
        
        - ##### **Notas de teclas y uso**

          La ventana muestra una leyenda con la codificación de colores y las abreviaturas de los parámetros.
          Un menú desplegable permite elegir cuándo reproducir la voz (todas las líneas, líneas específicas, etc.).
        
      - #### **Cómo usar el sistema de voz**
          1. **Asignar una nota al sistema de voz**
            En las "Opciones de salida" de la ventana "Internos de voz" o la interfaz principal, configura una columna de notas (p. ej., Nota 2) como "Sistema de voz".         
          2. **Editar formantes y configuración del oscilador**
            Usa los controles deslizantes y los botones de preajustes para dar forma a la vocal, el timbre y la sibilancia.         
          3. **Reproducción**
            Al reproducir música, las columnas de notas seleccionadas usarán el sintetizador de voz según tu configuración.          
          4. **Experimentar**
            Prueba diferentes combinaciones, rangos de aleatorización y mezclas de osciladores para obtener voces sintéticas robóticas, naturales o únicas.

      - #### **Consejos**
        - Combina y combina: Asigna algunas notas al sistema de voz y otras a pitidos para crear bandas sonoras ricas y con capas.
        - Para obtener mejores resultados, ajusta los formantes para que coincidan con el tono de tus notas.
        - Usa los controles deslizantes de aleatorización para lograr irregularidades más "humanas" o artefactos robóticos.
        - El sistema de voz se puede usar para diseño de sonido experimental, no solo para voces.

- ## Configuración
  La ventana de configuración de NeoBleeper se divide en cuatro pestañas principales, cada una dedicada a un aspecto diferente de la configuración de la aplicación.
  
  - ### Configuración general
    Esta pestaña se centra en las preferencias básicas y la integración a nivel de sistema:

    ![image](https://github.com/user-attachments/assets/dc8bc88c-734f-46b5-9dd7-0681bc225d1e)

    - #### Idioma
    **Selector de idioma:** Permite elegir el idioma de NeoBleeper entre inglés, alemán, francés, italiano, español, turco, ruso, ucraniano y vietnamita.
    
    - #### Apariencia general
    **Selector de tema:** Permite elegir entre los temas personalizados de NeoBleeper o la apariencia predeterminada de su sistema operativo.
    **Modo Bleeper clásico:** Una opción heredada para usuarios que prefieren la interfaz o el funcionamiento original.
    
    - #### Crea música con IA
    **Campo de clave API de Google Gemini™:** Entrada segura para habilitar funciones de música generadas por IA.
    **Advertencia de seguridad:** Se recomienda a los usuarios no compartir su clave API.
    **Botones de actualización/reinicio:** Administra el ciclo de vida de la clave API. El botón de actualización está deshabilitado, probablemente pendiente de una entrada válida.
    
    - #### Probando el altavoz del sistema
    **Botón de prueba:** Emite un pitido para confirmar la funcionalidad del altavoz.
    **Mensaje de respaldo:** Sugiere utilizar un dispositivo de sonido alternativo si no se escucha ningún sonido desde el altavoz del sistema.

  - ### Configuración de creación de sonidos
    Esta pestaña permite configurar cómo NeoBleeper genera pitidos de audio utilizando las capacidades de sonido de su sistema. Ofrece control técnico y flexibilidad creativa para moldear el tono y la textura de los sonidos que produce.

    ![image](https://github.com/user-attachments/assets/f5371da1-4983-4416-ab02-5b0e09057377)
    
    - #### Usar dispositivo de sonido para crear pitidos:
      Esta casilla habilita o deshabilita el uso del dispositivo de sonido del sistema para generar pitidos en lugar del altavoz. Si no está marcada, NeoBleeper usa el altavoz del sistema para crear sonido. Al habilitar esta opción, se obtiene una síntesis de sonido más rica, basada en formas de onda.
      
    - #### Creación de pitidos desde la configuración del dispositivo de sonido
      - ##### Selección de la forma de onda del tono
        **Elige la forma de onda para generar pitidos. Cada opción afecta el timbre y el carácter del sonido:**
        
        **Cuadrado (predeterminado):** Produce un tono agudo y vibrante. Ideal para pitidos digitales clásicos y alertas de estilo retro.
        
        **Sinusoidal:** Tono suave y puro. Ideal para notificaciones sutiles o aplicaciones musicales.
        
        **Triangular:** Más suave que el cuadrado, con un sonido ligeramente hueco. Equilibrio entre nitidez y suavidad.
        
        **Ruido:** Genera ráfagas de señal aleatorias, útil para efectos de sonido como estática, ráfagas o texturas de percusión.

  - ### Configuración de dispositivos

    Esta pestaña te permite configurar cómo interactúa NeoBleeper con hardware MIDI externo, instrumentos virtuales y otros dispositivos externos. Ya sea que integres entrada en vivo o enrutes la salida a un sintetizador, aquí es donde defines el flujo de señal.
    
    ![image](https://github.com/user-attachments/assets/6147feec-cac3-463d-a381-937b94da3c8d)

    - #### Dispositivos de entrada MIDI
      **Usar entrada MIDI en vivo:** Habilita la recepción de señales MIDI en tiempo real desde controladores externos o software. Al marcar esta opción, NeoBleeper detecta los mensajes MIDI entrantes para activar sonidos o acciones.
      
      **Seleccionar dispositivo de entrada MIDI:** Un menú desplegable muestra las fuentes de entrada MIDI disponibles. Elija su dispositivo preferido para comenzar a recibir datos MIDI.
      
      **Actualizar:** Actualiza la lista de dispositivos de entrada disponibles, útil al conectar nuevo hardware o al abrir puertos MIDI virtuales.
      
    - #### Dispositivos de salida MIDI
      **Usar salida MIDI:** Activa la transmisión MIDI desde NeoBleeper a dispositivos externos o instrumentos virtuales.
      
      **Seleccionar dispositivo de salida MIDI:** Elige dónde NeoBleeper envía sus señales MIDI. La opción predeterminada suele ser un sintetizador de propósito general como Microsoft GS Wavetable Synth.
      
      **Canal:** Selecciona el canal MIDI (1/16) utilizado para la salida. Esto permite el enrutamiento a instrumentos o pistas específicos en configuraciones multicanal.
      
      **Instrumento:** Define el instrumento General MIDI utilizado para la reproducción. Las opciones van desde pianos y cuerdas hasta sintetizadores y percusión, lo que permite controlar el timbre de la salida.
      
      **Actualizar:** Actualiza la lista de dispositivos de salida disponibles, garantizando que se reconozcan los equipos recién conectados.

    - #### Otros dispositivos y firmware de microcontroladores

      NeoBleeper también admite la interacción con diversos dispositivos externos, como zumbadores, motores y microcontroladores, lo que amplía sus capacidades más allá de los dispositivos MIDI tradicionales. El grupo **Otros dispositivos** dentro de la pestaña Configuración de dispositivos ofrece opciones de configuración y herramientas de generación de firmware para estos componentes externos.

      **Configuración para otros dispositivos:**
      - **Habilitar dispositivo:**
      Casilla para habilitar el uso del motor o zumbador (mediante Arduino, Raspberry Pi o ESP32). Debe marcar esta casilla para acceder a más opciones del dispositivo.
      - **Tipo de dispositivo:**
      Botones de opción para seleccionar entre:
      - **Motor paso a paso**
      - **Motor de CC o zumbador**
      - **Octava del motor paso a paso:**
      Control deslizante para ajustar la octava de la salida del motor paso a paso, lo que permite adaptar el movimiento del motor a los rangos de tonos musicales.
      - **Botón Obtener firmware:**
      Al hacer clic en este botón, se genera el firmware compatible con el dispositivo seleccionado. Debe instalar este firmware en su microcontrolador antes de usar esta función. Si el microcontrolador no está instalado, la casilla del dispositivo permanece inactiva.

      ![image1](https://github.com/user-attachments/assets/4c38b045-efc0-4a30-92d4-8b6e23c28ebd)
      
      **Generador de Firmware para Microcontroladores:**
      - Esta función permite generar y copiar rápidamente firmware listo para usar para microcontroladores (como Arduino) directamente desde NeoBleeper.
      - El firmware permite controlar hardware como zumbadores y motores paso a paso, lo que permite que tus composiciones musicales activen acciones físicas y sonidos.
      - Puedes seleccionar el tipo de microcontrolador (p. ej., "Arduino (archivo ino)") en el menú desplegable.
      - La ventana de código muestra el firmware generado, adaptado al dispositivo seleccionado.
      - Haz clic en el botón "Copiar Firmware al Portapapeles" para copiar fácilmente el código y subirlo a tu microcontrolador.
            
      **Ejemplo de uso:**
      
      Con esta función, puedes sincronizar la reproducción de música con el hardware, como activar zumbadores o controlar motores paso a paso, utilizando las señales de salida del sistema o el código G exportado.
      
      El firmware de Arduino generado incluye gestión de comandos serie para la identificación de dispositivos y el control de la velocidad del motor, lo que facilita la integración de NeoBleeper con robótica o instalaciones personalizadas.
      
      **Consejos de integración:**
      
      Combina la exportación de código G de NeoBleeper con el firmware del microcontrolador para convertir la música en movimientos mecánicos o salidas audibles. El grupo "Otros dispositivos" simplifica la conexión de tu PC a hardware externo, ampliando las posibilidades creativas para máquinas musicales, actuaciones cinéticas o arte sonoro experimental.
      
      > Para más detalles o resolución de problemas, consulta los canales de soporte de NeoBleeper o la documentación de tu microcontrolador.

  - ### Ajustes de apariencia
    Esta pestaña te da control total sobre la identidad visual de NeoBleeper, permitiéndote personalizar los colores de los elementos clave de la interfaz para mayor claridad, estética o estilo personal. Está organizada en secciones para teclado, botones, indicadores y visualización de eventos de texto.
        
    ![image5](https://github.com/user-attachments/assets/210bac21-692a-4124-b45c-c83aafce588a)

    - #### Colores del teclado
      **Define el esquema de colores para las diferentes octavas del teclado virtual:**
      
      **Color de la primera octava:** Naranja claro
      
      **Color de la segunda octava:** Azul claro
      
      **Color de la tercera octava:** Verde claro
      
      Estos ajustes ayudan a distinguir visualmente los rangos de tonos, lo que facilita tanto la interpretación como la composición.
      
    - #### Colores de botones y controles
      **Personaliza la apariencia de los elementos interactivos en la interfaz:**
      
      **Color de la línea en blanco:** Naranja claro
      
      **Color de las notas claras:** Azul
      
      **Color de la línea deseleccionada:** Cian claro
      
      **Color de la línea completa borrada:** Rojo
      
      **Color de los botones de reproducción:** Verde claro
      
      **Color del metrónomo:** Azul claro
      
      **Color del marcado del teclado:** Gris claro
      
      Estas asignaciones de color mejoran la usabilidad al hacer que las acciones y los estados sean visualmente intuitivos.
      
    - #### Colores de los indicadores
      **Configura los colores de los indicadores de retroalimentación en tiempo real:**
      
      **Color del indicador de pitido:** Rojo
      
      **Color del indicador de nota:** Rojo
      
      Estos indicadores parpadean o se resaltan durante la reproducción o la entrada, lo que te ayuda a monitorear la actividad de un vistazo.
      
    - #### Ajustes de letras/eventos de texto
      **Tamaño de letras/eventos de texto:** Ajusta el tamaño (en puntos) de las letras o eventos de texto que se muestran durante la reproducción de archivos MIDI u otras funciones basadas en eventos.
      
      **Vista previa de ajustes de letras/eventos de texto:** Usa este botón para previsualizar cómo aparecerán las letras o los eventos de texto, asegurando que la legibilidad y el estilo se ajusten a tus preferencias.
      
    - #### Opción de reinicio
      **Restablecer la configuración de apariencia a los valores predeterminados:** Un botón de un solo clic para restaurar todos los ajustes de color y apariencia a sus valores predeterminados originales, perfecto para deshacer experimentos o empezar de cero.

  - ## Herramientas
    Estas herramientas compactas pero potentes del menú "Archivo" ofrecen acceso rápido a tres funciones principales de NeoBleeper, cada una diseñada para optimizar tu flujo de trabajo y ampliar tus posibilidades creativas. Cada opción incluye un atajo de teclado para un control rápido y práctico:
    
    ![image](https://github.com/user-attachments/assets/68a9143c-c6b8-4ae0-8d16-bc45d87ef349)

    - ### Reproducir archivo MIDI - `Ctrl + M`
      Carga y reproduce instantáneamente un archivo MIDI a través del altavoz del sistema o del dispositivo de sonido en NeoBleeper. Esta función es ideal para previsualizar composiciones, comprobar la precisión de la reproducción o integrar datos MIDI externos en tu flujo de trabajo.
      
      ![image](https://github.com/user-attachments/assets/d66a25cb-f170-4505-bbeb-8c9d86186e5b)
      
      Selecciona el archivo MIDI haciendo clic en "Explorar archivo" en la ventana "Configuración de reproducción de archivos MIDI". El archivo MIDI seleccionado aparece en el cuadro de texto a la izquierda del botón.
      
      El tiempo se muestra como "00:00.00" (minutos, segundos, centésimas de segundo). Se actualiza solo cuando el temporizador de reproducción está activado y los mensajes MIDI se reproducen en el momento correcto, siempre que el tempo se mantenga sin cambios.
      El porcentaje indica la proporción de mensajes MIDI procesados. Por ejemplo, si la primera mitad contiene pocos mensajes y la segunda mitad es densa, el porcentaje podría no alcanzar el 50% hasta bien entrada la reproducción. La casilla "Bucle" permite que el archivo MIDI se reinicie automáticamente al finalizar.
      Los tres botones debajo del control deslizante, de izquierda a derecha, sirven para rebobinar (saltar al principio del archivo MIDI), reproducir (desde la posición actual) y detener (sin rebobinar). Una casilla debajo de estos controles permite la reproducción en bucle.
      
      En esta ventana, los usuarios pueden seleccionar canales específicos para la entrada. Los canales no seleccionados se ignorarán. Los usuarios pueden marcar o desmarcar las casillas y los cambios surtirán efecto inmediatamente. Al seleccionar una casilla, las notas del canal correspondiente se procesarán durante la reproducción.
      
      En la parte inferior de la ventana "Reproducir archivo MIDI", una cuadrícula de rectángulos muestra las notas sostenidas. Cada rectángulo representa una nota sostenida. Se pueden mostrar hasta 32 rectángulos simultáneamente. Si se mantienen más de 32 notas, solo se muestran las primeras 32.
      
      Modificar la opción "Cambiar entre notas cada ... ms" en la ventana "Reproducir archivo MIDI" afecta la velocidad de ciclo de las notas recibidas de la entrada MIDI.
      
      Si la casilla "Reproducir cada nota solo una vez (no alternar)" está marcada, cada nota se reproduce una vez durante el tiempo especificado en la opción "Cambiar entre notas cada ... ms". Esto produce un efecto más staccato.
      
      Si la casilla "Intentar que cada ciclo dure 30 ms (con un tiempo máximo de alternancia de 15 ms por nota)" está marcada, la duración de la alternancia se ajusta automáticamente para cumplir con este comportamiento de sincronización. Esto ayuda a mantener una sincronización precisa cuando se reproducen varias notas en rápida sucesión.
            
      #### Visualización de letras y eventos de texto
        
        El reproductor de archivos MIDI de NeoBleeper incluye una función para mostrar letras o eventos de texto incrustados en archivos MIDI, lo que proporciona información visual en tiempo real de las líneas vocales o pistas para karaoke y presentaciones.
        
        ![image1](https://github.com/user-attachments/assets/a5b121bd-85b5-4532-9b64-dc58b6e5448e)
        
        Al activar la casilla "Mostrar letras o eventos de texto" en la ventana "Reproducir archivo MIDI", cualquier evento de letra o texto incrustado en el archivo MIDI que se esté reproduciendo se mostrará de forma destacada en la parte inferior de la ventana de la aplicación. Estos eventos aparecen como superposiciones de texto grandes y claras, que se actualizan sincronizadas con la progresión de la canción.
        
        Esta función es especialmente útil para seguir las partes vocales, dar pistas a los artistas en vivo o simplemente disfrutar de una reproducción estilo karaoke. Si el archivo MIDI no contiene eventos de letra ni texto, la superposición permanece oculta.
        
        La visualización de la letra y el texto se actualiza automáticamente a medida que se detectan nuevos eventos durante la reproducción y desaparece al detenerse la reproducción o al cargar un nuevo archivo.
            
    - ### Crea música con IA - `Ctrl + Alt + A`
      Aprovecha el poder de la IA para generar ideas musicales. Ya sea que busques inspiración, completes tus huecos o experimentes con nuevos estilos, esta herramienta ofrece sugerencias inteligentes y contextuales para melodías, armonías y ritmos.
      
      ![image](https://github.com/user-attachments/assets/9d27cb8a-2f9a-44df-b99c-11edeaaf235d)
      
      **Cómo funciona:**
      - Abre la ventana "Crear música con IA" desde el menú Archivo o usando el acceso directo.
      - Elige el **modelo de IA** que desees (p. ej., Gemini 2.5 Flash) en el menú desplegable.
      - Introduce una indicación musical en el cuadro **Indicación** (p. ej., "Generar una melodía folk con guitarra acústica").
      - Haz clic en **Crear** para que la IA genere la música. Una barra de progreso indicará cuándo se está procesando la solicitud.
      - Una advertencia le recuerda que los resultados son sugerencias inspiradoras y pueden contener errores.
      - Esta función funciona con Google Gemini™.

      **Guía de indicaciones y restricciones de la IA:**
      - La herramienta de IA solo procesará indicaciones relacionadas con la composición musical. Si tu indicación no está relacionada con la música (p. ej., "escribe un chiste"), recibirás un error:
        
        ![image](https://github.com/user-attachments/assets/c3c8dbfb-09ef-4bfb-942c-ca5f25afca73)
        
        *"Solicitud no relacionada con musica detectada. Las solicitudes deben ser sobre composicion musical, artistas o generos musicales."*

      - No se permiten mensajes con contenido ofensivo o inapropiado. Si se detectan, se mostrará un error:

        ![image3](https://github.com/user-attachments/assets/759c9d2e-73f3-4028-bc8e-2e9a3216d8be) 

        *"Contenido inapropiado detectado. Intente solicitar una composicion musical o informacion musical relacionada con un artista."*

      - Las indicaciones válidas deben ser específicas y estar centradas en la música (por ejemplo, "Genera una melodía de jazz para piano" o "Crea un patrón de batería techno rápido").
      
      **Notas:**
        - Si no se escribe ninguna indicación al hacer clic en el botón "Crear", la IA usará como indicación el texto de la indicación en el cuadro de texto.
        - La música generada por IA está diseñada para inspirar y debe revisarse antes de usarla en composiciones finales.
        - La IA no garantiza resultados perfectos ni estilísticamente precisos.
        - Se debe verificar la precisión y musicalidad de todo el contenido generado antes de su uso público.
        
      **Integración con opciones de salida:**
        - Puedes usar la música generada por IA con cualquier motor de salida (pitido del sistema, dispositivo de sonido o sistema de voz).
        - Asigna la música generada por IA a columnas de notas específicas y combínala con funciones tradicionales o de síntesis de voz para obtener resultados únicos.
          
    - ### Convertir a GCode - `Ctrl + Shift + G`

      Transforma datos musicales en GCode para zumbadores o motores de máquinas CNC o impresoras 3D. Esto conecta el sonido con el movimiento, permitiendo representaciones físicas de secuencias musicales, ideal para arte experimental o herramientas educativas.
      
      ![image](https://github.com/user-attachments/assets/0f6d8845-3953-4a9f-9823-97e2986b7743)
      
      Esta función convierte configuraciones de notas musicales seleccionadas en instrucciones GCode para su uso con máquinas CNC o impresoras 3D. Se pueden definir hasta cuatro notas, cada una asignada a un tipo de componente (motor M3/M4 y M300 para zumbador). Las notas se pueden alternar individualmente.
      
      El orden de reproducción se puede configurar para alternar notas secuencialmente o por paridad de columnas (primero las columnas impares, luego las pares).
      
      Al activarse, el sistema genera un código G que activa los componentes asignados según el patrón de notas seleccionado. La sincronización y la modulación se determinan mediante la lógica de reproducción.
      
      Utilice el botón "Exportar como código G" para guardar el resultado. Asegúrese de que sea compatible con el equipo de destino antes de ejecutarlo.

    - ### Convertir a comando de pitido para Linux - `Ctrl + Shift + B`

      Convierte rápidamente tus composiciones musicales en un script de comando de pitido compatible con Linux para una fácil reproducción en sistemas Linux.

      ![image1](https://github.com/user-attachments/assets/45e323fd-4b9a-4311-919e-08b0cf3fecfd)
      
      **Resumen de funciones:**
      - NeoBleeper genera una secuencia de comandos de pitido que representan tu música, formateada para la utilidad `beep` de Linux.
      - Cada nota y silencio se traduce a los parámetros de pitido apropiados (`-f` para frecuencia, `-l` para duración, `-D` para retardo y `-n` para encadenar notas).
      - El resultado es un solo comando (o una serie de comandos) que se puede ejecutar en una terminal Linux para reproducir tu música usando el altavoz del sistema.
      
      **Cómo usar:**
        1. Componga su música en NeoBleeper como de costumbre.
        2. Abra la herramienta "Convertir a comando de pitido para Linux" desde el menú Archivo.
        3. Su música se convertirá instantáneamente en un script de comando de pitido y se mostrará en un área de texto.
        4. Use el botón "Copiar comando de pitido al portapapeles" para copiar el comando y usarlo en su terminal.
        5. También puede guardar el comando como un archivo `.sh` haciendo clic en "Guardar como archivo .sh" para ejecutarlo posteriormente en cualquier sistema Linux compatible.
      
      **Ejemplo de salida:**
        - El comando podría verse así:
        ```
        beep -f 554 -l 195 -n -f 0 -l 0 -D 5 -n -f 523 -l 195 -n ...
        ```
        Cada grupo de parámetros corresponde a una nota musical o silencio.

     **Integración y consejos:**
      - Ideal para compartir música con usuarios de Linux o para usar en scripts de shell.
      - El comando es compatible con la utilidad estándar de Linux «beep» (asegúrese de que esté instalada y de tener permisos para usar el altavoz del sistema).
      - Editar el comando generado permite ajustes rápidos de tempo, tono o ritmo.
      
      Esta función agiliza el proceso de llevar tu música a entornos Linux y permite usos creativos como notificaciones musicales, sistemas de alerta o simplemente disfrutar de tus composiciones fuera de NeoBleeper.
