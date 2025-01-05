using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, moveOverride, moveOverrideEnabled, unitMover) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<MoveOverride>, EnabledRefRW<MoveOverride>, RefRW<UnitMover>>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                moveOverrideEnabled.ValueRW = false;
            }
        }
    }
}
