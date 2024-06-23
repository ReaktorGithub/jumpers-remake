using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private bool[] _pedestal = new bool[4];
    private Vector3 _place1;
    private Vector3 _place2;
    private Vector3 _place3;
    private Vector3 _place4;

    private void Awake() {
        // _place1 = transform.Find("place1").gameObject.transform.position;
        // _place2 = transform.Find("place2").gameObject.transform.position;
        // _place3 = transform.Find("place3").gameObject.transform.position;
        // _place4 = transform.Find("place4").gameObject.transform.position;
        CleanPedestal();
    }

    public void SetPlayerToMaxPlace(PlayerControl player) {
        for (int i = 0; i < _pedestal.Length; i++) {
            if (_pedestal[i] == false) {
                player.PlaceAfterFinish = i + 1;
                _pedestal[i] = true;
                GameObject token = GameObject.Find(player.TokenName);
                // TokenControl tokenControl = token.GetComponent<TokenControl>();
                // GameObject clonedSprite = Instantiate(tokenControl.TokenImage);
                // clonedSprite.name = "pedestal_token_sprite";
                // RectTransform clonedRectTransform = clonedSprite.GetComponent<RectTransform>();
                // clonedRectTransform.SetParent(transform, false);
                // switch (i) {
                //     case 1:
                //     clonedSprite.transform.position = _place1;
                //     break;
                //     case 2:
                //     clonedSprite.transform.position = _place2;
                //     break;
                //     case 3:
                //     clonedSprite.transform.position = _place3;
                //     break;
                //     case 4:
                //     clonedSprite.transform.position = _place4;
                //     break;
                // }
                Destroy(token);
                break;
            }
        }
    }

    public void CleanPedestal() {
        _pedestal[0] = false;
        _pedestal[1] = false;
        _pedestal[2] = false;
        _pedestal[3] = false;
    }
}
