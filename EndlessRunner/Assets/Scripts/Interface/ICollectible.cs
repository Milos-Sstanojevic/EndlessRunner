using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible
{
    public void Collect(ICollectible collectible);
}
