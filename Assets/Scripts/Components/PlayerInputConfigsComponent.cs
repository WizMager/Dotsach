using Unity.Entities;

namespace Components
{
    public struct PlayerInputConfigsComponent : IComponentData
    {
        public float MoveSpeed;
        public float MouseSensitivity;
        public float MinVerticalAngle;
        public float MaxVerticalAngle;
        public float YCameraOffset;
    }
}