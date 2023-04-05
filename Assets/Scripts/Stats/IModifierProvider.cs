using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers(CharacterStat stat);
        IEnumerable<float> GetPercentageModifiers(CharacterStat stat);
    }
}
