using System;
using Workstation.ServiceModel.Ua;

[Subscription(endpointUrl: "opc.tcp://172.31.1.147:4840", publishingInterval: 50)]
class Test_R_HOME : SubscriptionBase
{
    /// <summary>
    /// Gets or sets the value of ProgramCubeCommandCntrlCmd.
    /// </summary>
    [MonitoredItem(nodeId: "ns=5;s=MotionDeviceSystem.ProcessData.Special.SpecialData.IA_R_START")]
    public bool R_HOLD
    {
        get { return this.r_home; }
        set { this.SetProperty(ref this.r_home, value); }
    }
    private bool r_home;

}