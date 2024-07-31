using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class DemoMain : MonoBehaviour
    {
        private PlayManager playManager;
        private void Awake()
        {
            playManager = new PlayManager();
        }

        private void Start()
        {
            playManager.Init();
            playManager.GameStart();
        }
    }
}