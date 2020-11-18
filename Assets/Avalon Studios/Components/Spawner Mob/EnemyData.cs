using AvalonStudios.Additions.Attributes;

using UnityEngine;

namespace AvalonStudios.Additions.Components.SpawnerMobSystem
{
    [System.Serializable]
    public class EnemyData
    {
        [field: SerializeField, Tooltip("Type of enemy."), IsProperty]
        public EnemyType Type { get; set; } = EnemyType.Mob;

        [field: SerializeField, Tooltip("Prefab to Spawn."), IsProperty]
        public GameObject Prefab { get; set; } = null;

        [field: SerializeField, Tooltip("Use probability?"), IsProperty]
        public bool UseProbability { get; set; } = false;

        [field: SerializeField, Tooltip("Probability to be spawn."), ShowInInspectorIf(nameof(UseProbability), true), IsProperty]
        public int Probability { get; private set; } = 0;
    }

}
