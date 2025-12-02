using Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public partial struct PlayerInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.AddComponentObject(state.SystemHandle, new InputActionsComponent
            {
                MoveActions = InputSystem.actions.FindAction("Move"),
                LookActions =  InputSystem.actions.FindAction("Look"),
            });
        }
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var playerInput in SystemAPI.Query<RefRW<PlayerInputComponent>>().WithAll<GhostOwnerIsLocal>())
            {
                playerInput.ValueRW = default;
                
                var actions = state.EntityManager.GetComponentObject<InputActionsComponent>(state.SystemHandle);
                var move = actions.MoveActions.ReadValue<Vector2>();
                var look = actions.LookActions.ReadValue<Vector2>();
                
                playerInput.ValueRW.MoveHorizontal = move.x;
                playerInput.ValueRW.MoveVertical = move.y;
                
                playerInput.ValueRW.LookHorizontal = look.x;
                playerInput.ValueRW.LookVertical = look.y;
                
                var keyboard = Keyboard.current;

                if (keyboard == null)
                    return;
                
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    playerInput.ValueRW.Fire.Set();
                }
            }
        }
    }
}