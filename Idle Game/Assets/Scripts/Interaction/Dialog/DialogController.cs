using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    //public static DialogController instance;

    //[SerializeField] private List<Dialog> _dialogs = new();
    //private int dialogIndex;
    //public bool isTalking;

    //public DialogUI _dialogUI;
    //private EntityController _entityController;
    //[SerializeField] private OpenCloseUI _openCloseUI;

    //private void Awake()
    //{
    //    instance = this;
    //}

    //public void StartDialog(int index)
    //{
    //    StartDialog(index, _entityController);
    //}

    //public void StartDialog(int index, EntityController _entityController = null)
    //{
    //    PlayerController _playerController = PlayerController.instance;

    //    if (isTalking)
    //    {
    //        _dialogUI.SpeedUpDialog(_dialogs[dialogIndex].text);
    //        return;
    //    }

    //    //Assigning values
    //    this._entityController = _entityController;
    //    dialogIndex = index;
    //    _playerController.isStopped = true;

    //    if (_entityController != null)
    //    {
    //        _entityController._entityWalk.isStopped = true;
    //        _entityController._entityWalk.GoTo(_playerController.transform.position, _playerController.transform.position);
    //        _dialogUI._npcPreview.UpdateAllByEntity(_entityController.GetComponent<EntityLookController>()._entityLook, _entityController._holdingController._itemController._gearHolder);
    //        _dialogUI.nameText.text = _entityController._entityInfo.name;

    //        //Checking if player is talking to right npc
    //        QuestController.instance.InvokeTalkEvent(_entityController._entityInfo.ID);
    //    }

    //    _openCloseUI.Open();
    //    _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    //    isTalking = true;
    //}

    //public void ChangeDialog(int index)
    //{
    //    if (!_dialogUI.finishedSpelling)
    //    {
    //        _dialogUI.SpeedUpDialog(_dialogs[dialogIndex].text);
    //        return;
    //    }

    //    dialogIndex = index;
    //    _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    //}

    //public void ForceChangeDialog(int index)
    //{
    //    if (!_dialogUI.finishedSpelling)
    //        _dialogUI.SpeedUpDialog(_dialogs[dialogIndex].text);

    //    dialogIndex = index;
    //    _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    //}

    //public void EndDialog()
    //{
    //    if (!_dialogUI.finishedSpelling)
    //    {
    //        _dialogUI.SpeedUpDialog(_dialogs[dialogIndex].text);
    //        return;
    //    }

    //    PlayerController.instance.isStopped = false;
    //    if (_entityController != null)
    //        _entityController._entityWalk.isStopped = false;

    //    _openCloseUI.Close();
    //    isTalking = false;
    //}
}
