using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

public static class RoomExtensions
{
    public static RoomIdentifier GetClosestRoomToPosition(Vector3 pos)
    {
        if (Room.List == null || Room.List.Count == 0)
            return null;

        RoomIdentifier closest = null;
        float closestSqrDist = float.MaxValue;

        foreach (Room room in Room.List)
        {
            if (room == null || room.Base == null)
                continue;

            float sqrDist = (room.Position - pos).sqrMagnitude;
            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest = room.Base;
            }
        }

        return closest;
    }
}
