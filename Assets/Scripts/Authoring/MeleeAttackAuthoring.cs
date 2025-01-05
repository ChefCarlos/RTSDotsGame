using Unity.Entities;
using UnityEngine;

public class MeleeAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int meleeAttackDamage;
    public float colliderSize;

    public class Baker : Baker<MeleeAttackAuthoring>
    {
        public override void Bake(MeleeAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAttack
            {
                timerMax = authoring.timerMax,
                meleeAttackDamage = authoring.meleeAttackDamage,
                colliderSize = authoring.colliderSize
            });
        }
    }

}

public struct MeleeAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public int meleeAttackDamage;
    public float colliderSize;
}