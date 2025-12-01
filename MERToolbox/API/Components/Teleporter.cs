using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MERToolbox.API.Data;
using UnityEngine;

namespace MERToolbox.API.Components
{
    public class Teleporter : MonoBehaviour
    {
        public TeleporterData Data;
        public Renderer Origin;
        public Renderer Target;
        public Dictionary<Player, DateTime> Times = [];
        public DateTime Time;

        public void Init(TeleporterData data, GameObject origin, GameObject target)
        {
            Data = data;
            Origin = origin.GetComponent<Renderer>();
            Target = target.GetComponent<Renderer>();
        }

        private void Update()
        {
            if (Origin != null && Target != null)
            {
                foreach (Player player in Player.ReadyList)
                {
                    if (!Data.AllowedRoles.Contains(player.Role))
                        continue;

                    if (Data.PerPlayerCooldown)
                    {
                        if (Times.TryGetValue(player, out DateTime lastTeleport))
                        {
                            if ((DateTime.Now - lastTeleport).TotalSeconds < Data.CoolDown)
                                continue;
                        }
                    }
                    else
                    {
                        if ((DateTime.Now - Time).TotalSeconds < Data.CoolDown)
                            continue;
                    }

                    if (Origin.bounds.Contains(player.Position))
                    {
                        if (Data.PerPlayerCooldown)
                            Times.Add(player, DateTime.Now);
                        
                        player.Position = Target.transform.position;
                    }

                    if (Target.bounds.Contains(player.Position))
                    {
                        if (Data.PerPlayerCooldown)
                            Times.Add(player, DateTime.Now);
                        
                        player.Position = Origin.transform.position;
                    }
                }
            }
        }
    }
}