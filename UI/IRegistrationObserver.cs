using UnityEngine;
using Backtory.Core.Public;
public interface IRegistrationObserver
{
    void RecieveRegistrationStatus(IBacktoryResponse<BacktoryUser> registrationResponse);
}
