using System.Linq;
using UnityEngine;

/// <summary>
/// ��D 3 ���̐����E�����ւ����s���}�l�[�W��
/// ������������������������������������������������������������������������������������������
/// �EScene �N���� / �X�e�[�W�J�n���� InitHand() ���Ă�
/// �E�~�m�iPieceUI�j��Ֆʂɒu������ ReplacePiece() ���Ă��
///   ��D����� 3 ���ɕۂ�
/// </summary>
public class HandManager : MonoBehaviour
{
    [Header("��D����ׂ�e")]
    public RectTransform handRoot;

    [Header("��D�s�[�X�̃v���n�u (PieceUI)")]
    public PieceUI pieceUIPrefab;

    const int HAND_SIZE = 3;

    // �X�e�[�W�ݒ���󂯎���ĕێ�����
    StageData _stageData;

    [Header("���̃X�e�[�W�ŏo���\�� PieceData")]
    public PieceData[] allowedPieces;

    /// <summary>�X�e�[�W�J�n���ɌĂ�</summary>
    public void InitHand(StageData stageData)
    {
        Debug.Log("InitHand");

        _stageData = stageData;

        // �����̎�D���N���A
        foreach (Transform t in handRoot)
            Destroy(t.gameObject);

        // �K�v������������
        for (int i = 0; i < HAND_SIZE; i++)
            CreatePiece();
    }

    /// <summary>
    /// �Ֆʂɔz�u���I����� PieceUI ��n���ƁA
    /// �j�����ĐV�����s�[�X���[����
    /// </summary>
    public void ReplacePiece(PieceUI used)
    {
        if (used != null)
            Destroy(used.gameObject);

        CreatePiece();
    }

    /// <summary>��D�s�[�X�� 1 �������� handRoot �ɕ��ׂ�</summary>
    void CreatePiece()
    {
        if (_stageData == null ||
            _stageData.allowedPieces == null ||
            _stageData.allowedPieces.Length == 0)
        {
            Debug.LogError("�X�e�[�W�ݒ�� allowedPieces ����ł�");
            return;
        }

        // �d�ݕt�������_���� PieceData ��I��
        var data = allowedPieces[Random.Range(0, allowedPieces.Length)];
        if (data == null) return;

        var ui = Instantiate(pieceUIPrefab, handRoot);
        ui.Init(data);
    }

    /// <summary>
    /// �X�e�[�W�f�[�^�� allowedPieces ����A
    /// weight �ɏ]�����d�ݕt�������_���� PieceData ��Ԃ�
    /// </summary>
    PieceData PickWeightedRandomPiece()
    {
        // 1) ���v�E�F�C�g���v�Z
        int total = 0;
        foreach (var wp in _stageData.allowedPieces)
            total += Mathf.Max(wp.weight, 0);

        // 2) ���������� [0, total)
        int r = Random.Range(0, total);

        // 3) �ݐς��Ē������ŏ��̗v�f��ԋp
        int sum = 0;
        foreach (var wp in _stageData.allowedPieces)
        {
            sum += Mathf.Max(wp.weight, 0);
            if (r < sum)
                return wp.piece;
        }

        // ����̃t�H�[���o�b�N
        return _stageData.allowedPieces.Last().piece;
    }

    /// <summary>
    /// ��D�̂����ꂩ���Ֆʂ̂ǂ����ɒu���邩�`�F�b�N
    /// </summary>
    public bool HasPlacablePiece(GridManager grid)
    {
        foreach (Transform t in handRoot)
        {
            var ui = t.GetComponent<PieceUI>();
            if (ui == null) continue;

            // �ՖʑS�Z���𑖍����Ĉ�J���ł��u����� true
            for (int x = 0; x < GridManager.Width; x++)
                for (int y = 0; y < GridManager.Height; y++)
                    if (grid.CanPlace(ui.data.cells, new Vector2Int(x, y)))
                        return true;
        }
        return false;   // ���ׂĒu���Ȃ�
    }

}