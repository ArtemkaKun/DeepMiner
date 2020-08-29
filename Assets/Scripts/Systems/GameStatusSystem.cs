using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(MoveSystem))]
    public class GameStatusSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref PlayerComponent _) =>
            {
                if (UI.FuelBar.CurrentValue <= 0f)
                {
                    UI.onGameOver.Invoke();
                }
            });
        }
    }
}