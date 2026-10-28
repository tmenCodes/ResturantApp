using MediatR;
using Resturant.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resturant.Domain.Entity
{
    public abstract class AggregateRoot : Entity
    {

        protected AggregateRoot(Guid id) : base(id) { }

        //a parameterless ctor is needed for ef framework 
        protected AggregateRoot()
        {
            
        }
        //this value cannot be null when initilizaing
        private List<INotification>? _domainEvents = new();

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents!.ToList();

        protected void RaiseDomainEvent(INotification eventItem)
        {
            //chekc to make sure _domainevents is not null
            _domainEvents = _domainEvents ?? new List<INotification>();

            _domainEvents.Add(eventItem);
        }

     
        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }


    }
}
