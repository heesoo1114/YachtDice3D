using UnityEngine;

public class DiceManager : MonoBehaviour
{
    private DiceContoller[] _diceController = new DiceContoller[5];

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
    }

    private bool AllDiceReady()
    {
        foreach (DiceContoller _controller in _diceController)
        {
            if (_controller.IsReady())
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void AllDiceRoll()
    {
        if (!AllDiceReady()) return;

        for (int i = 0; i < _diceController.Length; i++)
        {
            _diceController[i].RollCube();
        }
    }

    public void AllDiceReset()
    {
        foreach (DiceContoller _controller in _diceController)
        {
            _controller.Reset();
        }
    }
}
