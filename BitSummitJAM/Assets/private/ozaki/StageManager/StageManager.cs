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
    [Tooltip("�n�ʃ^�C���}�b�v")]
    private TileBase[] groundstile;

    [SerializeField]
    [Tooltip("�^�C�����W�擾�p")]
    private Tilemap tilePos;

    [SerializeField]
    [Tooltip("�e�X�g�I�u�W�F�N�g")]
    private GameObject[] mapchip;
    private enum GroundChip
    {
        /// <summary>
        /// �����Ȃ�
        /// </summary>
        EMPTY,
        /// <summary>
        /// ���P
        /// </summary>
        GROUNGTILE01,
        /// <summary>
        /// ���Q
        /// </summary>
        GROUNGTILE02,
        /// <summary>
        /// ���R
        /// </summary>
        GROUNGTILE03,
        /// <summary>
        /// ���S
        /// </summary>
        GROUNGTILE04,
        /// <summary>
        /// ���T
        /// </summary>
        GROUNGTILE05,
        /// <summary>
        /// ���U
        /// </summary>
        GROUNGTILE06,
        /// <summary>
        /// ���V
        /// </summary>
        GROUNGTILE07,
        /// <summary>
        /// ���W
        /// </summary>
        GROUNGTILE08,
        /// <summary>
        /// ���X
        /// </summary>
        GROUNGTILE09,
        /// <summary>
        /// �J�j
        /// </summary>
        CRAB,
        /// <summary>
        /// �J�j
        /// </summary>
        CRAB2,
        /// <summary>
        /// �_�c
        /// </summary>
        NEEDLEFISH,
        /// <summary>
        /// �_�c
        /// </summary>
        NEEDLEFISH2,
        /// <summary>
        /// ���h�J��
        /// </summary>
        HERMITCRAB,
        /// <summary>
        /// �{�X���h�J��
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
        //�^�C���}�b�v����������
        tilePos.ClearAllTiles();

        //���ォ�珇�Ԃɂ����Ă���
        for (int row = 0; row < hoge.Height; ++row)
        {
            for (int col = 0; col < hoge.Width; ++col)
            {
                //���W���擾�Ɛ����Ă�
                Vector3Int position = new(col, hoge.Height - row - 1);

                //�ꎞ�z��Ȃ̂ō��W����
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
    /// �Ή�����J�j��z�u����
    /// </summary>
    /// <param name="chipCode">�I�u�W�F�N�g�ԍ�</param>
    /// <param name="hoge">�|�W�V����</param>
    private void Crab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //�I�u�W�F�N�g�z�u
        Instantiate(mapchip[(int)chipCode], new (hoge.x,hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f)-0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// �Ή�����_�c��z�u����
    /// </summary>
    /// <param name="chipCode">�I�u�W�F�N�g�ԍ�</param>
    /// <param name="hoge">�|�W�V����</param>
    private void NeebleFish(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //�I�u�W�F�N�g�z�u
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) - 0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// �Ή����郄�h�J����z�u����
    /// </summary>
    /// <param name="chipCode">�I�u�W�F�N�g�ԍ�</param>
    /// <param name="hoge">�|�W�V����</param>
    private void HermtCrab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //�I�u�W�F�N�g�z�u
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) + 0.5f, hoge.z = 0), Quaternion.identity);

    }
    /// <summary>
    /// �Ή�����{�X���h�J����z�u����
    /// </summary>
    /// <param name="chipCode">�I�u�W�F�N�g�ԍ�</param>
    /// <param name="hoge">�|�W�V����</param>
    private void BossHermitCrab(GroundChip chipCode, Vector3 hoge, Vector2Int fuga)
    {
        //�I�u�W�F�N�g�z�u
        Instantiate(mapchip[(int)chipCode], new(hoge.x, hoge.y + (mapchip[(int)chipCode].transform.localScale.y * 0.5f) - 0.5f, hoge.z = 0), Quaternion.identity);

    }

}
