using MAIO.JDUS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.ASOS
{
    class ASOS
    {

        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public Main.taskset tk = null;
        public ASOSAPI aSOSAPI = new ASOSAPI();
        public ASOSsession session = new ASOSsession();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            try
            {
               
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
      /*  public string GetauthorizeID(CancellationToken ct)
        {
        }
        public string GetSkuid(CancellationToken ct)
        {
         
        }
        public void Addtocart(CancellationToken ct, string accesstoken, string skuid)
        {
        }*/
    }
}
