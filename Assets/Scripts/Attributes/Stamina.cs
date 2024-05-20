using GameDevTV.Utils;
using RPG.Stats;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{

    public float stamina
    {
        get {return _stamina.value;}
        set {_stamina.value=value;}
    }

    LazyValue<float> _stamina;


    BaseStats stats;

    [SerializeField] float secondsUntilRecover = 3f;

    float lastUseCooldown = 0;
    float maxStamina = 100f;
    float recoveryRate = .5f;

    private void Awake() {
        stats = GetComponent<BaseStats>();
        _stamina = new LazyValue<float>(GetIntitialStamina);
    }

    private float GetIntitialStamina()
    {
        return maxStamina;
    }

    private void Start() 
    {
        _stamina.ForceInit();
    }

    public void RemoveStamina(float amountToRemove)
    {
        stamina = Mathf.Max(stamina - amountToRemove,0);
        Debug.Log(stamina);
        lastUseCooldown = secondsUntilRecover;
    }


    // Update is called once per frame
    void Update()
    {
        //if stamina has been recently used then tick the cooldown
        if(lastUseCooldown > 0)
        {
            lastUseCooldown = Mathf.Max(lastUseCooldown - Time.deltaTime,0);
        }
        //time until recovery elapsed, can begin to recover stamina.
        if(lastUseCooldown == 0 && stamina < maxStamina)
        {
            stamina = Mathf.Min(stamina + recoveryRate,maxStamina);
        }
    }
}
