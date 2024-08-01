using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts
{
    public class TerrainManager : IProcess
    {
        private TerrainView View;
        public void Init(TerrainView view)
        {
            View = view;
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

        public void GetEffect(List<IEffect> effects)
        {
            if (effects is not { Count: > 0 }) return;
            //拿到目标是各自的效果并挂载到自身
            var selfEffects = effects
                .Where(e => e.Target == ETarget.Terrain).ToList();
            //todo: 对应地形获取自身的效果
            View.GetEffect(selfEffects);
            for (var i = selfEffects.Count - 1; i >= 0; i--)
            {
                var selfEffect = selfEffects[i];
                effects.Remove(selfEffect);
            }
        }

        public void BeforeGameStart()
        {
            
        }

        public void GameOver()
        {
            
        }
    }

    public class TerrainView : MonoBehaviour
    {
        /// <summary>
        /// 下标对应效果链
        /// </summary>
        private Dictionary<int, List<IEffect>> selfEffect = new();

        public void GetEffect(List<IEffect> effects)
        {
            foreach (var effect in effects)
            {
                if (selfEffect.TryGetValue(effect.TerrainIndex, out var effectList))
                {
                    if (effectList == null)
                    {
                        selfEffect[effect.TerrainIndex] = new List<IEffect>();
                        effectList = selfEffect[effect.TerrainIndex];
                    }
                    effectList.Add(effect);
                }
                else
                {
                    selfEffect.Add(effect.TerrainIndex, new List<IEffect>(){effect});
                }
            }
        }
    }
}