using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    public interface IBaseBuilding
    {
        void Interact(Thiruvizha.NPC.NPCStateContext npc);
    }
}
