using System.Collections.Generic;

namespace MvvmLib.Adaptive
{
    public class AdaptiveContainer
    {
        protected Dictionary<string, object> container;

        public AdaptiveContainer()
        {
            this.container = new Dictionary<string, object>();
        }

        public static AdaptiveContainer Create()
        {
            return new AdaptiveContainer();
        }

        public AdaptiveContainer Set(string name, object value)
        {
            if(this.container == null)
            {
                this.container = new Dictionary<string, object>();
            }
            this.container[name] = value;
            return this;
        }

        public Dictionary<string, object> Get()
        {
            return this.container;
        }
    }

}
