# Arquitectura del proyecto — Guía para la defensa

Este documento no es para el profesor, es para **vos**: un mapa de qué hace cada
pieza del código y por qué está diseñada así, para que puedas defenderla sin
tener que releer todo el proyecto de cero.

---

# PARCIAL 2 — Los 5 patrones nuevos

Se parte de la escena **MUNDO 1** (rama `parcial2`, código del Parcial 1). Los 5 patrones
elegidos están sobre cosas que ya estaban en el juego, no agregados "porque sí": cada uno
arregla algo que hoy era duplicado o rígido.

Cómo probar todo en el Editor: **Tools > Arena Survivor > Parcial 2 > Preparar escena MUNDO 1**
arma/cablea la escena (idempotente, se puede correr varias veces). En **Debug** hay atajos
solo para Play: alternar pausa, subir de nivel, desbloquear todas las armas, matar al
jugador (para ver el pantallazo azul) y resetear el idioma guardado.

## P2.1 Strategy — a quién le apunta cada arma y cómo se mueve cada enemigo

- **El problema:** "buscar el enemigo más cercano" estaba copiado en cinco lugares
  (`ProjectileWeapon`, `LaserWeapon`, `DroneWeapon`, `DroneMovement`, `ChainLightningWeapon`)
  y el chase de `Enemy.FixedUpdate` era un único comportamiento fijo.
- **Targeting** (`Weapons/Targeting/`): `ITargetingStrategy` (la abstracción) →
  `TargetingStrategy` (base ScriptableObject, trae el filtro común) → `NearestTargeting`,
  `FarthestTargeting`, `StrongestTargeting`, `RandomTargeting`. Las armas heredan de
  `TargetedWeapon` y piden `FindTarget(...)`; la estrategia concreta es un asset que se
  arrastra en el Inspector (Laser = el más fuerte, Dron = el más lejano, resto = el más cercano).
- **Movimiento** (`Enemies/Movement/`): `IMovementStrategy` → `MovementStrategy` (SO) →
  `ChaseMovement` (Basic), `ZigZagMovement` (Swarm), `ChargeMovement` (Tank). Se asigna en el
  `EnemyData`. `Enemy` solo llama `CalculateVelocity(MovementContext)`.
- **Por qué es Strategy y no un `switch`:** agregar un criterio nuevo es crear una clase/asset
  nuevo; no se toca ni `Enemy` ni ninguna arma (Open/Closed), y ambos dependen de la
  interfaz, no de la implementación (Dependency Inversion).
- **Detalle que suma:** las estrategias no guardan estado por enemigo — reciben todo en el
  `MovementContext` — así UN asset mueve a los 150 enemigos de un tipo.

## P2.2 Flyweight — datos compartidos de enemigos, proyectiles y números de daño

- **Roles del patrón:**
  - *Flyweight* (estado intrínseco, compartido y solo de lectura): `EnemyData`,
    `WeaponData` (lo comparten todos los proyectiles del arma), `DamageNumberStyle`.
  - *FlyweightFactory*: `Core/Flyweight/FlyweightFactory<T>` — entrega una única instancia
    compartida por clave (`GetOrCreate`) y cuenta cuántos contextos usan cada flyweight
    (`Acquire`/`Release`). `EnemyFactory` además guarda un pool por cada `EnemyData`.
  - *Contexto* (estado extrínseco, propio de cada objeto): `Enemy` (objetivo, vida actual,
    cooldown, empujón), `Projectile` (dirección, daño, cronómetro), `FloatingDamageNumber`
    (monto, cronómetro).
- **Lo que cambió:** antes `Enemy.Initialize` COPIABA vida/velocidad/daño/color a campos
  propios en cada enemigo. Ahora guarda solo una referencia al `EnemyData` compartido.
- **Cómo se demuestra en la defensa:** el panel verde de abajo a la izquierda
  (`FlyweightStatsUI`, F3 lo oculta) muestra en vivo "Enemigos vivos: 87 -> datos
  compartidos (EnemyData): 3". Es la prueba visible del ahorro.

## P2.3 Look-Up Table — sonidos, daño/cooldown por nivel y XP por nivel

- **Sonidos** (`Audio/SoundBank`): tabla `SoundId -> {clip, volumen}` (diccionario, acceso
  O(1)). Ningún script carga un audio por nombre ni por ruta: piden un `SoundId`. Los clips
  se arrastran en el Inspector del objeto **Audio** (15 filas, una por sonido). `SoundPlayer`
  traduce eventos del juego a `SoundId`s; `MusicPlayer` pide `SoundId.Music`. Si una fila no
  tiene clip, ese sonido simplemente no suena.
