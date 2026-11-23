using UnityEngine;
using UnityEngine.Events;

public static class CharacterEvents
{
    // Evento cuando el personaje recibe daño
    public static UnityAction<GameObject, int> characterDamaged;

    // Evento cuando el personaje es curado
    public static UnityAction<GameObject, int> characterHealed;
}
