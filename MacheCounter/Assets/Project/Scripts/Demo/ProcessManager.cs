using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts
{
    public interface IProcess
    {
        public void RoundBefore();

        public void RoundStart();
        public void RoundAfter();
        public void RoundOver();
        public bool IsOver();
    }
    public class ProcessManager
    {
        public int Round;

        private List<IProcess> processes = new();
        
        private TerrainManager terrainManager;
        private PlayerCounterManager playerCounterManager;

        public void OnInit()
        {
            Round = 0;
            terrainManager = new TerrainManager();
            processes.Add(terrainManager);
            playerCounterManager = new PlayerCounterManager();
            processes.Add(playerCounterManager);
        }
        
        private bool IsOver()
        {
            //todo: 可能会有其他条件，例如指定回合数内
            return processes.Any(t => t.IsOver());
        }

        private void OnBeforeGameStart()
        {
            
        }

        public void GameStart()
        {
            OnBeforeGameStart();
            OnGameStart();
            OnGameOver();
        }

        private void OnGameStart()
        {
            while (!IsOver())
            {
                OnRoundBefore();
                if (IsOver())
                    break;
                OnRoundStart();
                if (IsOver())
                    break;
                OnRoundAfter();
                if (IsOver())
                    break;
                OnRoundOver();
                if (IsOver())
                    break;
            }
        }

        private void OnGameOver()
        {
            
        }

        #region ---------- 轮次流程 ----------

        private void OnRoundBefore()
        {
            Round++;
            processes.ForEach(p => p.RoundBefore());
        }        
        private void OnRoundStart()
        {
            processes.ForEach(p => p.RoundStart());
        }        
        private void OnRoundAfter()
        {
            processes.ForEach(p => p.RoundAfter());
        }

        private void OnRoundOver()
        {
            processes.ForEach(p => p.RoundOver());
        }

        #endregion ---------- 轮次流程 ----------
    }
}