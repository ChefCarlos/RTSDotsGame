using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var (localTransform, zombieSpawner)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ZombieSpawner>>())
        {
            zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawner.ValueRO.timer > 0f)
            {
                // Timer not elapsed
                continue;
            }
            zombieSpawner.ValueRW.timer = zombieSpawner.ValueRO.timerMax;

            var zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefabEntity);

            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            entityCommandBuffer.AddComponent(zombieEntity, new RandomWalking
            {
                originPosition = localTransform.ValueRO.Position,
                targetPosition = localTransform.ValueRO.Position,
                distanceMin = zombieSpawner.ValueRO.randomWalkingDistanceMin,
                distanceMax = zombieSpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)zombieEntity.Index)
            });
        }
    }
}
