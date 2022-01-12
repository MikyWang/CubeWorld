using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkSpun.Common;
using MilkSpun.CubeWorld.Managers;
using MilkSpun.CubeWorld.Models;
using UnityEngine;

namespace MilkSpun.CubeWorld
{
    public class WorldRenderer : MonoBehaviour
    {
        public World World { get; set; }

        private void Start()
        {
            transform.position = World.Center;
            gameObject.name = World.ToString();
        }

    }
}
