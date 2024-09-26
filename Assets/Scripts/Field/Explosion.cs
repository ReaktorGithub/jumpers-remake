using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject _sprite;
    private Animator _anim;

    private void Awake() {
        _anim = _sprite.GetComponent<Animator>();
        _sprite.SetActive(false);
    }

    public void SetPosition(Vector3 position) {
        transform.localPosition = position;
    }

    public void Explode() {
        StartCoroutine(ExplodeDefer());
    }

    private IEnumerator ExplodeDefer() {
        _sprite.SetActive(true);
        _anim.SetBool("isExplode", true);
        yield return new WaitForSeconds(2f);
        _anim.SetBool("isExplode", false);
        _sprite.SetActive(false);
    }
}
