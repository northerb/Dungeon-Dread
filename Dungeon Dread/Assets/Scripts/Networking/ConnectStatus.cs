using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectStatus
{
    Undefined,
    Success,
    ServerFull,
    GameInProgress,
    LoggedInAgain,
    UserRequestedDisconnect,
    GenericDisconnect
}

