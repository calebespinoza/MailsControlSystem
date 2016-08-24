using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailsControlSystem.Models
{
    public class IOMailWrapperModel
    {     
        public Incoming incoming { get; set; }
        public Outcoming outcoming { get; set; }        
        public Mail mail { get; set; }

        public IOMailWrapperModel()
        {           
            incoming = new Incoming();
            outcoming = new Outcoming();
            mail = new Mail();        
        }
    }
}