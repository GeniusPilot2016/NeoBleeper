# Guía de solución de problemas de NeoBleeper

Esta guía ofrece soluciones a problemas comunes al usar NeoBleeper, especialmente aquellos relacionados con el funcionamiento de los altavoces del sistema, la salida de sonido, la compatibilidad de hardware y los pitidos persistentes del sistema.

---

## 1. Sonido bloqueado en el altavoz del sistema tras un fallo o cierre forzado

**Problema:**
Si NeoBleeper se bloquea o se cierra forzadamente mientras se reproduce audio a través del altavoz del sistema (PC), el sonido puede bloquearse, lo que provoca pitidos o zumbidos continuos.

**Por qué ocurre esto:**
El altavoz del sistema se controla a un nivel bajo de hardware/software. Si la aplicación no libera o reinicia correctamente el altavoz al salir, el sonido puede persistir.

**Soluciones:**
- **Usa la utilidad NeoBleeper - Detección de pitidos (para la versión de 64 bits):**
  NeoBleeper incluye una herramienta llamada "NeoBleeper - Detección de pitidos" en la carpeta del programa.
  
  ![image4](https://github.com/user-attachments/assets/c6d0a5e1-07ed-4447-ad12-f6bb33fa33eb)
  
  - Inicia esta herramienta y pulsa el botón **Detener pitido** para detener el pitido bloqueado del altavoz del sistema.
  - Usa esta utilidad solo cuando el pitido persista tras un fallo o un cierre forzado.

  #### Mensajes de Detección de pitidos y su significado

  Al usar Detección de pitidos, es posible que vea los siguientes mensajes:

  ![image1](https://github.com/user-attachments/assets/10b9d329-195b-41fb-8e13-772bcd2df1ae)
    
    **El altavoz del sistema no emite ningún pitido o emite un pitido diferente. No se realizó ninguna acción.**  
    Este mensaje aparece cuando la utilidad revisa el altavoz del sistema y determina que no emite ningún pitido o que emite un pitido que la herramienta no puede controlar. En este caso, Beep Stopper no realizará ninguna otra acción.
    - *Consejo:* Si el pitido persiste, intente reiniciar el ordenador.

  ![image2](https://github.com/user-attachments/assets/53e2571d-f825-4cc0-8a3c-42d7dd3ed28b)
    
    **¡El pitido se detuvo correctamente!**  
    Este mensaje confirma que la utilidad Beep Stopper detectó un pitido atascado y logró detenerlo correctamente. No se requiere ninguna acción adicional.

  ![image3](https://github.com/user-attachments/assets/12dcb625-3395-4488-8f8a-fc4bec3bce53)
  
    **La salida de los altavoces del sistema no está presente o la salida no es la estándar. El silenciador de pitidos puede causar inestabilidad o comportamientos indeseados. ¿Desea continuar?**  
    Este mensaje aparece cuando se inicia la utilidad Beep Stopper y detecta que su sistema no tiene un altavoz estándar (de PC) o que la salida del altavoz no es estándar. En este caso, la utilidad le advierte que intentar usar Beep Stopper podría no funcionar como se espera y podría causar un comportamiento inesperado o inestabilidad.

    Si continúa, la herramienta intentará detener el pitido, pero podría ser ineficaz o tener efectos secundarios si su hardware no es compatible o no es estándar.
    Si decide no continuar, la herramienta se cerrará sin realizar ningún cambio.
    - *Consejo:* Si recibe este mensaje, significa que su ordenador no tiene un altavoz compatible o que su salida no se puede controlar correctamente. Es probable que cualquier pitido o zumbido que escuche provenga de otro dispositivo de audio (como los altavoces principales o los auriculares). Utilice la configuración estándar de su dispositivo de sonido para solucionar los problemas de sonido y cierre cualquier aplicación que pueda estar produciendo audio no deseado. Si el problema persiste, intente reiniciar el ordenador o revisar la configuración de sonido de su dispositivo.

- **Reinicie su computadora:**
  Si Beep Stopper no resuelve el problema, reinicie el sistema para restablecer el hardware del altavoz.

- **Prevención:**
  Siempre cierre NeoBleeper normalmente. Evite forzar su cierre mediante el Administrador de tareas o herramientas similares mientras se reproduce sonido.

---

## 2. Detección y compatibilidad de altavoces del sistema

NeoBleeper incluye lógica de detección para comprobar si su sistema dispone de una salida de altavoz de PC estándar, así como compatibilidad con salidas de altavoz ocultas (como las que no utilizan el ID PNP0800). Si su hardware no admite un altavoz de sistema estándar u oculto, o si la salida no es estándar y no se puede utilizar, es posible que vea mensajes de advertencia o que tenga que usar su dispositivo de sonido habitual para los pitidos. Sin embargo, a partir de versiones recientes, NeoBleeper ya no le obliga a usar el dispositivo de sonido exclusivamente cuando no hay un altavoz estándar; ahora permite el uso de salidas de altavoz ocultas/no PNP0800, si las hay.

### Ejemplo de advertencia (Imagen 1):

![imagen1](https://github.com/user-attachments/assets/b9996af5-ea37-4d28-bc93-1b097ec06280)

> **Explicación:**
> La placa base de su ordenador no tiene una salida de altavoz de sistema estándar, o la salida no es estándar. NeoBleeper intentará detectar y ofrecer el uso de salidas de altavoz de sistema "ocultas" no identificadas como PNP0800. Si dicha salida está disponible, podrá usar el altavoz del sistema incluso si aparece esta advertencia. De lo contrario, NeoBleeper volverá a su dispositivo de sonido habitual (como altavoces o auriculares).

### Diálogos de configuración (Imágenes 2 y 3):

![image2](https://github.com/user-attachments/assets/03557983-51c4-4838-ad49-89c11009db15)

![image3](https://github.com/user-attachments/assets/51ee6d34-0d20-4403-8a94-92cb586ee891)

- **Disponibilidad del botón "Probar altavoz del sistema":**
  Esta opción se activa si NeoBleeper detecta alguna salida de altavoz del sistema utilizable, incluidas las salidas ocultas o que no sean PNP0800.
- **Ajuste "Usar dispositivo de sonido para crear pitido":**
  Ahora puede desactivar esta función si se detecta una salida de altavoz del sistema oculta o no estándar.

#### ¿Qué significa "salida de altavoz del sistema no estándar"?
Algunas computadoras, portátiles o máquinas virtuales modernas no tienen un altavoz de PC real o el enrutamiento de la señal no es estándar. NeoBleeper ahora intenta detectar y utilizar estas salidas de altavoz del sistema ocultas (no identificadas como dispositivos PNP0800), pero solo puede habilitar la opción de altavoz del sistema si es realmente accesible a nivel de hardware. Si no se encuentra ninguna salida utilizable, deberá usar su dispositivo de sonido habitual.

## 2.1 Prueba de Salida de Altavoz del Sistema (Detección de Frecuencia Ultrasónica)

NeoBleeper ahora incluye una nueva prueba de hardware avanzada para detectar la salida del altavoz del sistema (también conocido como altavoz del PC), incluso si Windows no lo detecta (en ciertos identificadores como PNP0C02 en lugar de PNP0800). Esta prueba utiliza frecuencias ultrasónicas (normalmente de 30 a 38 kHz, que son inaudibles) y analiza la retroalimentación eléctrica en el puerto del altavoz del sistema.

- **Cómo funciona:**
  Durante el inicio, NeoBleeper realiza un segundo paso después de la comprobación habitual del identificador del dispositivo. Envía señales ultrasónicas al puerto del altavoz del sistema y monitoriza la retroalimentación del hardware para detectar la presencia de una salida de altavoz funcional, incluso si está oculta o no es estándar.

- **Qué puede notar:**
  En algunos sistemas, especialmente aquellos con zumbadores piezoeléctricos, es posible que escuche leves clics durante esta etapa. Esto es normal e indica que la prueba de hardware se está ejecutando.
  
  ![image4](https://github.com/user-attachments/assets/f4510db6-1f61-457c-85e0-93680d8287d7)
  
  *Comprobando la presencia de la salida del altavoz del sistema (altavoz de PC) en el paso 2/2... (puede que oiga clics)*

- **¿Por qué esta prueba?**
  Muchos sistemas modernos carecen de un altavoz del sistema PNP0800, pero aún tienen una salida de altavoz utilizable (oculta). NeoBleeper utiliza este método avanzado para habilitar funciones de pitido en más hardware.
  
---

## 3. Compatibilidad y limitaciones con ARM64

**Dispositivos basados ​​en ARM64:** En sistemas Windows ARM64, la prueba "Altavoz del sistema" y la casilla "Usar dispositivo de sonido para generar pitidos" **no están disponibles** en NeoBleeper. En su lugar, todos los pitidos y sonidos se reproducen siempre a través de su dispositivo de audio habitual (altavoces o auriculares).

- El botón "Probar altavoz del sistema" y las funciones de detección relacionadas **no** estarán visibles en la configuración de dispositivos ARM64.

- La opción "Usar dispositivo de sonido para generar pitidos" no está presente porque este comportamiento se aplica automáticamente.

- Esta limitación se debe a que el acceso directo al hardware del altavoz del sistema no está disponible en plataformas Windows ARM64.

- En ARM64, siempre escuchará los pitidos a través de su dispositivo de salida de audio habitual.

**Si utiliza un equipo ARM64 y no ve las opciones del altavoz del sistema en NeoBleeper, esto es normal y no se trata de un error.**

---

## 4. Cómo comprobar la presencia de altavoces del sistema

- **Ordenadores de escritorio:** La mayoría de los ordenadores de escritorio antiguos tienen un conector para altavoces de PC en la placa base. Los sistemas más nuevos pueden omitir esta función o presentar la salida en un formato oculto/no PNP0800 que NeoBleeper ahora puede utilizar.
- **Portátiles:** La mayoría de los portátiles no tienen un altavoz de sistema independiente; todo el sonido se enruta a través del sistema de audio principal.
- **Máquinas virtuales:** La emulación de altavoces del sistema suele estar ausente o ser poco fiable; es posible que las salidas no PNP0800 no estén disponibles.
- **Cómo saberlo:** Si ve las advertencias anteriores, pero puede habilitar y probar el altavoz del sistema en NeoBleeper, es probable que su ordenador tenga una salida oculta o no estándar.
  
---

## 5. ¡No oigo ningún sonido!

- **Comprueba la configuración de NeoBleeper:**

  Si el altavoz del sistema no está disponible, asegúrate de que el dispositivo de sonido (altavoces/auriculares) esté correctamente seleccionado y funcionando.

- **Comprueba el mezclador de volumen de Windows:**

  Asegúrate de que NeoBleeper no esté silenciado en el mezclador de volumen del sistema. - **Prueba el botón "Probar altavoz del sistema":**
  Úsalo para probar el altavoz de tu PC.
- **Lee los mensajes de advertencia:**
  NeoBleeper te dará instrucciones específicas si no puede acceder al altavoz del sistema.

---

## 6. Preguntas frecuentes

### P: ¿Puedo usar el altavoz del sistema si mi hardware no tiene un dispositivo PNP0800?
**R:** ¡Sí! NeoBleeper ahora intenta detectar y usar salidas de altavoz del sistema ocultas o que no sean PNP0800 siempre que sea posible. Si lo logra, podrá usar el altavoz del sistema incluso si Windows no detecta un dispositivo estándar.

### P: ¿Por qué la configuración "Usar dispositivo de sonido para crear un pitido" a veces se vuelve permanente (en versiones anteriores)?
**R:** Cuando no se detecta una salida de altavoz del sistema estándar (en versiones anteriores), NeoBleeper aplica esta configuración para garantizar que la salida de sonido siga siendo posible.

### P: ¿Hay alguna solución alternativa si no se detecta el altavoz del sistema?
**R:** Debe usar su dispositivo de sonido habitual (altavoces/auriculares) si no se encuentra una salida de altavoz del sistema estándar (en versiones anteriores).

### P: ¿Qué pasa si la herramienta Beep Stopper no detiene el pitido bloqueado? **R:** Reinicie su computadora para restablecer el hardware de los altavoces si la utilidad Beep Stopper falla.

### P: ¿Por qué escucho clics al iniciar?
**R:** Durante la prueba avanzada de salida de los altavoces del sistema (paso 2), NeoBleeper envía señales ultrasónicas al hardware para detectar salidas de altavoz ocultas o no estándar. En algunos sistemas (especialmente aquellos con zumbadores piezoeléctricos), esto puede causar leves clics. Esto es normal y no indica un problema; simplemente significa que la prueba de hardware se está ejecutando.

### P: ¿Puede la prueba ultrasónica de hardware (paso 2) detectar altavoces del sistema rotos (circuito abierto) o desconectados?
**R:** Esto no se ha probado actualmente y se desconoce. Si bien la prueba verifica la retroalimentación eléctrica y la actividad del puerto, es posible que no distinga de forma fiable entre un altavoz físicamente presente pero roto (circuito abierto) o desconectado y uno que falta. Si el altavoz está completamente roto o desconectado (circuito abierto), la prueba puede dar un resultado falso, lo que indica que no se detecta ninguna salida funcional. Sin embargo, este comportamiento no está garantizado y puede depender del hardware específico y del modo de fallo. Si sospecha que el altavoz de su sistema no funciona, se recomienda realizar una inspección física o usar un multímetro.

### P: ¿Por qué no veo ninguna opción de altavoz del sistema ni de sonido de pitido en mi dispositivo ARM64?
**R:** En sistemas Windows ARM64, NeoBleeper deshabilita la configuración relacionada con el altavoz del sistema porque las plataformas ARM64 no admiten el acceso directo al hardware del altavoz del sistema. Todos los pitidos se reproducen a través de su dispositivo de salida de sonido habitual (altavoces o auriculares), y las opciones «Probar altavoz del sistema» y «Usar dispositivo de sonido para crear pitido» se ocultan automáticamente. Este comportamiento es intencional y no se trata de un error.

**Posibles actualizaciones futuras:**
Si las pruebas o el desarrollo futuros permiten que NeoBleeper detecte de forma fiable los altavoces del sistema dañados o desconectados mediante la prueba ultrasónica de hardware, estas preguntas frecuentes y la lógica de detección se actualizarán para reflejar dichas mejoras. Esté atento a los registros de cambios o a las nuevas versiones para obtener más información.

---

## 7. Obtener ayuda

- **Proporcione detalles del equipo y el entorno:** Al reportar problemas de detección de hardware o sonido, incluya detalles sobre su equipo (ordenador de escritorio/portátil, fabricante/modelo, sistema operativo) y cualquier hardware relevante.
- **Adjunte capturas de pantalla o cuadros de diálogo de error:** Las capturas de pantalla de los cuadros de diálogo de error o advertencia son muy útiles. Indique exactamente cuándo ocurrió el problema.
- **Incluya el archivo de registro:** A partir de las versiones más recientes, NeoBleeper crea un archivo de registro detallado llamado `DebugLog.txt` en la carpeta del programa. Adjunte este archivo cuando busque ayuda, ya que contiene información de diagnóstico útil.
- **Describa los pasos para reproducir el problema:** Describa claramente qué estaba haciendo cuando ocurrió el problema.
- **Abra una incidencia en GitHub:** Para obtener más ayuda, abra una incidencia en GitHub e incluya todos los detalles anteriores para obtener el mejor soporte.

_Esta guía se actualiza a medida que se descubren nuevos problemas y soluciones. Para obtener más ayuda, abra un problema en GitHub con información detallada sobre su configuración y el problema encontrado._
