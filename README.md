# Tank Guys Multiplayer

Proyecto multijugador desarrollado en Unity con una arquitectura de red personalizada, diseñada bajo principios SOLID y orientada a la escalabilidad hacia un modelo de servidor dedicado.

---

## 1. Descripción general

El objetivo del proyecto es implementar un sistema multijugador desde cero que permita la comunicación entre múltiples clientes bajo un modelo **Player Host** (un jugador actúa como servidor), con la posibilidad de evolucionar hacia un servidor dedicado sin requerir una reestructuración completa del código.

El juego consiste en un sistema de combate entre tanques en una arena cerrada, donde los jugadores compiten hasta que solo uno permanece con vidas disponibles.

---

## 2. Arquitectura del sistema

El sistema está organizado en capas con responsabilidades claramente definidas:

- **GameClient / GameServer / HostNetwork**: Coordinación de la comunicación de red.
- **GameLogic**: Encapsulación de la lógica del juego (movimiento, daño, reglas).
- **GameState**: Representación del estado compartido del juego.
- **Handlers de mensajes**: Procesamiento desacoplado de los mensajes de red.
- **Transport Layer (TCP)**: Abstracción de la infraestructura de red.
- **Systems (ej: PingSystem)**: Lógica transversal desacoplada del transporte y la UI.
- **UI Layer**: Representación visual del estado del juego.

Esta organización permite desacoplar completamente la lógica del juego del transporte de red y de la interfaz.

---

## 3. Principios de diseño aplicados

El proyecto implementa los siguientes principios:

- **Single Responsibility Principle (SRP)**: Cada componente tiene una única responsabilidad claramente definida.
- **Open/Closed Principle (OCP)**: El sistema permite extender el comportamiento mediante nuevos handlers sin modificar código existente.
- **Dependency Inversion Principle (DIP)**: Las capas superiores dependen de abstracciones (`ITransport`) en lugar de implementaciones concretas.
- **Separación de dominio y transporte**: La lógica del juego no depende directamente de la implementación de red.
- **Separación de UI y lógica**: La interfaz solo refleja el estado, no contiene lógica del juego.

---

## 4. Sistema de red

El sistema de red se basa en:

- Comunicación mediante **TCP sockets**.
- Uso de mensajes tipados (`NetMessage`) para evitar dependencias en strings.
- Sistema de despacho de mensajes (`MessageDispatcher`).
- Separación explícita entre cliente y servidor.
- Modelo **host-authoritative** para eventos críticos (como el daño).

### Tipos de sincronización

1. **Input replication**
   - Movimiento
   - Rotación del tanque
   - Rotación de torreta
   - Disparo

2. **Eventos de estado**
   - Vidas del jugador
   - Estado (Alive / Spectator)
   - Fin de partida

3. **Eventos administrativos**
   - Kick
   - Ban
   - Pausa del juego

---

## 5. Sistema de ping (latencia)

Se implementó un sistema de medición de latencia basado en RTT (Round Trip Time):

### Flujo:

1. Cliente envía `PingMessage` con timestamp.
2. Servidor responde con `PongMessage`.
3. Cliente calcula RTT.
4. Cliente envía `PingReportMessage`.
5. Servidor hace broadcast del ping a todos los clientes.
6. Cada cliente actualiza su UI.

### Características:

- No depende del `GameState`.
- No afecta la lógica del juego.
- Se actualiza periódicamente (1 segundo).
- Permite visualizar la latencia de todos los jugadores.

---

## 6. Funcionalidades implementadas

Actualmente, el sistema incluye:

### Conectividad
- Creación de sala (host).
- Conexión de múltiples clientes.
- Rechazo de conexiones cuando la partida ya inició.
- Manejo de desconexiones sin reiniciar la partida.

### Gameplay
- Movimiento sincronizado.
- Rotación del tanque en 8 direcciones.
- Rotación independiente de torreta.
- Sistema de disparo en red.
- Detección de impacto (host-authoritative).
- Sistema de vidas (5 por jugador).
- Eliminación de jugadores (modo espectador).
- Detección de ganador.
- Fin de partida.

