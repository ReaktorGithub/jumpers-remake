using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject _sprite, _glow0, _glow1, _glow2, _glow3, _glowBox;
    private Animator _anim;
    private bool _animate = false;

    private void Awake() {
        _anim = _sprite.GetComponent<Animator>();
        _sprite.SetActive(false);
        HideGlowSprites();
    }

    private void HideGlowSprites() {
        _glow0.SetActive(false);
        _glow1.SetActive(false);
        _glow2.SetActive(false);
        _glow3.SetActive(false);
    }

    private void ShowGlowSprites(int areaSize) {
        _glow0.SetActive(true);
        _glow1.SetActive(true);
        _glow2.SetActive(areaSize > 1);
        _glow3.SetActive(areaSize == 3);
    }

    public void SetPosition(Vector3 position) {
        transform.localPosition = position;
    }

    public void Explode(int areaSize) {
        StartCoroutine(StartGlowAnimation(areaSize));
        StartCoroutine(ExplodeDefer());
    }

    private IEnumerator ExplodeDefer() {
        _sprite.SetActive(true);
        _anim.SetBool("isExplode", true);
        yield return new WaitForSeconds(2f);
        _anim.SetBool("isExplode", false);
        _sprite.SetActive(false);
        _animate = false;
        HideGlowSprites();
    }

    private IEnumerator StartGlowAnimation(int areaSize) {
        _animate = true;

        while (_animate) {
            ShowGlowSprites(areaSize);
            yield return new WaitForSeconds(0.3f);
            HideGlowSprites();
            yield return new WaitForSeconds(0.3f);
        }
    }
}
