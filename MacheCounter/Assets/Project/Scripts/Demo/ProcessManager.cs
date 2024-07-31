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

        public void GetEffect(List<IEffect> effects);
        public void BeforeGameStart();
        public void GameOver();
    }
    public class ProcessManager
    {
        public int Round;

        /// <summary>
        /// 流程管道
        /// </summary>
        public List<IProcess> Processes = new();

        /// <summary>
        /// 生成待认领缓存效果
        /// </summary>
        public List<IEffect> Effects = new();

        public void Init(List<IProcess> processes)
        {
            Processes = processes;
        }
        
        private bool IsOver()
        {
            //todo: 可能会有其他条件，例如指定回合数内
            return Processes.Any(t => t.IsOver());
        }

        private void OnBeforeGameStart()
        {
            Round = 0;
            Processes.ForEach(p => p.BeforeGameStart());
        }

        public void GameStart()
        {
            OnBeforeGameStart();
            OnGameStart();
            OnGameOver();
        }

        private void OnGameStart()
        {
            //todo: 临时这样写，之后改为状态机
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
            Processes.ForEach(p => p.GameOver());
            //todo：临时使用
            PlayManager.Instance.GameOver();
        }

        #region ---------- 轮次流程 ----------

        private void OnRoundBefore()
        {
            Round++;
            foreach (var process in Processes)
            {
                process.GetEffect(Effects);
                process.RoundBefore();
                if (IsOver())
                    break;
            }
        }        
        private void OnRoundStart()
        {
            foreach (var process in Processes)
            {
                process.GetEffect(Effects);
                process.RoundStart();
                if (IsOver())
                    break;
            }
        }        
        private void OnRoundAfter()
        {
            foreach (var process in Processes)
            {
                process.GetEffect(Effects);
                process.RoundAfter();
                if (IsOver())
                    break;
            }
        }

        private void OnRoundOver()
        {
            foreach (var process in Processes)
            {
                process.GetEffect(Effects);
                process.RoundOver();
                if (IsOver())
                    break;
            }
        }

        /// <summary>
        /// 获取目标是各自的类型
        /// </summary>
        private void GetSelfEffect()
        {
            
        }

        #endregion ---------- 轮次流程 ----------
    }
}