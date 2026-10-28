using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resturant.Data.Entities
{
    public class Event
    {


        public Guid Id { get; set; }
        public Guid StreamId { get; set; }
        public EventItemType Type { get; set; }
        public int Price { get; set; }   




        public Event(Guid streamId, EventItemType type, int price)
        {
            Id = Guid.NewGuid();
            StreamId = streamId;
            Type = type;
            Price = price;
        }


    }
}
