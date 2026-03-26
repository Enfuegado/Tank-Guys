# Tank Guys Multiplayer

Proyecto multijugador desarrollado en Unity con una arquitectura de red personalizada, diseñada bajo principios SOLID y orientada a la escalabilidad hacia un modelo de servidor dedicado.

---

## 1. Descripción general

El objetivo del proyecto es implementar un sistema multijugador desde cero que permita la comunicación entre múltiples clientes, inicialmente bajo un modelo de player host (host + cliente en la misma instancia), y que pueda evolucionar hacia un servidor dedicado sin requerir una reestructuración completa del código.

El juego consiste en un sistema de combate entre tanques en una arena cerrada, donde los jugadores compiten hasta que solo uno permanece con vidas disponibles.

---

## 2. Arquitectura del sistema

El sistema está organizado en capas con responsabilidades claramente definidas:

- **GameClient / GameServer / HostNetwork**: Coordinación de la comunicación de red.
- **GameLogic**: Encapsulación de la lógica del juego (movimiento, daño, reglas).
- **GameState**: Representación del estado compartido del juego.
- **Handlers de mensajes**: Procesamiento desacoplado de los mensajes de red.
- **Transport Layer (TCP)**: Abstracción de la infraestructura de red.
- **Sistema de sincronización por eventos**: Comunicación basada en mensajes tipados.

Esta organización permite desacoplar la lógica del juego del transporte de red, facilitando cambios futuros en el protocolo de comunicación.

---

## 3. Principios de diseño aplicados

El proyecto implementa los siguientes principios:

- **Single Responsibility Principle (SRP)**: Cada componente tiene una única responsabilidad claramente definida.
- **Open/Closed Principle (OCP)**: El sistema permite extender el comportamiento mediante nuevos handlers sin modificar código existente.
- **Dependency Inversion Principle (DIP)**: Las capas superiores dependen de abstracciones (`ITransport`) en lugar de implementaciones concretas.
- **Separación de dominio y transporte**: La lógica del juego no depende directamente de la implementación de red.

---

## 4. Sistema de red

El sistema de red se basa en:

- Comunicación mediante **TCP sockets**.
- Uso de mensajes tipados (`NetMessage`) para evitar dependencias en strings.
- Un sistema de despacho de mensajes (`MessageDispatcher`) que desacopla el procesamiento de los mismos.
- Separación explícita entre cliente y servidor.
- Modelo **host-authoritative** para eventos críticos como el daño.

Actualmente, la sincronización se realiza mediante:

1. **Eventos de entrada (input replication)**: movimiento, disparo y rotaciones.
2. **Eventos de estado**: actualización de vidas, estado del jugador y destrucción.

---

## 5. Estado actual del proyecto

Actualmente, el sistema permite:

- Creación de sala (host).
- Conexión de múltiples clientes.
- Sincronización de la lista de jugadores.
- Inicio de partida desde el host.
- Movimiento de jugadores sincronizado.
- Rotación del tanque en 8 direcciones.
- Rotación de torreta independiente sincronizada.
- Sistema de disparo en red.
- Detección de impacto y aplicación de daño (host-authoritative).
- Sistema de vidas (5 por jugador).
- Eliminación de jugadores al morir (modo espectador).
- Sincronización del estado de jugadores.
- Desconexión del host con retorno automático al menú.
- Limitación del mapa mediante arena circular.

---

## 6. Estructura del proyecto

```
Scripts
├── Core
├── Gameplay
│ ├── Core
│ ├── Player
│ ├── Combat
│ ├── State
│ └── Arena
├── Network
│ ├── Client
│ ├── Server
│ ├── Shared
│ ├── Handlers
│ ├── Transport
│ └── Runtime
└── UI
```


Esta estructura responde a la separación por responsabilidades del dominio del juego, evitando agrupaciones genéricas y facilitando la escalabilidad del sistema.

---

## 7. Escalabilidad

El sistema ha sido diseñado para permitir una transición hacia un modelo de servidor dedicado. En este escenario:

- El transporte TCP puede ser reemplazado sin modificar la lógica del juego.
- La lógica central (`GameLogic`) es independiente del motor de render.
- El sistema de mensajes permite migrar hacia sincronización por snapshot.
- Componentes como `GameState`, `GameLogic` y `PlayerManager` permanecen sin cambios.

Esto reduce significativamente el costo de migración.

---

## 8. Trabajo futuro

Las siguientes funcionalidades están planificadas:

- Sistema de fin de partida (detección de ganador).
- UI de victoria y retorno al menú.
- Sistema de pausa (host vs cliente).
- Interpolación de movimiento para suavizado visual.
- Predicción de cliente (opcional).
- Sincronización completa basada en snapshots.
- Migración a servidor dedicado.

---

## 9. Tecnologías utilizadas

- Unity
- C#
- TCP Sockets