- **Daño/cooldown/proyectiles por nivel** (`WeaponData.levelTable`) y **XP por nivel**
  (`PlayerLeveling.xpPerLevel`): antes eran fórmulas que se recalculaban; ahora son tablas
  indexadas por nivel, editables fila por fila en el Inspector. Los parámetros viejos quedan
  como "generador" (menú contextual *Regenerar tabla*).

## P2.4 Decorator — efectos de golpe apilables por arma

- **El problema:** Láser y Cadena repetían "dañar + empujar + avisar impacto", y cada arma
  nueva con un efecto extra (ralentizar...) obligaba a copiar más código o a heredar.
- **Estructura** (`Weapons/HitEffects/`): `IHitEffect` (componente) → `DamageHitEffect`
  (componente concreto: solo daño) → `HitEffectDecorator` (base) → `KnockbackDecorator`,
  `SlowDecorator`, `ImpactSparksDecorator`.
- **Cómo se arma:** `WeaponBase.BuildHitEffect()` empieza por el daño y envuelve solo los
  efectos que el arma tiene configurados en el Inspector (empujón, ralentización, chispas).
  Ejemplo: el Láser = chispas(ralentizar(empujar(daño))).
- **Detalle:** el daño es siempre el más interno (el último en ejecutarse), así empujar y
  ralentizar se aplican antes de que el golpe mate al enemigo y el pool lo recicle.
- Los decoradores piden capacidades por interfaz (`IDamageable`, `IKnockbackable`,
  `ISlowable` — ISP): un objetivo que no sea ralentizable simplemente se saltea ese efecto.

## P2.5 Localization Manager — ES/EN en vivo

- `LocalizationManager` (corre primero, `DefaultExecutionOrder(-200)`) indexa la
  `LocalizationTable` (asset editable: clave -> español / inglés), recuerda el idioma en
  `PlayerPrefs` y publica `LanguageChangedEvent` cuando cambia (Observer).
- El código nunca escribe un texto a mano: pide una clave (`LocKeys`). Los textos fijos usan
  `LocalizedText`; los dinámicos (HUD, panel de nivel, BSOD) guardan su último dato y se
  redibujan al recibir el evento. `Localizer.Get(...)` es el acceso corto; si no hay manager
  devuelve la clave (el juego no se rompe).
- El botón **Idioma** del menú de pausa llama `Localizer.ToggleLanguage()`.
- Interfaz `ILocalizationService`: el resto del juego depende de ella, no de la clase concreta.

## Extras de esta entrega (no cuentan como patrón, pero conviene saber explicarlos)

- **Menú de pausa** (ESC): `PauseController` maneja solo el ESTADO (publica `GamePausedEvent` /
  `GameResumedEvent`); `PauseMenuUI` muestra el panel al recibirlos. ESC no pisa una pausa
  de otro sistema (panel de nivel o fin de partida).
- **Pantallazo azul** (`BsodUI`): reemplaza al panel de game over para la derrota; la
  victoria sigue en `GameEndUI`.
- **FX reciclados** de la escena anterior: partículas (muerte e impacto), números de daño,
  parpadeo al recibir daño y "pop" al aparecer.
- **Armas nuevas** (Láser, Cadena, Dron): se desbloquean desde el panel de nivel.

## Respuestas cortas para la defensa (Parcial 2)

- **¿Por qué estos 5?** Cada uno arregla algo real: duplicación de búsqueda de objetivos
  (Strategy), copias de datos por enemigo (Flyweight), fórmulas y campos sueltos de audio
  (Look-Up Table), efectos de golpe copiados (Decorator), textos fijos en español (Localization).
- **¿Y MVC / Memento / Builder / Screen Manager?** No se forzaron: con las pantallas actuales
  (nivel, pausa, fin) no justificaban el costo — mejor 5 bien aplicados que 9 a medias.
- **¿Sigue valiendo lo del Parcial 1?** Sí: Pool + Factory y Observer (EventManager) son la
  base sobre la que se apoyan los 5 nuevos (el Flyweight usa el pool, Localization y la
  pausa publican eventos, el sonido escucha eventos).

---

# PARCIAL 1 — Patrones y SOLID (base del proyecto)

## 1. Los patrones que pide la consigna

### Object Pool + Factory (4 pts)

**El problema que resuelven juntos:** en una arena survivor se crean y destruyen
centenares de objetos (enemigos, proyectiles, gemas de XP) todo el tiempo.
`Instantiate`/`Destroy` constantes son caros (CPU + Garbage Collector). El Pool
recicla instancias inactivas en vez de destruirlas; el Factory decide *qué*
instanciar/reciclar según los datos, para que el resto del código nunca llame
`Instantiate` a mano.

- **`Core/Pooling/ObjectPool.cs`** — la pieza genérica: `ObjectPool<T>` sirve
  para cualquier prefab con un componente `T`. `Get()` reusa una instancia
  inactiva o crea una nueva; `Release()` la devuelve al pool.
