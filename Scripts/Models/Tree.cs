using System;
using UnityEngine;
namespace MilkSpun.CubeWorld.Models
{
    [System.Serializable]
    public class Tree:IEquatable<Tree>
    {
        public readonly Vector3 position;
        public int height;

        public Tree(Vector3 position)
        {
            this.position = position;
            height = 3;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Tree other && Equals(other);
        }

        public bool Equals(Tree other)
        {
            return other != null && position.Equals(other.position);
        }
    }
}
