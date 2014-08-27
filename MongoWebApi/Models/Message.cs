using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoWebApi.Models
{
    public class Message
    {
        public string Content { get; set; }
        public string Time { get; set; }
        public string Sender { get; set; }
       
    }
}