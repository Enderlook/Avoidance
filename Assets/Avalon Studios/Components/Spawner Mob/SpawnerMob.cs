using AvalonStudios.Additions.Attributes;
using Random = AvalonStudios.Additions.Utils.Random;
using AvalonStudios.Additions.Utils.HandlePoint;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvalonStudios.Additions.Components.SpawnerMobSystem
{
    public class SpawnerMob : MonoBehaviour
    {
        //Properties

        public HandlePoint GetPoint
        {
            get
            {
                if (point == null)
                    point = new HandlePoint();
                return point;
            }
        }

        public List<EnemyData> EnemiesData => enemiesData;

        // Variables

        [SerializeField]
        private Color pointColor = Color.red;

        [SerializeField]
        private float pointGizmoRadius = .5f;

        [SerializeField]
        private HandlePoint point = new HandlePoint();

        [SerializeField, Tooltip("The path of the enemies prefabs.")]
        private string path = "Assets";

        [SerializeField]
        private float timeToStartSpawn = 0;

        [SerializeField]
        private float timeBtwSpawn = 0;

        [field: SerializeField, IsProperty]
        public bool UseProbability { get; private set; } = false;

        [SerializeField]
        private int amountEnemiesToSpawn = 0;

        [SerializeField]
        private List<EnemyData> enemiesData = new List<EnemyData>();

        private int enemiesToSpawn;
        private int enemiesInGame = 0;

        public void CreatePointSpawn(bool reset = false) => point.Initialize(transform.position, 1, reset);

        public void ResetPoint() => CreatePointSpawn(true);

        public void Remove() => point.RemoveAllPoints();

        private void Awake()
        {
            enemiesToSpawn = UseProbability ? amountEnemiesToSpawn : enemiesData.Count;

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            if (enemiesToSpawn == enemiesInGame) yield break;

            yield return new WaitForSeconds(timeToStartSpawn);

            for (int x = 0; x < enemiesToSpawn; x++)
            {
                int p = UnityEngine.Random.Range(0, point.CountOfPoints);
                IEnumerable<EnemyData> enemyTypes = enemiesData.Where(t => t.Type == EnemyType.Mob);
                IEnumerable<int> enumerableProbability = enemyTypes.Select(f => f.Probability);
                GameObject enemy = Random.CumulativeProbability(enemyTypes.ToList(), enumerableProbability.ToList()).Prefab;
                SpawnEnemy(enemy, point[p]);
                enemiesInGame++;
                yield return new WaitForSeconds(timeBtwSpawn);
            }
        }

        private void SpawnEnemy(GameObject gameObject, Vector3 position)
        {
            GameObject enemy = Instantiate(gameObject, position, Quaternion.identity);


        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = pointColor;
            if (point.CountOfPoints != 0)
            {
                Vector3[] points = point.GetPositionsPoints;
                foreach(Vector3 p in points)
                    Gizmos.DrawWireSphere(p, pointGizmoRadius);
            }
        }
#endif
    }
}
