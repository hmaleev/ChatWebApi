using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoWebApi.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public List<Message> Messages { get; set; }
    }
}