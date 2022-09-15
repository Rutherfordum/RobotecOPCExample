using System;
using System.Windows.Forms;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OPCUA_Free
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Init()
        {
            var clientDescription = new ApplicationDescription
            {
                ApplicationName = "Workstation.UaClient.FeatureTests",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:Workstation.UaClient.FeatureTests",
                ApplicationType = ApplicationType.Client
            };

            var channel = new UaTcpSessionChannel(
                clientDescription,
                null, // no x509 certificates
                new UserNameIdentity("",""), // user identity
                "opc.tcp://opcua.rocks:4840", // the public endpoint of a server at opcua.rocks.
                SecurityPolicyUris.None); // no encryption
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
