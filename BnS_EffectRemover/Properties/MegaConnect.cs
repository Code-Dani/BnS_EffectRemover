using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CG.Web.MegaApiClient;

namespace BnS_EffectRemover
{
    class MegaConnect
    {
        public List<INode> items { get; set; }
        public MegaApiClient client { get; set; }
        public MegaConnect(MegaApiClient client)
        {
            this.client = client;
        }

        public void StatusCheck()
        {

            try
            {
                Uri fileLink = new Uri("https://mega.nz/folder/yoYB0QRB#uy2hyl6Gf8skMUuz_lK3oQ");
                IEnumerable<INode> node = client.GetNodesFromLink(fileLink);
                this.items = node.ToList();
                Thread.CurrentThread.Abort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
