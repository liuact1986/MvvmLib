using System;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The back request manager contract.
    /// </summary>
    public interface IBackRequestManager
    {

        /// <summary>
        /// Handles SystemNavigationManager back requested.
        /// </summary>
        /// <param name="frame">The frame</param>
        /// <param name="callback">Go back callback</param>
        void Handle(Frame frame, Action callback);

        /// <summary>
        /// Unhandles SystemNavigationManager back requested.
        /// </summary>
        void Unhandle();
    }
}