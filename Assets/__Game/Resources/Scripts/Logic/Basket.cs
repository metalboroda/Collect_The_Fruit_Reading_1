using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Infrastructure;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class Basket : MonoBehaviour
  {
    [SerializeField] private CorrectValuesContainerSo _correctValuesContainerSo;
    [field: Header("Tutorial")]
    [field: SerializeField] public bool Tutorial { get; private set; }

    private List<TreeItem> _correctItems = new List<TreeItem>();
    private List<TreeItem> _incorrectItems = new List<TreeItem>();
    private bool _canReceiveItems = true;
    private TreeItem _placedTreeItem;
    private int _correctCounter;
    private int _incorrectCounter;

    private GameBootstrapper _gameBootstrapper;

    private EventBinding<EventStructs.SpawnedItemsEvent> _spawnedItemsEvent;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable()
    {
      _spawnedItemsEvent = new EventBinding<EventStructs.SpawnedItemsEvent>(ReceiveSpawnedItems);
    }

    private void OnDisable()
    {
      _spawnedItemsEvent.Remove(ReceiveSpawnedItems);
    }

    private void Start()
    {
      EventBus<EventStructs.ComponentEvent<Basket>>.Raise(new EventStructs.ComponentEvent<Basket> { Data = this });
    }

    private void ReceiveSpawnedItems(EventStructs.SpawnedItemsEvent spawnedItemsEvent)
    {
      _correctItems = spawnedItemsEvent.CorrectItems;
      _incorrectItems = spawnedItemsEvent.IncorrectItems;
    }

    public void PlaceItem(TreeItem treeItem)
    {
      if (_canReceiveItems == false) return;
      if (_gameBootstrapper.StateMachine.CurrentState is not GameplayState) return;

      _placedTreeItem = treeItem;

      treeItem.transform.SetParent(transform);
      treeItem.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
      {
        CheckForCorrectItem();

        _canReceiveItems = true;
      });

    }

    private void CheckForCorrectItem()
    {
      if (_correctValuesContainerSo.CorrectValues.Contains(_placedTreeItem.Answer))
      {
        _correctCounter++;
        _correctItems.Remove(_placedTreeItem);

        Destroy(_placedTreeItem.gameObject);

        if (_correctCounter >= _correctItems.Count)
          _gameBootstrapper.StateMachine.ChangeState(new GameWinState(_gameBootstrapper));

        if (Tutorial == false)
          EventBus<EventStructs.LevelPointEvent>.Raise(new EventStructs.LevelPointEvent { LevelPoint = 1 });

        EventBus<EventStructs.BasketPlacedItemEvent>.Raise(new EventStructs.BasketPlacedItemEvent
        {
          Correct = true,
          CorrectIncrement = 1
        });
      }
      else
      {
        _incorrectCounter++;
        _incorrectItems.Remove(_placedTreeItem);

        Destroy(_placedTreeItem.gameObject);

        if (_incorrectCounter >= _incorrectItems.Count)
          _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));

        EventBus<EventStructs.BasketPlacedItemEvent>.Raise(new EventStructs.BasketPlacedItemEvent
        {
          Correct = false,
          IncorrectIncrement = 1
        });
      }

      EventBus<EventStructs.BasketReceivedItemEvent>.Raise(new EventStructs.BasketReceivedItemEvent());
    }
  }
}