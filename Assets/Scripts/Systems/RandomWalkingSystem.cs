using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        foreach (var (localTransform, randomWalking, unitMover)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<RandomWalking>,
                RefRW<UnitMover>>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) <= UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                var random = randomWalking.ValueRO.random;
                var randomDirection = new float3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                randomDirection = math.normalize(randomDirection);
                randomWalking.ValueRW.random = random;
                randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
            }
            else
            {
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
            }
        }

    }
}
