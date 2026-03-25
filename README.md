# Tank Guys Multiplayer

Proyecto multijugador desarrollado en Unity con una arquitectura de red personalizada, diseñada bajo principios SOLID y orientada a la escalabilidad hacia un modelo de servidor dedicado.

---

## 1. Descripción general

El objetivo del proyecto es implementar un sistema multijugador desde cero que permita la comunicación entre múltiples clientes, inicialmente bajo un modelo de player host (host + cliente en la misma instancia), y que pueda evolucionar hacia un servidor dedicado sin requerir una reestructuración completa del código.

---

## 2. Arquitectura del sistema

El sistema está organizado en capas con responsabilidades claramente definidas:

- **GameClient / GameServer**: Coordinación de la comunicación de red.
- **GameLogic**: Encapsulación de la lógica del juego.
- **GameState**: Representación del estado compartido del juego.
- **Handlers de mensajes**: Procesamiento desacoplado de los mensajes de red.
- **Transport Layer (TCP)**: Abstracción de la infraestructura de red.
- **Snapshot System**: Mecanismo alternativo de sincronización basado en estado completo.

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

Además, el sistema soporta dos enfoques de sincronización:

1. **Basado en mensajes** (actual): eventos discretos enviados entre cliente y servidor.
2. **Basado en estado (snapshot)**: actualización completa del estado del juego, preparada para futuras implementaciones.

---

## 5. Estado actual del proyecto

Actualmente, el sistema permite:

- Creación de sala (host).
- Conexión de múltiples clientes.
- Sincronización de la lista de jugadores.
- Inicio de partida desde el host.
- Manejo de desconexión y retorno al menú.
- Validación de la arquitectura en entorno funcional.

---

## 6. Estructura del proyecto
# Tank Guys Multiplayer

Proyecto multijugador desarrollado en Unity con una arquitectura de red personalizada, diseñada bajo principios SOLID y orientada a la escalabilidad hacia un modelo de servidor dedicado.

---

## 1. Descripción general

El objetivo del proyecto es implementar un sistema multijugador desde cero que permita la comunicación entre múltiples clientes, inicialmente bajo un modelo de player host (host + cliente en la misma instancia), y que pueda evolucionar hacia un servidor dedicado sin requerir una reestructuración completa del código.

---

## 2. Arquitectura del sistema

El sistema está organizado en capas con responsabilidades claramente definidas:

- **GameClient / GameServer**: Coordinación de la comunicación de red.
- **GameLogic**: Encapsulación de la lógica del juego.
- **GameState**: Representación del estado compartido del juego.
- **Handlers de mensajes**: Procesamiento desacoplado de los mensajes de red.
- **Transport Layer (TCP)**: Abstracción de la infraestructura de red.
- **Snapshot System**: Mecanismo alternativo de sincronización basado en estado completo.

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

Además, el sistema soporta dos enfoques de sincronización:

1. **Basado en mensajes** (actual): eventos discretos enviados entre cliente y servidor.
2. **Basado en estado (snapshot)**: actualización completa del estado del juego, preparada para futuras implementaciones.

---

## 5. Estado actual del proyecto

Actualmente, el sistema permite:

- Creación de sala (host).
- Conexión de múltiples clientes.
- Sincronización de la lista de jugadores.
- Inicio de partida desde el host.
- Manejo de desconexión y retorno al menú.
- Validación de la arquitectura en entorno funcional.

---

## 6. Estructura del proyecto
```
Scripts
├── Core
├── Gameplay
│ ├── Managers
│ └── Logic
├── Network
│ ├── Client
│ ├── Server
│ ├── Shared
│ ├── Handlers
│ ├── Transport
│ └── Runtime
└── UI
```

Esta estructura responde a la separación por responsabilidades (cliente, servidor, estado compartido, transporte), en lugar de agrupar únicamente por tipo de archivo.

---

## 7. Escalabilidad

El sistema ha sido diseñado para permitir una transición hacia un modelo de servidor dedicado. En este escenario:

- El transporte TCP puede ser reemplazado por HTTP sin modificar la lógica del juego.
- El sistema de sincronización por snapshot (`GameSnapshot`) permite trabajar con actualizaciones completas del estado.
- Componentes como `GameLogic`, `GameState` y `PlayerManager` permanecen sin cambios.

Esto reduce significativamente el costo de migración.

---

## 8. Trabajo futuro

Las siguientes funcionalidades están planificadas:

- Spawn de jugadores en escena.
- Sincronización de movimiento en tiempo real.
- Interpolación de movimiento para suavizado visual.
- Predicción de cliente (opcional).
- Implementación de servidor dedicado.

---

## 9. Tecnologías utilizadas

- Unity
- C#
- TCP Sockets
