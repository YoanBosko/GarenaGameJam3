using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADtoMoveScript : MonoBehaviour
{
    public void MoveA()
    {
        PlayerController.Instance.MoveLeft();;
    }

    public void MoveD()
    {
        PlayerController.Instance.MoveRight();;
    }

    public void Jump()
    {
        PlayerController.Instance.MoveJump();
    }

    public void Attack()
    {
        PlayerController.Instance.MoveAttack();
    }
}
