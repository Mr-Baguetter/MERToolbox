using LabApi.Features.Wrappers;
using UnityEngine;

namespace MERToolbox.API.Components
{
    public class KillAreaDetector : MonoBehaviour
    {
        private Renderer Renderer;
        public void Init(GameObject obj)
        {
            Renderer = obj.GetComponent<Renderer>();
        }

        private void Update()
        {
            if (Renderer != null)
            {
                Bounds bounds = Renderer.bounds;
                foreach (Player player in Player.ReadyList)
                {
                    bool isInside = bounds.Contains(player.Position);
                    if (isInside)
                    {
                        Logger.Debug($"{player.DisplayName} is inside a KillArea");
                        player.Kill();
                    }
                }
            }
        }
    }
}