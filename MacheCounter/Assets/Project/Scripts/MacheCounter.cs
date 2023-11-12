using AppBase.Event;
using AppBase.Resource;

public class MacheCounter : Architecture<MacheCounter>
{
    protected override void Init()
    {
        RegisterSystem<IEventSystem>(new EventManager());
        RegisterSystem<IResourceSystem>(new ResourceManager());
    }
}
