using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Thiruvizha.Player.States
{
    public abstract class PlayerBaseState
    {
        public abstract void EnterState(Thiruvizha.Player.PlayerStateContext player);
        public abstract void UpdateState(Thiruvizha.Player.PlayerStateContext player);
        public abstract void EndState(Thiruvizha.Player.PlayerStateContext player);

    }
}
