using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }

        foreach (var health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }

        foreach (var shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.onShoot.isTriggered = false;
        }
    }
}