using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var cameraForward = Vector3.zero;
        if (Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }

        foreach (var (localTransform, healthBar) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
        {
            var parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
            if (localTransform.ValueRO.Scale == 1f)
            {
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }


            var health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

            if (!health.onHealthChanged)
            {
                continue;
            }

            float healthNormalized = (float)health.healthAmount / health.healthAmountMax;

            if (healthNormalized == 1f)
            {
                localTransform.ValueRW.Scale = 0f;
            }
            else
            {
                localTransform.ValueRW.Scale = 1f;
            }

            var barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}
