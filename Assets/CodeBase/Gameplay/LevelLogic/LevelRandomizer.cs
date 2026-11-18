using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    /// <summary>
    /// Процедурно строит наполнение игровой сцены при каждой её загрузке.
    /// Сервисные объекты уровня, лежащие в контейнере <c>LevelLogic</c> (точка
    /// старта игрока, спавнеры врагов и астероидов, финиш, точка патруля,
    /// граница арены), остаются строго там, где расставлены в сцене — сервис
    /// только учитывает их позиции, чтобы наполнение не появлялось поверх. Всё
    /// остальное — бонусы и декор — расставляется случайно: из каталога
    /// <see cref="LevelContentRegistry"/> создаются планеты, гравитационные
    /// планеты, стены и патрульные корабли, поэтому два прохождения одного
    /// уровня никогда не совпадают, а сложность растёт вместе с числом
    /// требуемых убийств.
    /// </summary>
    public static class LevelRandomizer
    {
        private const int MaxPlacementAttempts = 32;
        private const float MinObjectDistance = 4.5f;
        private const float MinDistanceFromSpawn = 7f;

        // Процедурно создаваемое окружение.
        private const string GeneratedContentName = "Generated Level Content";

        // Контейнер сервисных объектов уровня: его содержимое расставлено
        // вручную и никогда не перемещается генератором.
        private const string LevelLogicContainerName = "LevelLogic";
        private const float PlanetMinScale = 0.2f;
        private const float PlanetMaxScale = 0.85f;
        private const float PlanetBackgroundOffset = 3f;
        private const float BackgroundPushBack = 20f;
        private const float GravityWellMinScale = 0.3f;
        private const float GravityWellMaxScale = 0.8f;
        private const float GravityWellMinRadius = 8f;
        private const float GravityWellMaxRadius = 12f;
        private const float PatrolZoneMinRadius = 6f;
        private const float PatrolZoneMaxRadius = 9f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Init();
        }

        /// <summary>
        /// Subscribes the randomizer to scene loading. Safe to call many
        /// times: the handler is never registered twice.
        /// </summary>
        public static void Init()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Randomize(scene);
        }

        /// <summary>
        /// Rearranges the common objects of the given scene and spawns the
        /// procedural content. Returns false for scenes that are not gameplay
        /// levels (for example the main menu).
        /// </summary>
        public static bool Randomize(Scene scene)
        {
            LevelBoundary boundary = LevelBoundary.Instance != null
                ? LevelBoundary.Instance
                : Object.FindObjectOfType<LevelBoundary>();

            if (boundary == null) return false;
            if (boundary.Radius <= 0f) return false;

            int difficulty = GetDifficulty();
            float gameplayZ = GetGameplayZ(boundary);
            Vector2 spawnCenter = GetSpawnCenter();

            // The star background shares the gameplay plane in the source
            // scene, so push it behind the generated planets first.
            PushBackgroundBack(gameplayZ);

            // Объекты под контейнером LevelLogic (спавнеры врагов и астероидов,
            // финиш, точка патруля) не перемещаются: они остаются там, где
            // расставлены в сцене. Их позиции только собираются, чтобы случайное
            // наполнение не появлялось поверх них.
            List<Vector2> usedPositions = CollectOccupiedPositions();

            int powerups = RandomizePowerups(boundary, spawnCenter, gameplayZ, usedPositions);
            int planets = RandomizePlanets(boundary, spawnCenter, gameplayZ, difficulty);
            int gravityWells = RandomizeGravityWells(boundary, spawnCenter, gameplayZ, difficulty, usedPositions);
            int walls = RandomizeHazardWalls(boundary, spawnCenter, gameplayZ, difficulty);
            int patrolShips = RandomizePatrolShips(boundary, spawnCenter, gameplayZ, difficulty, usedPositions);

            Debug.Log(string.Format(
                "[LevelRandomizer] {0}: difficulty={1}, power-ups={2}, planets={3}, gravity wells={4}, walls={5}, patrol ships={6}",
                scene.name, difficulty, powerups, planets, gravityWells, walls, patrolShips));

            return true;
        }

        private static int GetDifficulty()
        {
            LevelController controller = LevelController.Instance != null
                ? LevelController.Instance
                : Object.FindObjectOfType<LevelController>();

            if (controller == null || controller.Properties == null) return 1;

            return Mathf.Max(1, controller.Properties.RequiredKills);
        }

        /// <summary>
        /// Плоскость, на которой находятся игровые объекты (корабли, спавнеры,
        /// финиш). Сцена хранит часть объектов на нулевом Z, а часть — со
        /// смещением, поэтому генератор приводит всё к одной видимой плоскости.
        /// </summary>
        private static float GetGameplayZ(LevelBoundary boundary)
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            if (spawnPoint != null)
                return spawnPoint.transform.position.z;

            if (Player.Instance != null && Player.Instance.ActiveShip != null)
                return Player.Instance.ActiveShip.transform.position.z;

            return boundary.transform.position.z;
        }

        private static Vector2 GetSpawnCenter()
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            if (spawnPoint != null)
                return spawnPoint.transform.position;

            Player player = Object.FindObjectOfType<Player>();

            if (player != null)
                return player.transform.position;

            return Vector2.zero;
        }

        /// <summary>
        /// Собирает позиции объектов, которые расставлены в сцене вручную и не
        /// перемещаются: содержимое контейнера <c>LevelLogic</c> (спавнеры
        /// врагов и астероидов, финиш, точка патруля). Позиции нужны только для
        /// того, чтобы случайное наполнение не появлялось поверх них.
        /// </summary>
        private static List<Vector2> CollectOccupiedPositions()
        {
            List<Vector2> used = new List<Vector2>();

            AddPositions(used, Object.FindObjectsOfType<EntitySpawnerDebris>());
            AddPositions(used, Object.FindObjectsOfType<EntitySpawner>());
            AddPositions(used, Object.FindObjectsOfType<Powerup>());
            AddPositions(used, Object.FindObjectsOfType<LevelCompletionPosition>());
            AddPositions(used, Object.FindObjectsOfType<AIPointPatrol>());

            return used;
        }

        private static void AddPositions<T>(List<Vector2> used, T[] components) where T : Component
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null) continue;
                if (IsInsideLevelLogic(components[i].transform)) continue;

                used.Add(components[i].transform.position);
            }
        }

        /// <summary>
        /// Переставляет бонусы, лежащие вне контейнера <c>LevelLogic</c>, в
        /// случайные точки арены. Бонусы, добавленные вручную под LevelLogic,
        /// остаются на месте. Новые позиции добавляются в список занятых, чтобы
        /// случайный декор не появлялся поверх бонусов.
        /// </summary>
        private static int RandomizePowerups(LevelBoundary boundary, Vector2 spawnCenter, float gameplayZ, List<Vector2> used)
        {
            Powerup[] powerups = Object.FindObjectsOfType<Powerup>();
            int moved = 0;

            for (int i = 0; i < powerups.Length; i++)
            {
                Powerup powerup = powerups[i];
                if (powerup == null) continue;
                if (IsInsideLevelLogic(powerup.transform)) continue;

                Vector2 position = PickPosition(boundary, spawnCenter, 0.2f, 0.8f, used, MinObjectDistance);

                powerup.transform.position = new Vector3(position.x, position.y, gameplayZ);
                moved++;
            }

            return moved;
        }

        /// <summary>
        /// Проверяет, принадлежит ли объект сервисному контейнеру
        /// <c>LevelLogic</c>. Объекты под ним расставлены вручную и никогда не
        /// перемещаются генератором.
        /// </summary>
        private static bool IsInsideLevelLogic(Transform transform)
        {
            for (Transform current = transform; current != null; current = current.parent)
            {
                if (current.name == LevelLogicContainerName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Расставляет декоративные планеты на заднем плане. Количество растёт
        /// со сложностью, а размер выбирается случайно, поэтому подбираются
        /// разные спрайты планет.
        /// </summary>
        private static int RandomizePlanets(LevelBoundary boundary, Vector2 spawnCenter, float gameplayZ, int difficulty)
        {
            LevelContentRegistry registry = LevelContentRegistry.Instance;
            if (registry == null || registry.Planets == null) return 0;

            int targetCount = 5 + difficulty * 2;
            Transform container = GetOrCreateContainer();
            Vector2 center = boundary.transform.position;
            float radius = boundary.Radius;
            float z = gameplayZ + PlanetBackgroundOffset;

            int created = 0;

            for (int i = 0; i < targetCount; i++)
            {
                GameObject prefab = LevelContentRegistry.Pick(registry.Planets);
                if (prefab == null) break;

                Vector2 position = PickDecorPosition(center, radius, spawnCenter);

                GameObject planet = Object.Instantiate(
                    prefab,
                    new Vector3(position.x, position.y, z),
                    Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)),
                    container);

                planet.name = prefab.name;

                float scale = Random.Range(PlanetMinScale, PlanetMaxScale);
                SetPlanetScale(planet, scale);

                created++;
            }

            return created;
        }

        /// <summary>
        /// Создаёт гравитационные планеты и настраивает их уникальную механику.
        /// </summary>
        private static int RandomizeGravityWells(LevelBoundary boundary, Vector2 spawnCenter, float gameplayZ, int difficulty, List<Vector2> used)
        {
            LevelContentRegistry registry = LevelContentRegistry.Instance;
            if (registry == null || registry.GravityWells == null) return 0;

            int targetCount = Mathf.Clamp(difficulty, 1, 4);
            Transform container = GetOrCreateContainer();

            int created = 0;

            for (int i = 0; i < targetCount; i++)
            {
                GameObject prefab = LevelContentRegistry.Pick(registry.GravityWells);
                if (prefab == null) break;

                Vector2 position = PickPosition(boundary, spawnCenter, 0.45f, 0.8f, used, MinObjectDistance * 1.5f);

                GameObject well = Object.Instantiate(
                    prefab,
                    new Vector3(position.x, position.y, gameplayZ),
                    Quaternion.identity,
                    container);

                well.name = prefab.name;

                float scale = Random.Range(GravityWellMinScale, GravityWellMaxScale);
                SetPlanetScale(well, scale);

                GravityWell gravityWell = well.GetComponent<GravityWell>();

                if (gravityWell != null)
                {
                    float force = 8f + difficulty * 3f;
                    float radius = Random.Range(GravityWellMinRadius, GravityWellMaxRadius);
                    bool changeDirection = difficulty >= 2;

                    gravityWell.Configure(force, radius, changeDirection, 3f);
                }

                created++;
            }

            return created;
        }

        /// <summary>
        /// Ставит стены-препятствия по краю арены. Длина стены фиксирована
        /// префабом, поэтому меняется только положение и поворот.
        /// </summary>
        private static int RandomizeHazardWalls(LevelBoundary boundary, Vector2 spawnCenter, float gameplayZ, int difficulty)
        {
            LevelContentRegistry registry = LevelContentRegistry.Instance;
            if (registry == null || registry.Walls == null) return 0;

            int targetCount = Mathf.Clamp(difficulty, 1, 4);
            Transform container = GetOrCreateContainer();
            Vector2 center = boundary.transform.position;

            int created = 0;

            for (int i = 0; i < targetCount; i++)
            {
                GameObject prefab = LevelContentRegistry.Pick(registry.Walls);
                if (prefab == null) break;

                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(0.82f, 0.97f) * boundary.Radius;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 position = center + direction * distance;

                if (Vector2.Distance(position, spawnCenter) < MinDistanceFromSpawn) continue;

                // Ориентируем длинную сторону стены вдоль края арены: локальная
                // ось Y стены после поворота на угол совпадает с касательной.
                float rotation = angle * Mathf.Rad2Deg;

                GameObject wall = Object.Instantiate(
                    prefab,
                    new Vector3(position.x, position.y, gameplayZ),
                    Quaternion.Euler(0f, 0f, rotation),
                    container);

                wall.name = prefab.name;
                created++;
            }

            return created;
        }

        /// <summary>
        /// Создаёт патрульные корабли и привязывает каждого к собственной
        /// случайной точке патрулирования.
        /// </summary>
        private static int RandomizePatrolShips(LevelBoundary boundary, Vector2 spawnCenter, float gameplayZ, int difficulty, List<Vector2> used)
        {
            LevelContentRegistry registry = LevelContentRegistry.Instance;
            if (registry == null || registry.PatrolShips == null) return 0;

            int targetCount = Mathf.Clamp(difficulty, 1, 4);
            Transform container = GetOrCreateContainer();

            int created = 0;

            for (int i = 0; i < targetCount; i++)
            {
                GameObject prefab = LevelContentRegistry.Pick(registry.PatrolShips);
                if (prefab == null) break;

                Vector2 position = PickPosition(boundary, spawnCenter, 0.4f, 0.85f, used, MinObjectDistance * 1.5f);
                Vector3 worldPosition = new Vector3(position.x, position.y, gameplayZ);

                // Сохраняем ориентацию из префаба: корабль сам доворачивается
                // к точке патруля, но стартует в правильную сторону.
                // Корабль создаётся в корне сцены: его турели и снаряды ищут
                // свой корабль через transform.root, поэтому корабль не должен
                // лежать внутри общего контейнера сгенерированного контента.
                GameObject ship = Object.Instantiate(prefab, worldPosition, prefab.transform.rotation);
                ship.name = prefab.name;

                AIController controller = ship.GetComponent<AIController>();

                if (controller != null)
                {
                    float zoneRadius = Random.Range(PatrolZoneMinRadius, PatrolZoneMaxRadius);
                    AIPointPatrol patrolPoint = CreatePatrolPoint(container, worldPosition, zoneRadius);

                    controller.SetPatrolBehaviour(patrolPoint);
                }

                created++;
            }

            return created;
        }

        private static AIPointPatrol CreatePatrolPoint(Transform parent, Vector3 position, float radius)
        {
            GameObject pointObject = new GameObject("Patrol Point");
            pointObject.transform.SetParent(parent, false);
            pointObject.transform.position = position;

            AIPointPatrol point = pointObject.AddComponent<AIPointPatrol>();
            point.Configure(radius);

            return point;
        }

        /// <summary>
        /// Задаёт масштаб планеты и пересчитывает её спрайт: PlanetVisual
        /// выбирает картинку по размеру, а размер меняется уже после создания.
        /// </summary>
        private static void SetPlanetScale(GameObject planet, float scale)
        {
            planet.transform.localScale = new Vector3(scale, scale, scale);

            PlanetVisual visual = planet.GetComponentInChildren<PlanetVisual>();

            if (visual != null)
                visual.Refresh();
        }

        /// <summary>
        /// Отодвигает звёздный фон за игровую плоскость, чтобы процедурные
        /// планеты рисовались перед ним, а корабли — перед планетами. Фон
        /// следит за камерой только по X/Y, поэтому собственный Z сохраняется.
        /// </summary>
        private static void PushBackgroundBack(float gameplayZ)
        {
            SyncTransform[] syncTransforms = Object.FindObjectsOfType<SyncTransform>();
            float z = gameplayZ + BackgroundPushBack;

            for (int i = 0; i < syncTransforms.Length; i++)
            {
                SyncTransform background = syncTransforms[i];
                if (background == null) continue;

                Vector3 position = background.transform.position;
                background.transform.position = new Vector3(position.x, position.y, z);
            }
        }

        private static Transform GetOrCreateContainer()
        {
            GameObject container = GameObject.Find(GeneratedContentName);

            if (container == null)
            {
                container = new GameObject(GeneratedContentName);
                container.transform.position = Vector3.zero;
            }

            return container.transform;
        }

        private static Vector2 PickDecorPosition(Vector2 center, float radius, Vector2 spawnCenter)
        {
            for (int attempt = 0; attempt < MaxPlacementAttempts; attempt++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(0.05f, 1.1f) * radius;
                Vector2 candidate = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (Vector2.Distance(candidate, spawnCenter) < MinDistanceFromSpawn * 0.5f) continue;

                return candidate;
            }

            return center + Vector2.up * radius;
        }

        private static Vector2 PickPosition(
            LevelBoundary boundary,
            Vector2 spawnCenter,
            float minRadiusFactor,
            float maxRadiusFactor,
            List<Vector2> used,
            float minObjectDistance = MinObjectDistance)
        {
            Vector2 center = boundary.transform.position;
            float radius = boundary.Radius;

            Vector2 candidate = center + Vector2.up * radius * maxRadiusFactor;

            for (int attempt = 0; attempt < MaxPlacementAttempts; attempt++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(minRadiusFactor, maxRadiusFactor) * radius;
                candidate = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (Vector2.Distance(candidate, spawnCenter) < MinDistanceFromSpawn) continue;
                if (IsTooClose(candidate, used, minObjectDistance)) continue;

                break;
            }

            used.Add(candidate);
            return candidate;
        }

        private static bool IsTooClose(Vector2 position, List<Vector2> used, float minDistance)
        {
            for (int i = 0; i < used.Count; i++)
            {
                if (Vector2.Distance(position, used[i]) < minDistance)
                    return true;
            }

            return false;
        }
    }
}
