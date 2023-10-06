using UnityEngine;
using UnityEngine.UI;

public class UpdateOverview : MonoBehaviour
{
    public ScrollRect scrollRect;             // ScrollRectコンポーネントを参照するための変数
    public GameObject content;                // ScrollView内のコンテンツを参照するための変数
    public float refreshThreshold = 50f;
    public GameObject overviewObj;
    private bool updateFlg;
    // 更新をトリガーする引っ張りの閾値

    private Vector2 startDragPos;             // ドラッグ開始時のNormalized Position
    private bool isDragging = false;          // ドラッグ中かどうかを示すフラグ

    private void Start()
    {
        // ScrollRectのValueChangedイベントにOnScrollValueChanged関数をリスナーとして登録
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        updateFlg = true;
    }

    private async void OnScrollValueChanged(Vector2 value)
    {
        // ドラッグ中でない場合は何もしない
        if (!isDragging)
            return;

        // スクロールが上方向に動いていて、閾値以上引っ張られた場合
        if (scrollRect.verticalNormalizedPosition >= 1f &&
            value.y > startDragPos.y + (refreshThreshold / content.GetComponent<RectTransform>().sizeDelta.y))
        {
            // 更新処理をトリガー
            if(updateFlg){
                updateFlg = false;
                isDragging = false;
                scrollRect.verticalNormalizedPosition = 0f;
                Debug.Log("Refresh triggered!");
                await overviewObj.GetComponent<HomeMaster>().awaitReloaddata();
                updateFlg = true;
            }

            // ここに更新処理を実装します。
            // 例えば、新しいデータの読み込みやUI要素の更新を行うことが考えられます。

            // 更新処理が完了したら、以下のようにScrollViewを元の位置に戻すことができます。
            // scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 1f);
        }
    }

    public void OnBeginDrag()
    {
        // ドラッグ開始時のNormalized Positionを記録し、ドラッグ中フラグを設定
        if (updateFlg)
        {
            isDragging = true;
            startDragPos = scrollRect.normalizedPosition;
        }

    }

    public void OnEndDrag()
    {
        // ドラッグ終了時にドラッグ中フラグをクリア
        isDragging = false;
    }
}