### UI
- Lobby dinámico con lista de jugadores.
- UI de partida con:
  - Nombre de jugadores
  - Ping en tiempo real
  - Indicador de vidas (corazones)
- Resaltado del jugador local.
- Panel de error para conexión rechazada.
- UI de pausa.
- UI de fin de partida.

### Funcionalidades administrativas (host)
- Expulsar jugadores (Kick).
- Banear jugadores por IP.
- Pausar y reanudar la partida.
- Visualización de ping de todos los jugadores.

---

## 7. Estructura de scripts

El proyecto está organizado por capas y responsabilidades, separando claramente red, lógica de juego y UI.

### Gameplay

Contiene toda la lógica del juego independiente de la red:

- **Arena**: Definición del espacio de juego (límites, entorno).
- **Combat**: Sistema de combate (disparos, daño, proyectiles).
- **Core**: Control principal del juego (`GameManager`, flujo de partida).
- **Player**: Componentes relacionados a los jugadores (`PlayerData`, controladores).
- **State**: Representación del estado del juego (`GameState`, estados de jugador).
- **Systems**: Sistemas transversales desacoplados (ej: `PingSystem`).

---

### Network

Encargado de toda la comunicación cliente-servidor:

- **Client**: Lógica específica del cliente.
- **Server**: Lógica específica del host (servidor).
- **Core**: Componentes centrales de red (`GameClient`, `MessageDispatcher`).
- **Runtime**: Ejecución de red en tiempo real (loops de conexión).
- **Handlers**:
  - **Client**: Procesamiento de mensajes recibidos por el cliente.
  - **Server**: Procesamiento de mensajes recibidos por el servidor.
- **Shared**:
  - **Messages**: Definición de mensajes serializables (`NetMessage`).
  - **State**: Estructuras compartidas entre cliente y servidor.
- **Transport**:
  - **TCP**: Implementación concreta del transporte.
  - **Interfaces**: Abstracciones (`ITransport`, `INetworkClient`).
  - **Common**: Utilidades compartidas de red.

---

### UI

Contiene toda la interfaz gráfica del juego:

- **Menu**: Interfaz del menú principal.
- **Lobby**: UI de la sala (lista de jugadores, acciones de host).
- **Game**: UI durante la partida (vidas, ping, estado).
- **Common**: Elementos reutilizables (paneles de error, mensajes).

---

### Notas de diseño

- La lógica de juego (`Gameplay`) no depende directamente de la red.
- Los mensajes (`Network/Shared/Messages`) son estructuras puras de datos.
- Los handlers desacoplan la recepción de mensajes de su procesamiento.
- La UI solo refleja el estado, sin contener lógica de red o gameplay.
- Los sistemas (como el ping) actúan como capa intermedia entre red y UI.

---

## 8. Flujo del juego

1. Jugador crea o se une a una sala.
2. Se sincroniza la lista de jugadores.
3. El host inicia la partida.
4. Los jugadores compiten en la arena.
5. Se eliminan jugadores al perder todas sus vidas.
6. Se detecta un ganador.
7. Se muestra pantalla de fin de partida.
8. Posibilidad de regresar al menú.

---

## 9. Escalabilidad

El sistema ha sido diseñado para permitir migración a servidor dedicado:

- `GameLogic` es independiente de Unity.
- `GameState` es reutilizable.
- El sistema de mensajes es extensible.
- El transporte TCP puede reemplazarse sin afectar la lógica.
- Los sistemas (como Ping) están desacoplados.

---

## 10. Limitaciones actuales

- No se implementa interpolación de movimiento.
- No hay predicción del cliente.
- El ping no tiene smoothing (puede fluctuar).
- Sin reconexión automática de jugadores.
- UI aún básica (sin animaciones).

---

## 11. Trabajo futuro

- Interpolación y extrapolación de movimiento.
- Client-side prediction.
- Sistema de snapshots.
- Migración a servidor dedicado.
- Mejora visual de UI.
- Optimización de red.

---

## 12. Tecnologías utilizadas

- Unity
- C#
- TCP Sockets