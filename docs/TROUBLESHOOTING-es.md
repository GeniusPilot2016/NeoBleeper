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

  ![image1](https://github.com/user-attachments/assets/7e58e585-e4d3-4d27-91c6-2ce33a5efce8)
    
    **El altavoz del sistema no emite ningún pitido o emite un pitido diferente. No se realizó ninguna acción.**  
    Este mensaje aparece cuando la utilidad revisa el altavoz del sistema y determina que no emite ningún pitido o que emite un pitido que la herramienta no puede controlar. En este caso, Beep Stopper no realizará ninguna otra acción.
    - *Consejo:* Si el pitido persiste, intente reiniciar el ordenador.

  ![image2](https://github.com/user-attachments/assets/db7e26dc-934f-43e4-b74f-3fd009d647a3)
    
    **¡El pitido se detuvo correctamente!**  
    Este mensaje confirma que la utilidad Beep Stopper detectó un pitido atascado y logró detenerlo correctamente. No se requiere ninguna acción adicional.

  ![image3](https://github.com/user-attachments/assets/460c876a-67a1-46da-ab71-2f88941ee59c)
  
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

![image2](https://github.com/user-attachments/assets/baf0fe49-0867-4bfc-a2cd-235bcedec924)

![image3](https://github.com/user-attachments/assets/eb9f518e-d79f-4899-994d-383f618a8f0c)

- **Disponibilidad del botón "Probar altavoz del sistema":**
  Esta opción se activa si NeoBleeper detecta alguna salida de altavoz del sistema utilizable, incluidas las salidas ocultas o que no sean PNP0800.
- **Ajuste "Usar dispositivo de sonido para crear pitido":**
  Ahora puede desactivar esta función si se detecta una salida de altavoz del sistema oculta o no estándar.

#### ¿Qué significa "salida de altavoz del sistema no estándar"?
Algunas computadoras, portátiles o máquinas virtuales modernas no tienen un altavoz de PC real o el enrutamiento de la señal no es estándar. NeoBleeper ahora intenta detectar y utilizar estas salidas de altavoz del sistema ocultas (no identificadas como dispositivos PNP0800), pero solo puede habilitar la opción de altavoz del sistema si es realmente accesible a nivel de hardware. Si no se encuentra ninguna salida utilizable, deberá usar su dispositivo de sonido habitual.

## 2.1 Prueba de salida del altavoz del sistema (detección de frecuencia ultrasónica)

NeoBleeper ahora incluye una nueva prueba avanzada de hardware para detectar la salida del altavoz del sistema (también conocido como altavoz PC), incluso si el dispositivo no se reporta en Windows (por ejemplo, como PNP0C02 en vez de PNP0800) o si la conexión es no estándar.

- **Cómo funciona:**  
  Al iniciar, NeoBleeper realiza un segundo paso después de la comprobación habitual de la ID del dispositivo. Se envían señales ultrasónicas al puerto del altavoz del sistema y se monitoriza la respuesta del hardware para detectar la presencia de una salida de altavoz funcional.

- **Qué puedes notar:**  
  En algunos sistemas, **independientemente de si el altavoz es un zumbador piezoeléctrico o de otro tipo**, puedes escuchar **clics o chasquidos** durante esta etapa. Esto es normal e indica que la prueba de hardware está en progreso.

  ![image1](https://github.com/user-attachments/assets/ebcf9c20-1ea9-4ecb-b816-d3c0d16e2380)

  *Comprobando la presencia de salida del altavoz del sistema (altavoz PC) en el paso 2/2… (puede que escuches clics/chasquidos)*

- **¿Por qué esta prueba?**  
  Muchos sistemas modernos no tienen un dispositivo de altavoz del sistema PNP0800, pero sí una salida “oculta” utilizable. NeoBleeper utiliza este método avanzado para habilitar la función de beep en más hardware.
  
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

## 6. Advertencias, errores y solución de problemas de la API de Google Gemini™

La función "Crea música con IA" de NeoBleeper utiliza la API de Google Gemini™. Es posible que aparezcan mensajes de error o advertencias relacionados con la disponibilidad de la API, los límites de uso o las restricciones geográficas.

### 6.1 Errores de cuota o límite de frecuencia (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/de27c81c-f840-42b4-b568-e25d62188322)

**Posibles causas:**

- **Se ha agotado la cuota de la API de su cuenta.** Si utiliza una clave API gratuita, es posible que algunos modelos (como `gemini-2.0-pro-exp`) no estén disponibles o tengan límites de uso muy bajos o estrictos para las cuentas gratuitas.

- **Limitaciones del plan gratuito:** Algunos modelos generativos más recientes (como Gemini Pro Exp) *no* están disponibles para los usuarios del plan gratuito. Si intenta usarlos, se producirá un error de cuota o disponibilidad.

- **Límites de frecuencia excedidos:** Si envía demasiadas solicitudes con demasiada rapidez, puede alcanzar los límites de frecuencia de la API, incluso con un plan de pago.

**Cómo solucionarlo:**
- **Verifique su cuota y facturación de la API:** Inicie sesión en su cuenta de Google Cloud/Gemini para verificar su uso y actualizar su plan si es necesario.

- **Use solo modelos compatibles:** Los usuarios del plan gratuito pueden tener limitaciones con respecto a ciertos modelos. Consulte la documentación para ver los modelos disponibles o cambie a uno compatible.

- **Espere y vuelva a intentarlo más tarde:** A veces, esperar unos instantes permite que la cuota se actualice temporalmente, como indica la cuenta regresiva del mensaje.

- **Consulta la [documentación de la API de Gemini](https://ai.google.dev/gemini-api/docs/rate-limits) para conocer las políticas de uso y los límites de frecuencia más recientes.**

---

### 6.2 Solución de problemas para modelos Gemini muy nuevos o no documentados (p. ej., Gemini 3 Pro Preview)

Algunos modelos Gemini, especialmente los lanzamientos más recientes como **Gemini 3 Pro Preview**, podrían no aparecer en la documentación oficial de precios o cuotas de la API de Gemini en el momento de su lanzamiento. Es posible que se produzcan errores de cuota, acceso o "RESOURCE_EXHAUSTED" incluso si la cuota total de su cuenta parece estar sin usar.

**Consideraciones importantes para los modelos muy nuevos:**
- Google suele limitar el acceso a los modelos de vista previa (como Gemini 3 Pro Preview) a cuentas o regiones específicas y puede imponer límites de solicitud y uso mucho más estrictos.

- Las cuentas gratuitas podrían tener cuota cero para estos modelos o las solicitudes podrían estar bloqueadas por completo.

- Es posible que el modelo no esté visible en las pestañas de cuota/precios ni en la documentación de Google durante varias semanas después de su lanzamiento.

- Los precios, el acceso y la disponibilidad de los nuevos modelos Gemini pueden cambiar con frecuencia.

**Qué hacer si encuentra errores:**
- Verifique su [uso de la API y cuotas](https://ai.dev/usage?tab=rate-limit) y si el nuevo modelo aparece en su consola.

- Revise la [documentación de la API de Gemini](https://ai.google.dev/gemini-api/docs/rate-limits), pero tenga en cuenta que la documentación puede estar desactualizada con respecto a los modelos recién lanzados.

- Si ve errores como "RESOURCE_EXHAUSTED" para un modelo que no está documentado en las tablas de precios oficiales, probablemente significa que el modelo aún no está disponible para el público general o que el acceso a la vista previa es muy restringido.

- Espere a que Google actualice su documentación y a que se implemente de forma generalizada si necesita usar estos modelos experimentales.

> **Nota:**
> NeoBleeper y aplicaciones similares no pueden eludir estas limitaciones. Si su cuenta o región no cumple con los requisitos, debe esperar hasta que Google habilite oficialmente el acceso o aumente la cuota para el modelo de Gemini que haya elegido.

---

### 6.3 Restricciones regionales o por país

#### "La API no está disponible en tu país"

![image4](https://github.com/user-attachments/assets/bdcb35e3-ddca-408f-a42e-25879a74f2fb)

Algunas regiones no son compatibles con la API de Google Gemini™ debido a restricciones regionales o legales.

**Posibles motivos:**

- Tu país es uno de los que tiene acceso restringido a la API de Gemini.

- La clave de API que estás usando está registrada en una región que no tiene acceso.

**Cómo solucionarlo:**

- **Consulta la lista de países compatibles con la API de Google Gemini™** en la documentación oficial.

- Si te encuentras en un país con restricciones, las funciones de IA no estarán disponibles.

#### Advertencia específica por región (Panel de configuración)

![image3](https://github.com/user-attachments/assets/769d1e90-70dc-4a4e-9c50-a0849eefa02c)

En el Espacio Económico Europeo, Suiza o el Reino Unido, la API de Gemini™ puede requerir una cuenta de Google de pago (no gratuita).

- Si ves esta advertencia, asegúrate de haber actualizado tu plan de la API de Gemini antes de intentar usar las funciones de IA.

--

### 6.4 Consejos generales sobre la API de IA

- Introduce solo tu propia clave de API; no la compartas por tu seguridad.

- NeoBleeper no transmite tu clave de API, excepto directamente al servicio Gemini cuando sea necesario para el uso de las funciones.

Si experimenta errores repetidos, intente eliminar y volver a agregar su clave API y verifique que esté activa.

---

## 7. Altavoz del sistema y consejos de sonido para chipsets específicos (incluido Intel B660)

### Si no escucha ningún sonido, el sonido está distorsionado o el altavoz del sistema no funciona correctamente:

Algunos chipsets modernos, incluidos los de la serie Intel B660 y posteriores, pueden tener problemas al inicializar o reinicializar el altavoz del sistema (aviso acústico del PC), lo que provoca silencio o problemas de sonido.

**Consejos para los usuarios afectados:**

- **Intente poner el ordenador en modo de suspensión y volver a activarlo.**

Esto puede ayudar a reinicializar o restablecer el puerto de hardware de bajo nivel responsable del altavoz del sistema y restaurar la función de aviso acústico.

- **Utilice la función "Usar dispositivo de sonido para crear un aviso acústico"** como alternativa si la salida del altavoz del sistema no funciona correctamente.

- **Busque actualizaciones de BIOS o firmware:** Algunos fabricantes de placas base pueden publicar actualizaciones que mejoran la compatibilidad del puerto del altavoz.

**Específico para equipos de escritorio:** Si ha añadido, eliminado o reconectado el hardware de los altavoces del sistema, reinicie el equipo por completo.

_Esta solución alternativa se destaca en la configuración:_

![image2](https://github.com/user-attachments/assets/0b50e3a2-d22a-40b0-8ffa-a7fab0056054)

> *Si no escucha ningún sonido o el sonido está distorsionado, intente poner el equipo en modo de suspensión y reactivarlo. Esto puede ayudar a reinicializar los altavoces del sistema en los chipsets afectados.*

---

*Para cualquier problema relacionado con el sonido o la IA que no se trate aquí, incluya capturas de pantalla del error, detalles del hardware de su PC (especialmente la marca y el modelo de la placa base/chipset) y su país/región al solicitar asistencia o abrir un problema en GitHub.*

---

## 8. Preguntas frecuentes

### P: ¿Puedo usar el altavoz del sistema si mi hardware no tiene un dispositivo PNP0800?
**R:** ¡Sí! NeoBleeper ahora intenta detectar y usar salidas de altavoz del sistema ocultas o que no sean PNP0800 siempre que sea posible. Si lo logra, podrá usar el altavoz del sistema incluso si Windows no detecta un dispositivo estándar.

### P: ¿Por qué la configuración "Usar dispositivo de sonido para crear un pitido" a veces se vuelve permanente (en versiones anteriores)?
**R:** Cuando no se detecta una salida de altavoz del sistema estándar (en versiones anteriores), NeoBleeper aplica esta configuración para garantizar que la salida de sonido siga siendo posible.

### P: ¿Hay alguna solución alternativa si no se detecta el altavoz del sistema?
**R:** Debe usar su dispositivo de sonido habitual (altavoces/auriculares) si no se encuentra una salida de altavoz del sistema estándar (en versiones anteriores).

### P: ¿Qué pasa si la herramienta Beep Stopper no detiene el pitido bloqueado? **R:** Reinicie su computadora para restablecer el hardware de los altavoces si la utilidad Beep Stopper falla.

### P: ¿Por qué escucho clics y chasquidos al iniciar?
**R:** Durante la prueba avanzada de salida del altavoz del sistema (paso 2), NeoBleeper envía señales ultrasónicas al hardware para detectar salidas de altavoz ocultas o no estándar. **Independientemente de si tu sistema usa un zumbador piezoeléctrico o otro tipo de altavoz, puedes escuchar clics y chasquidos.** Tal como se muestra en la pantalla de inicio del programa, estos sonidos ocurren mientras NeoBleeper comprueba la presencia de salida del altavoz del sistema (altavoz PC) en el paso 2/2.

### P: ¿Puede la prueba ultrasónica de hardware (paso 2) detectar altavoces del sistema rotos (circuito abierto) o desconectados?
**R:** Esto no se ha probado actualmente y se desconoce. Si bien la prueba verifica la retroalimentación eléctrica y la actividad del puerto, es posible que no distinga de forma fiable entre un altavoz físicamente presente pero roto (circuito abierto) o desconectado y uno que falta. Si el altavoz está completamente roto o desconectado (circuito abierto), la prueba puede dar un resultado falso, lo que indica que no se detecta ninguna salida funcional. Sin embargo, este comportamiento no está garantizado y puede depender del hardware específico y del modo de fallo. Si sospecha que el altavoz de su sistema no funciona, se recomienda realizar una inspección física o usar un multímetro.

### P: ¿Por qué no veo ninguna opción de altavoz del sistema ni de sonido de pitido en mi dispositivo ARM64?
**R:** En sistemas Windows ARM64, NeoBleeper deshabilita la configuración relacionada con el altavoz del sistema porque las plataformas ARM64 no admiten el acceso directo al hardware del altavoz del sistema. Todos los pitidos se reproducen a través de su dispositivo de salida de sonido habitual (altavoces o auriculares), y las opciones «Probar altavoz del sistema» y «Usar dispositivo de sonido para crear pitido» se ocultan automáticamente. Este comportamiento es intencional y no se trata de un error.

### P: ¿Qué significa recibir la advertencia "Salida de altavoz del sistema no estándar"?
**R:** NeoBleeper ha detectado hardware de altavoz que no cumple con los estándares tradicionales de altavoces de PC (por ejemplo, no es un dispositivo PNP0800). Podría tratarse de una salida de altavoz "oculta" que se encuentra en ordenadores de escritorio modernos o máquinas virtuales. En estos casos, es posible que no todas las funciones de pitido funcionen correctamente, pero NeoBleeper intentará usar cualquier salida compatible que detecte.

### P: ¿Por qué aparece el botón "Probar altavoz del sistema" si Windows no muestra ningún dispositivo de altavoz en la lista?
**R:** NeoBleeper incluye lógica de detección para salidas de altavoz del sistema ocultas o no estándar. Si aparece el botón, significa que NeoBleeper ha encontrado un puerto de hardware potencial para la salida de altavoz, aunque Windows no lo identifique como un dispositivo.

### P: Estoy usando la API de Google Gemini™ para funciones de IA y veo un mensaje de "cuota agotada" o "API no disponible en tu país". ¿Qué debo hacer?
**R:** Consulta la sección 6 de esta guía. Asegúrate de que tu clave de API y tu facturación/cuota estén al día y de que tu uso cumpla con las restricciones regionales de Google. Si te encuentras en una región restringida, lamentablemente, es posible que las funciones de IA no estén disponibles.

### P: Tengo un sistema Intel B660 (o posterior) y el altavoz de mi PC a veces no funciona o se bloquea. ¿Es normal?
**R:** Algunos chipsets más recientes tienen problemas de compatibilidad conocidos al reiniciar el altavoz del sistema. Intenta poner tu ordenador en modo de suspensión y volver a activarlo, o usa tu dispositivo de sonido habitual. Busca actualizaciones de BIOS/firmware que puedan mejorar la compatibilidad con el altavoz.

### P: ¿Cuál es la mejor manera de informar sobre problemas de sonido o IA para obtener asistencia técnica?
**R:** Proporcione siempre la mayor cantidad de información posible: la marca y el modelo de su computadora, su región, capturas de pantalla de los cuadros de diálogo de error y su archivo `DebugLog.txt` de la carpeta de NeoBleeper. Para problemas con la IA, incluya el texto completo de los cuadros de diálogo de error y describa el estado de su cuenta de la API de Gemini.

### P: Después de un fallo o un cierre forzado, el Beep Stopper de NeoBleeper no detuvo un pitido continuo. ¿Hay otra forma de solucionarlo?
**R:** Si el Beep Stopper no funciona, reiniciar la computadora restablecerá el hardware del altavoz del sistema y detendrá cualquier pitido persistente.

### P: ¿Es seguro usar la utilidad Beep Stopper si veo un mensaje de advertencia sobre una salida de altavoz del sistema no estándar o faltante?
**R:** Sí, pero tenga en cuenta que la utilidad podría no ser capaz de controlar el hardware y, en raras ocasiones, podría causar inestabilidad o no tener ningún efecto. Si no está seguro, opte por no continuar y reinicie la computadora.

### P: En las máquinas virtuales, no consigo que funcione el altavoz del sistema. ¿Es un error del sistema?
**R:** No necesariamente. Muchas máquinas virtuales no emulan correctamente el altavoz de un PC o presentan la salida de audio de una forma que no se puede controlar mediante programación. Para obtener mejores resultados, utilice su dispositivo de salida de audio habitual.

**Posibles actualizaciones futuras:**
Si las pruebas o el desarrollo futuros permiten que NeoBleeper detecte de forma fiable los altavoces del sistema dañados o desconectados mediante la prueba ultrasónica de hardware, estas preguntas frecuentes y la lógica de detección se actualizarán para reflejar dichas mejoras. Esté atento a los registros de cambios o a las nuevas versiones para obtener más información.

---

## 9. Obtener ayuda

- **Proporcione detalles del equipo y el entorno:** Al reportar problemas de detección de hardware o sonido, incluya detalles sobre su equipo (ordenador de escritorio/portátil, fabricante/modelo, sistema operativo) y cualquier hardware relevante.
- **Adjunte capturas de pantalla o cuadros de diálogo de error:** Las capturas de pantalla de los cuadros de diálogo de error o advertencia son muy útiles. Indique exactamente cuándo ocurrió el problema.
- **Incluya el archivo de registro:** A partir de las versiones más recientes, NeoBleeper crea un archivo de registro detallado llamado `DebugLog.txt` en la carpeta del programa. Adjunte este archivo cuando busque ayuda, ya que contiene información de diagnóstico útil.
- **Describa los pasos para reproducir el problema:** Describa claramente qué estaba haciendo cuando ocurrió el problema.
- **Abra una incidencia en GitHub:** Para obtener más ayuda, abra una incidencia en GitHub e incluya todos los detalles anteriores para obtener el mejor soporte.

_Esta guía se actualiza a medida que se descubren nuevos problemas y soluciones. Para obtener más ayuda, abra un problema en GitHub con información detallada sobre su configuración y el problema encontrado._
