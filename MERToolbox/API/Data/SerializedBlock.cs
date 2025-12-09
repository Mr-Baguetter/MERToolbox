using System.Collections.Generic;
using UnityEngine;
using MERToolbox.API.Enums;

namespace MERToolbox.API.Data
{
    public class SerializedBlock
    {
        public string Name { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public BlockTypes BlockType { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}