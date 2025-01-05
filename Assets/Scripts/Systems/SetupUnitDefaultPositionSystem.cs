using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitDefaultPositionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var (localTransform, unitMover, setupUnitDefaultPosition, entity) in SystemAPI.Query<
            RefRO<LocalTransform>,
            RefRW<UnitMover>,
            RefRO<SetupUnitDefaultPosition>>()
            .WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

            entityCommandBuffer.RemoveComponent<SetupUnitDefaultPosition>(entity);
        }
    }
}
