using GameDevTV.Utils;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour 
    {

        public float mana
        {
            get {return _mana.value;}
            set {_mana.value=value;}
        }

        LazyValue<float> _mana;


        BaseStats stats;

        private void Awake() {
            stats = GetComponent<BaseStats>();
            _mana = new LazyValue<float>(GetMaxMana);
        }

        private void Start() 
        {
            _mana.ForceInit();
        }


        public float GetMaxMana()
        {
            return stats.GetCharacterStat(CharacterStat.Mana);
        }

        public float GetManaRegen()
        {
            return stats.GetCharacterStat(CharacterStat.ManaRegenRate);
        }

        private void Update()
        {
            if(mana < GetMaxMana())
            {
                mana += GetManaRegen() * Time.deltaTime;
                if(mana > GetMaxMana())
                {
                    mana = GetMaxMana();
                }
            }
        }
        
        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana)
            {
                return false;
            }
            mana -= manaToUse;
            return true;
        }
    }
}