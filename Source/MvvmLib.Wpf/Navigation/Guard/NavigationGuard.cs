using System;
using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    public class NavigationGuard 
    {
        Action<IActivatable, object> onActivationCanceled;
        Action<IDeactivatable> onDeactivationCanceled;

        public void SetCancellationCallback(Action<IActivatable, object> onActivationCanceled, Action<IDeactivatable> onDeactivationCanceled)
        {
            this.onActivationCanceled = onActivationCanceled;
            this.onDeactivationCanceled = onDeactivationCanceled;
        }

        public async Task<bool> CheckCanDeactivateAsync(IDeactivatable deactivatable)
        {
            if (deactivatable != null)
            {
                var result = await deactivatable.CanDeactivateAsync();
                if (!result)
                {
                    this.onDeactivationCanceled?.Invoke(deactivatable);
                }
                return result;
            }
            return true;
        }

        public async Task<bool> CheckCanActivateAsync(IActivatable activatable, object parameter)
        {
            if (activatable != null)
            {
                var result = await activatable.CanActivateAsync(parameter);
                if (!result)
                {
                    this.onActivationCanceled?.Invoke(activatable, parameter);
                }
                return result;
            }
            return true;
        }
    }
}
