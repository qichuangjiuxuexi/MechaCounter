using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class TerrainManager : IProcess
    {
        public void InitTerrainView(TerrainView view)
        {
            
        }

        public void RoundBefore()
        {
            
        }

        public void RoundStart()
        {
            
        }

        public void RoundAfter()
        {
            
        }

        public void RoundOver()
        {
            
        }

        public bool IsOver()
        {
            return false;
        }
    }

    public class TerrainView : MonoBehaviour
    {
        /// <summary>
        /// 下标对应效果链
        /// </summary>
        private Dictionary<int, List<IEffect>> selfEffect = new();
    }
}