using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    // esse código é usado para separar cada ambiente com um ID proprio, fizemos isso pois no começo
    // quando colocamos varios ambientes, quando um deles começava uma nova epoca, ele puxava a 
    // reinicialização de todos, com um id separado eles conseguem reiniciar individualmente.
    public int environmentID;
}
