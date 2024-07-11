using System.Collections.Generic;

namespace Project.Scripts
{
    public class ProcessManager
    {
        public PlayerCounter PlayerData1;
        public PlayerCounter PlayerData2;

        public void InitPlayer(IPlayerData playerData1, IPlayerData playerData2)
        {
            PlayerData1 = new PlayerCounter(playerData1);
            PlayerData2 = new PlayerCounter(playerData2);
        }

        public void Process()
        {
            while (PlayerData1.Hp > 0 && PlayerData2.Hp > 0)
            {
                
            }
        }
    }
}