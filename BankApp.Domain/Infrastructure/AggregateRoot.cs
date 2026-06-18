using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Domain.Infrastructure
{
    public abstract class AggregateRoot : Entity
    {
        // usually an interface rather than a base class, but we can do it as a base class if we want to have some shared logic for all aggregate roots, such as tracking changes, etc. - this is the base class for all aggregate roots in the domain, such as customer
        // here we made this a base class so we could still refer to the dot Id property, which is common to all aggregate roots, and then we can have logic in the base class to handle that Id property, such as checking for equality based on Id, etc.

        // a lot of the time though we just have this empty as a marker class/interface


        // inerhitance vs base class choice:
            // you can only inherit from one base class, but you can implement multiple interfaces, so if you need to have some shared logic for all aggregate roots, then a base class is a good choice, but if you just need to mark something as an aggregate root and don't need any shared logic, then an interface is a better choice
                // don't burn your inheritance opportunity
                // look further into inheritance chains ***

                // ex: Liskov square/rectangle inheritance problem - if you have a base class of shape/sides, then you can just have a shape be a collection of side objects, which you can use to make any shape like square or rectangle - composition rather than inheritance to solve this
    }
}