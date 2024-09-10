using System;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Models
{
    public abstract class BaseModel<T> : IDisposable
    {
        public virtual void Dispose()
        {
            
        }
    }
}