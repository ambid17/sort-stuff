using UnityEngine;
using Utils;

namespace CaosCreations
{
    public class BowlSkinSelectedEvent : IEvent { }

    public class EnvironmentSelectedEvent : IEvent { }

    public class GameStartedEvent : IEvent { }

    public class  GameEndedEvent : IEvent { }

    public class  ItemSortedEvent : IEvent
    {
       public Sortable item;

       public ItemSortedEvent(Sortable item)
       {
           this.item = item;
        }
    }
}
