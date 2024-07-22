using UnityEngine;

public class PlayersControl : MonoBehaviour
{
    [SerializeField] private float finishDelay = 0.5f;
    [SerializeField] private float loseDelay = 2f;
    [SerializeField] private float redEffectDelay = 1f;

    public float FinishDelay {
        get { return finishDelay; }
        private set {}
    }

    public float LoseDelay {
        get { return loseDelay; }
        private set {}
    }

    public float RedEffectDelay {
        get { return redEffectDelay; }
        private set {}
    }
}
