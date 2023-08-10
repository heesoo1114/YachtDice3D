using System.Collections;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    private DiceContoller[] _diceController = new DiceContoller[5];
    private DiceContoller _selectController = null;

    private bool isRollDone;

    private bool isSelecting;
    private bool isChanging;
    private int selectedIndex = 0;
    private int initIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _diceController[i] = transform.GetChild(i).GetComponent<DiceContoller>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AllDiceRoll();
        }

        if (isRollDone && !isChanging)
        {
            float inputH = Input.GetAxisRaw("Horizontal");

            if (inputH != 0)
            {
                ChangeSelectDice(inputH);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isSelecting)
            {
                _selectController.SetDiceKeepOrOut();
            }
        }
    }

    #region Select

    public void StartSelectDice()
    {
        isSelecting = true;

        _selectController = _diceController[initIndex];
        ChangeSelectDice();
    }

    public void EndSelectDice()
    {
        if (_selectController != null)
        {
            _selectController.SelectScaleAnimStop();
            _selectController = null;
            selectedIndex = initIndex;
        }
        isSelecting = false;
    }

    public void ChangeSelectDice(float axis = 0)
    {
        StartCoroutine(SetSelectDice(selectedIndex + (int)axis));
    }

    private IEnumerator SetSelectDice(int currentIndex)
    {
        if (currentIndex < 0 || currentIndex >= _diceController.Length) yield break;

        isChanging = true;

        _selectController.SelectScaleAnimStop(); // 바꾸기 전에 애니메이션 실행
        _selectController = _diceController[currentIndex]; // 바꿔줌
        _selectController.SelectScaleAnimPlay(); // 바꾼 주사위 애니메이션 실행
        
        selectedIndex = currentIndex;

        yield return new WaitForSeconds(_selectController.SelectAnimSpeed);
        isChanging = false;
    }

    #endregion

    #region Check

    // 모든 다이스들이 롤 할 준비가 되었는지
    private bool AllDiceReady()
    {
        foreach (DiceContoller _controller in _diceController)
        {
            if (!_controller.IsReady())
            {
                return false;
            }
        }
        return true;
    }

    // 주사위 중 하나라도 이상한 상태인 경우
    public bool IsCorrectRoll() 
    {
        foreach (DiceContoller _controller in _diceController)
        {
            if (_controller.DiceNum == 0)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Flow

    public void AllDiceRoll()
    {
        if (!AllDiceReady()) return;

        EndSelectDice();

        isRollDone = false;
        foreach (DiceContoller _controller in _diceController)
        {
            _controller.RollCube();
        }

        StartCoroutine(LaterRollCheck());
    }

    private IEnumerator LaterRollCheck()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (AllDiceReady())
            {
                AllDiceReset();

                if (!IsCorrectRoll())
                {
                    Debug.Log("is not correct");
                    yield return new WaitForSeconds(_diceController[0].MoveAnimSpeed);
                    AllDiceRoll();
                }
                else
                {
                    Debug.Log("is correct");
                    RollDone();
                }

                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 롤이 잘 마무리가 되었을 때 실행되는 함수
    private void RollDone()
    {
        isRollDone = true;
        StartSelectDice();
    }
    
    public void AllDiceReset()
    {
        if (!AllDiceReady()) return;

        foreach (DiceContoller _controller in _diceController)
        {
            _controller.ResetCube();
        }
    }

    #endregion
}
