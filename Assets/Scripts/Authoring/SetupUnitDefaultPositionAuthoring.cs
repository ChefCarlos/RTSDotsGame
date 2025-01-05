using Unity.Entities;
using UnityEngine;

public class SetupUnitDefaultPositionAuthoring : MonoBehaviour
{
    public class Baker : Baker<SetupUnitDefaultPositionAuthoring>
    {
        public override void Bake(SetupUnitDefaultPositionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SetupUnitDefaultPosition());
        }
    }
}

public struct SetupUnitDefaultPosition : IComponentData
{

}