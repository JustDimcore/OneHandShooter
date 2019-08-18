using System;
using System.Collections.Generic;

public interface IEnemiesGenerator
{
    void SubscribeOnAdd(Action<IEnumerable<Enemy>> enemies);
}