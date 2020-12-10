using Assets.src.body;
using System.Collections.Generic;

namespace Assets.src.collision
{
    public abstract class ACollision
    {
        public abstract void findContacts(List<Body> bodies, List<ACollisionContact> contacts);
    }
}
