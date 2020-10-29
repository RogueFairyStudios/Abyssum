using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DEEP.Stage
{
    public interface ITrigger
    {
        event Action OnTrigger;
    }
}
