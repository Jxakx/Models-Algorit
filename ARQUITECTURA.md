# Arquitectura del proyecto — Guía para la defensa

Este documento no es para el profesor, es para **vos**: un mapa de qué hace cada
pieza del código y por qué está diseñada así, para que puedas defenderla sin
tener que releer todo el proyecto de cero.

---

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
