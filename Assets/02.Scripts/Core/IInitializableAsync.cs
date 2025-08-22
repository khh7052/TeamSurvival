using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IInitializableAsync 
{
    bool IsInitialized { get; }
    void InitializeAsync();

}
