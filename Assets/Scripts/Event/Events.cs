using UnityEngine;
using Utils;

namespace CaosCreations
{
    public class SkinSelectedEvent : IEvent
    {
        public Skin skin;

        public SkinSelectedEvent(Skin skin)
        {
            this.skin = skin;
        }
    }
}
