using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class BasketVisual : MonoBehaviour
  {
    [SerializeField] private DOTweenAnimation _doTweenAnimation;

    private EventBinding<EventStructs.BasketReceivedItemEvent> _basketReceivedItemEvent;

    private void OnEnable()
    {
      _basketReceivedItemEvent = new EventBinding<EventStructs.BasketReceivedItemEvent>(PlayPunchANimation);
    }

    private void OnDisable()
    {
      _basketReceivedItemEvent.Remove(PlayPunchANimation);
    }

    private void PlayPunchANimation(EventStructs.BasketReceivedItemEvent basketReceivedItemEvent)
    {
      _doTweenAnimation.DORestart();
    }
  }
}