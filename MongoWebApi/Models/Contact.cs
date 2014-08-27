using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoWebApi.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public List<Message> Messages { get; set; }
        public string Status { get; set; }
    }
}