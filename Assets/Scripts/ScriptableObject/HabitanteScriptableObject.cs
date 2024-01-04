using UnityEngine;

namespace Proyecto.Habitantes.Scriptable
{
    [CreateAssetMenu(fileName = "HabitanteData", menuName = "Scriptable/Habitante")]
    public class HabitanteScriptableObject : ScriptableObject
    {
        public HabitanteScriptable habitanteScriptable;
    }
}