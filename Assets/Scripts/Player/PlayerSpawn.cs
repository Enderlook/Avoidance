using Avoidance.Scene;

using Enderlook.Enumerables;
using Enderlook.Unity.Navigation;
using Enderlook.Unity.Navigation.D2;

using UnityEngine;

namespace Avoidance.Player
{
    [DefaultExecutionOrder(1)]
    public class PlayerSpawn : MonoBehaviour
    {
        private void Awake()
        {
            // Spawn Character with position and rotation
            Node node = ((IGraphAtoms<Node, Edge>)MazeGenerator.Graph).Nodes.RandomPick();
            Vector3 position = node.Position;
            transform.position = new Vector3(position.x, 0, position.y);

            // TODO: This rotation doesn't work fine...
            Vector3 oldRotation = transform.rotation.eulerAngles;
            Vector2 to = node.Edges.RandomPick().To.Position;
            transform.LookAt(new Vector3(to.x, .5f, to.y));
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(oldRotation.x, eulerAngles.y, oldRotation.z));

            Destroy(this);
        }
    }
}