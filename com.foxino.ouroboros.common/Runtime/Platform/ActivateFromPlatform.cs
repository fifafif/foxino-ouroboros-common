using UnityEngine;

namespace Ouroboros.Common.Platform
{
public class ActivateFromPlatform : MonoBehaviour
{
    [SerializeField] private bool isHiddenInProd;
    [SerializeField] private bool isVisibleInEditor;

    private void Awake()
    {
#if OUROBOROS_PROD

        if (isHiddenInProd
            
    #if UNITY_EDITOR

            && !isVisibleInEditor

    #endif
            )
        {
            gameObject.SetActive(false);
        }

#endif
    }
}
}