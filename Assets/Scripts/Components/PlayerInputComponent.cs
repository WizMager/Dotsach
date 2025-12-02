using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInputComponent : IInputComponentData
    {
        public float MoveHorizontal;
        public float MoveVertical;
        
        public float LookHorizontal;
        public float LookVertical;

        public InputEvent Fire;
        public InputEvent Jump;
    }
}