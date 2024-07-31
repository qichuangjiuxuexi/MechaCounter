using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class PlayManager
    {
        public ProcessManager ProcessManager;
        public PlayerCounterManager PlayerCounterManager;
        public TerrainManager TerrainManager;

        private static PlayManager instance;

        public static PlayManager Instance => instance;

        public void Init()
        {
            if (instance != null)
                instance = this;
            TerrainManager = new TerrainManager();
            PlayerCounterManager = new PlayerCounterManager();
            ProcessManager = new ProcessManager();
            ChildrenInit();
        }

        private void ChildrenInit()
        {
            //TerrainManager.Init();
            //PlayerCounterManager.Init();
            //流程管道
            List<IProcess> processes = new()
            {
                TerrainManager,
                PlayerCounterManager
            };
            ProcessManager.Init(processes);
        }

        public void GameStart()
        {
            ProcessManager.GameStart();
        }

        public void GameOver()
        {
            
        }
    }
}