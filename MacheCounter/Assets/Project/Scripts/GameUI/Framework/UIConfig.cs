using System.Collections.Generic;

public class UIConfig 
{
    public int viewId;
    public string pathFilename;
    public int layerType;
    public bool isLarge;

    public List<int> closeLayerList = new List<int>();

    public UIConfig(int viewId, string pathFilename, int layerType, bool isLarge = false, int[] closeLayerArray = null)
    {
        this.viewId = viewId;             //ui id
        this.pathFilename = pathFilename; //prifab 存储目录
        this.layerType = layerType;  //层级
        this.isLarge = isLarge;      //是否全屏
    }
}
