namespace MvvmLib.Adaptive
{
    public class DesignMode
    {
        private static bool? _isInDesignMode;
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
                }
                return _isInDesignMode.Value;
            }
        }
    }

}
