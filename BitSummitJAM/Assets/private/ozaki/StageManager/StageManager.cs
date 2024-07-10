using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField]
    private int StageNumber;

    [SerializeField]
    [Tooltip("地面タイルマップ")]
    private TileBase[] groundstile;

    [SerializeField]
    [Tooltip("タイル座標取得用")]
    private Tilemap tilePos;

    [SerializeField]
    [Tooltip("テストオブジェクト")]
    private GameObject[] mapchip;
    private enum GroundChip
    {
        /// <summary>
        /// 何もなし
        /// </summary>
        EMPTY,
        /// <summary>
        /// 床１
        /// </summary>
        GROUNGTILE01,
        /// <summary>
        /// 床２
        /// </summary>
        GROUNGTILE02,
        /// <summary>
        /// 床３
        /// </summary>
        GROUNGTILE03,
        /// <summary>
        /// 床４
        /// </summary>
        GROUNGTILE04,
        /// <summary>
        /// 床５
        /// </summary>
        GROUNGTILE05,
        /// <summary>
        /// 床６
        /// </summary>
        GROUNGTILE06,
        /// <summary>
        /// 床７
        /// </summary>
        GROUNGTILE07,
        /// <summary>
        /// 床８
        /// </summary>
        GROUNGTILE08,
        /// <summary>
        /// 床９
        /// </summary>
        GROUNGTILE09,
        /// <summary>
        /// カニ
        /// </summary>
        CRAB,
        /// <summary>
        /// カニ
        /// </summary>
        CRAB2,
        /// <summary>
        /// ダツ
        /// </summary>
        NEEDLEFISH,
        /// <summary>
        /// ダツ
        /// </summary>
        NEEDLEFISH2,
        /// <summary>
        /// ヤドカリ
        /// </summary>
        HERMITCRAB,
        /// <summary>
        /// ボスヤドカリ
        /// </summary>
        BOSSHERMITCRAB,
    }

    // Start is called before the first frame update
    void Start()
    {

        switch(StageNumber)
        {
            case 1:
                var data = JsonReader.LoadStage("stage01");

                StageGenerator(data);
                break;
            case 2:
                var data2 = JsonReader.LoadStage("stage02");

                StageGenerator(data2);
                break;
        }
        

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StageGenerator(StageData hoge)
    {
        //タイルマップ初期化する
        tilePos.ClearAllTiles();

        //左上から順番においていく
        for (int row = 0; row < hoge.Height; ++row)
        {
            for (int col = 0; col < hoge.Width; ++col)
            {
                //座標を取得と整えてる
                Vector3Int position = new(col, hoge.Height - row - 1);

                //一時配列なので座標足す
                var chip = (GroundChip)hoge.Mapchip[col + row * hoge.Width];

                switch (chip)
                {
                    case GroundChip.EMPTY:
                        break;
                    case GroundChip.GROUNGTILE01:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE02:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE03:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE04:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE05:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE06:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE07:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE08:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.GROUNGTILE09:
                        TileMapPut(chip, new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.CRAB:
                        Crab(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.CRAB2:
                        Crab(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.NEEDLEFISH:
                        NeebleFish(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.NEEDLEFISH2:
                        NeebleFish(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.HERMITCRAB:
                        HermtCrab(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                    case GroundChip.BOSSHERMITCRAB:
                        BossHermitCrab(chip - 10, tilePos.GetCellCenterWorld(position), new(col, hoge.Height - (row + 1)));
                        break;
                }
            }
        }
    }

    private void TileMapPut(GroundChip chipCode, Vector3Int fuga)
    {
        tilePos.SetTile(fuga, groundstile[(int)chipCode]);
    }


    /// <summary>
    /// 対応するカニを配置する
    /// </summary>
    /// <param name="chipCode">オブジェクト番号</param>
    /// <param name="hoge">ポジション</param>
    private void Crab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //オブジェクト配置
        Instantiate(mapchip[(int)chipCode], new (hoge.x,hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f)-0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// 対応するダツを配置する
    /// </summary>
    /// <param name="chipCode">オブジェクト番号</param>
    /// <param name="hoge">ポジション</param>
    private void NeebleFish(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //オブジェクト配置
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) - 0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// 対応するヤドカリを配置する
    /// </summary>
    /// <param name="chipCode">オブジェクト番号</param>
    /// <param name="hoge">ポジション</param>
    private void HermtCrab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //オブジェクト配置
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) + 0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// 対応するボスヤドカリを配置する
    /// </summary>
    /// <param name="chipCode">オブジェクト番号</param>
    /// <param name="hoge">ポジション</param>
    private void BossHermitCrab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //オブジェクト配置
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) - 0.5f, hoge.z = 0), Quaternion.identity);

    }

}
