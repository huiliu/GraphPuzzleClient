
namespace GraphGame.Client
{
    public class DelayHideComponent
        : DelayToDoComponent
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            this.OnTimeout += this.HandleTimeout;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.OnTimeout -= this.HandleTimeout;
        }

        private void HandleTimeout(DelayToDoComponent delayToDoComponent)
        {
            this.gameObject.SetActive(false);
        }
    }
}
