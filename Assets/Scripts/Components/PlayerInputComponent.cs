using Unity.NetCode;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInputComponent : IInputComponentData
    {
        public float MoveHorizontal;
        public float MoveVertical;

        public InputEvent Fire;
        public InputEvent Jump;
    }
}