- **`Core/Pooling/IPoolable.cs`** — interfaz opcional (`OnSpawn`/`OnDespawn`)
  para que un objeto resetee su propio estado al ser reciclado.
- **Tres Factories concretas**, todas con la misma forma (constructor con el
  prefab + `Create(...)` que pide al pool y llama `Initialize`):
  - `Enemies/EnemyFactory.cs` → pool de `Enemy`
  - `Weapons/ProjectileFactory.cs` → pool de `Projectile`
  - `Progression/XpGemFactory.cs` → pool de `XpGem`

**Por qué 3 factories y no una sola genérica:** cada una inicializa su tipo de
forma distinta (`Enemy.Initialize(data, target, callback)` no tiene la misma
firma que `Projectile.Launch(pos, dir, speed, daño, vida)`). Forzarlas a una
interfaz común de "creación" no ganaría nada y complicaría la firma de cada una
sin necesidad — es la clase de abstracción que *no* conviene generalizar.

**Si te preguntan "¿por qué no usaste el `ObjectPool<T>` que trae Unity?"**:
porque el ejercicio pide *implementar* el patrón, no usar la librería que ya
lo resuelve. La versión propia deja ver exactamente cómo funciona.

### Observer + EventManager (4 pts)

- **`Core/Events/EventManager.cs`** — bus de eventos genérico y estático:
  `Subscribe<T>`, `Unsubscribe<T>`, `Publish<T>`, donde `T : IGameEvent`.
- **`Core/Events/IGameEvent.cs`** — interfaz marcadora vacía; solo sirve para
  que el compilador only acepte tipos pensados para esto.
- **Los eventos concretos viven junto a quien los declara**, no todos en un
  solo archivo gigante: `PlayerEvents.cs`, `EnemyEvents.cs`, `GameEvents.cs`.

Tabla rápida de quién publica y quién escucha (la vas a necesitar si te piden
"segui un evento de punta a punta"):

| Evento | Lo publica | Lo escuchan |
|---|---|---|
| `PlayerHealthChangedEvent` | `PlayerHealth` | `HealthBarUI` |
| `PlayerDiedEvent` | `PlayerHealth` | `GameManager`, `EnemySpawner` |
| `EnemyKilledEvent` | `Enemy` | `EnemySpawner`, `XpGemSpawner`, `GameManager` |
| `XpCollectedEvent` | `XpGem` | `PlayerLeveling` |
| `PlayerXpChangedEvent` | `PlayerLeveling` | `XpBarUI` |
| `PlayerLeveledUpEvent` | `PlayerLeveling` | `LevelUpManager` |
| `PlayerStatsChangedEvent` | `PlayerStats` | `PlayerStatsUI` |
| `GameTimeChangedEvent` / `KillCountChangedEvent` | `GameManager` | `GameTimerUI` / `KillCounterUI` |
| `GameOverEvent` / `GameWonEvent` | `GameManager` | `GameEndUI`, `EnemySpawner` |

**Por qué es Open/Closed de verdad:** para agregar `PlayerStatsChangedEvent`
(lo agregamos hoy) no tocamos ni una línea de `EventManager.cs` — solo creamos
el struct y lo usamos. Esa es la prueba concreta si te piden justificarlo.

---

## 2. Los 5 principios SOLID en este proyecto

### S — Single Responsibility

Cada clase clave tiene un único motivo de cambio:

- `Health` — solo matemática de vida (restar, sumar, invulnerabilidad). No
  sabe que existe una UI ni un EventManager.
- `PlayerHealth` — solo traduce los eventos *locales* de `Health` a eventos
  *globales*. Es la única pieza que sabe "este Health es el del jugador".
- `HealthBarUI` — solo pinta una barra a partir de un evento. No conoce a
  `Health` ni a `Player`.
- `EnemyFactory` — solo decide crear/reciclar. No sabe perseguir al jugador
  (eso es `Enemy`) ni cuándo spawnear (eso es `EnemySpawner`).

Esto es lo que hace que, por ejemplo, cambiar cómo se ve la barra de vida no
obligue a tocar `Health`, y cambiar el daño de contacto no obligue a tocar la UI.

### O — Open/Closed

Los tres puntos de extensión reales del proyecto:

1. **Eventos nuevos** sin tocar `EventManager` (ya explicado arriba).
2. **Armas nuevas**: heredar de `WeaponBase` e implementar `Fire()` (o, como
   `OrbitalWeapon`, ni siquiera eso). `WeaponBase` ya resuelve cooldown, nivel
   y daño con stats — la subclase solo decide *cómo* ataca.
