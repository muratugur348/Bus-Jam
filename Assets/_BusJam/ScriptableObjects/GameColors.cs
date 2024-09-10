using UnityEngine;

//[CreateAssetMenu(fileName = "Game Color", menuName = "Game Color")]
public class GameColors : ScriptableObject
{
    public Color[] ActiveColors;
    public Color[] DeactiveColors;
    public Material[] ActiveMaterials;
    public Material[] DeactiveMaterials;
}