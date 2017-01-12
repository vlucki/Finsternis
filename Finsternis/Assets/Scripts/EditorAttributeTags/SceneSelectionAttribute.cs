using UnityEngine;

public class SceneSelectionAttribute : PropertyAttribute
{
    public readonly bool allowDisabledScenes;
    public SceneSelectionAttribute(bool allowDisabledScenes = true)
    {
        this.allowDisabledScenes = allowDisabledScenes;
    }

}
