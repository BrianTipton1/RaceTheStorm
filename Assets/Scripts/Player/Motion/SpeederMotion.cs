using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{
    public class SpeederMotion
    {
        public int NFrames { get; set; }
        public List<List<Vector3>> positions { get; set; } = new();
        public List<List<Quaternion>> rotations { get; set; } = new();
        public List<List<Vector3>> scales { get; set; } = new();
    }
}
