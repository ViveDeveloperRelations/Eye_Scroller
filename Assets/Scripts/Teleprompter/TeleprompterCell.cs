using GamesTan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleprompterCell : MonoBehaviour, IScrollCell {
    public TextMeshProUGUI text;
    private TeleprompterCellData _data;

    public void BindData(TeleprompterCellData data) {
        _data = data;
        text.text = _data.text;
        //BtnItem.onClick.RemoveListener(OnClick_BtnItem);
        //BtnItem.onClick.AddListener(OnClick_BtnItem);
        //TextCount.text = data.Count.ToString();
        //TextName.text = data.Name.ToString();
        //name = "Cell " + data.Idx;
    }

    void OnClick_BtnItem() {
        UnityEngine.Debug.Log(" Click " + _data);
    }
}