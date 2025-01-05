using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collsitionWorld = physicsWorldSingleton.CollisionWorld;
        var raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

        foreach (var (localTransform, attack, target, unitMover) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MeleeAttack>, RefRW<Target>, RefRW<UnitMover>>().WithDisabled<MoveOverride>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float meleeAttackDistanceSq = 2f;
            var isCloseEnough = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < meleeAttackDistanceSq;

            var isTouchingTarget = false;
            if (!isCloseEnough)
            {
                var dirToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                dirToTarget = math.normalize(dirToTarget);
                var offset = 0.4f;
                var rayCastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + dirToTarget * (attack.ValueRO.colliderSize + offset),
                    Filter = CollisionFilter.Default
                };
                raycastHitList.Clear();
                if (collsitionWorld.CastRay(rayCastInput, ref raycastHitList))
                {
                    foreach (var raycastHit in raycastHitList)
                    {
                        if (raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            isTouchingTarget = true;
                            break;
                        }
                    }
                }
            }

            if (!isCloseEnough && !isTouchingTarget)
            {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                attack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (attack.ValueRO.timer > 0)
                {
                    continue;
                }

                attack.ValueRW.timer = attack.ValueRO.timerMax;

                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= attack.ValueRO.meleeAttackDamage;
                targetHealth.ValueRW.onHealthChanged = true;
            }
        }
    }
}