3. **Tipos de enemigo/arma nuevos sin código**: `EnemyData` y `WeaponData` son
   `ScriptableObject`. Un "Enemigo Fantasma" nuevo es un asset con otros
   números, no una clase C# nueva. Ya lo hicimos 3 veces (Basic, Swarm, Tank)
   sin escribir una sola clase de enemigo distinta.

**Excepción honesta que hay que poder explicar:** `LevelUpManager.BuildChoicePool()`
tiene 3 mejoras de stat (velocidad, daño, vida) escritas a mano en el método,
en vez de ser datos. Es una decisión consciente: son solo 3, no van a crecer
mucho, y armar un `ScriptableObject` de "definición de mejora" para 3 casos
fijos habría sido complejidad de más sin beneficio real. Si en algún momento
hubiera 15 mejoras distintas, ahí sí convendría un enum o esa creación
sería un asset en vez de código.

### L — Liskov Substitution

`ProjectileWeapon` y `OrbitalWeapon` son ambas `WeaponBase`, y en ningún lugar
del código (ni `LevelUpManager`, ni nadie) se pregunta "¿sos un `ProjectileWeapon`
o un `OrbitalWeapon`?" — todo el mundo las trata como `WeaponBase` (leen
`Data.WeaponName`, `CurrentLevel`, `IsMaxLevel`, llaman `LevelUp()`, prenden o
apagan `enabled`). Cualquiera de las dos puede reemplazar a la otra en esa
lista sin romper nada — eso es Liskov cumplido, no solo herencia por herencia.

### I — Interface Segregation

Las dos interfaces del proyecto son deliberadamente chicas:

- `IDamageable` → `TakeDamage(float)` + `IsAlive`. Nada más. Un proyectil no
  necesita saber si golpeó a un `Health` de jugador o de enemigo, ni pedirle
  vida máxima ni eventos — solo necesita poder pegarle.
- `IPoolable` → `OnSpawn()` + `OnDespawn()`. Nada más.

Ninguna clase se ve forzada a implementar métodos que no usa (el típico
síntoma de violar ISP). Si hubiéramos hecho una sola interfaz gigante
`IGameEntity` con vida + pooling + movimiento + daño, `XpGem` habría tenido
que implementar métodos de "perseguir" que no le sirven de nada.

### D — Dependency Inversion

Los módulos de alto nivel dependen de abstracciones, no de clases concretas:

- `Enemy.TryDealContactDamage` y `Projectile.OnTriggerEnter2D` piden
  `IDamageable` por `TryGetComponent`, nunca `Health` directamente. Así,
  cualquier cosa "dañable" a futuro (una caja destructible, una torreta) no
  necesitaría cambiar ni una línea de `Enemy` o `Projectile`.
- `WeaponBase` depende de `WeaponData` (datos) para su comportamiento, no de
  números hardcodeados — el "qué tan fuerte es el arma" está invertido hacia
  un asset configurable, no atado a la clase.

---

## 3. Preguntas típicas de defensa (y respuesta corta)

**"¿Por qué `Health` no es una interfaz también?"**
Porque es una implementación concreta reusada tal cual (mismo comportamiento
para Player y Enemy) — no hace falta que existan variantes distintas de "cómo
sumar y restar vida". Una interfaz ahí sería abstracción sin un segundo caso
de uso real (regla general: no metas una interfaz si no tenés o vas a tener
más de una implementación).

**"¿Qué pasa si querés agregar un cuarto tipo de enemigo?"**
Creás un asset `EnemyData` nuevo con otros números (vida, velocidad, daño,
color, escala) y lo agregás a la lista `waveEntries` del `EnemySpawner` con su
tiempo de desbloqueo. Cero código nuevo.

**"¿Por qué el Pool es tuyo y no el de Unity (`UnityEngine.Pool.ObjectPool<T>`)?"**
Para poder explicar la implementación en la defensa — usar la clase de Unity
resolvería el problema pero no demostraría entender el patrón.

**"¿Dónde está el Observer, mostrámelo en un ejemplo concreto?"**
`Enemy` muere → publica `EnemyKilledEvent` → **tres** sistemas totalmente
distintos reaccionan sin que `Enemy` sepa que existen: `EnemySpawner` (baja el
contador de vivos), `XpGemSpawner` (larga una gema), `GameManager` (suma una
baja). Ese es el ejemplo más claro de "un publicador, varios observers
independientes".

**"¿Cumplís SOLID en todos lados, sin excepciones?"**
No, y decirlo así de entrada suena mejor que negarlo: `GameManager` junta
timer + contador de kills + condición de victoria/derrota en una sola clase
(podría ser 3). Se mantuvo junto porque las tres cosas son "estado general de
la partida" y separarlas en 3 componentes con 3 suscripciones hubiera sido más
archivos por separar algo bastante cohesivo. Saber señalar vos mismo este tipo
de trade-off (en vez de que te lo encuentren) suma mucho más en una defensa.
